using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 25f;
    public float lifetime = 3f;
    public float radius = 0.1f; // Радиус вашей круглой пули (настройте под размер сферы)

    // Временная переменная для теста рикошета. 
    // Позже перенесем управление этой переменной в общий менеджер игры/игрока
    public bool isMirrorModeActive = true;
    public int maxBounces = 3; // Максимальное количество отскоков

    private int currentBounces = 0;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // Расстояние, которое пуля должна пролететь в этом кадре
        float step = speed * Time.deltaTime;

        // Пускаем луч из центра пули вперед по направлению движения
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        // Проверяем, столкнется ли пуля со стеной на этом шаге (с учетом её радиуса)
        if (Physics.Raycast(ray, out hit, step + radius))
        {
            // Проверяем, врезались ли мы в стену (убедитесь, что у кубов-стен в Unity стоит тег "Wall")
            if (hit.collider.CompareTag("Wall"))
            {
                if (isMirrorModeActive && currentBounces < maxBounces)
                {
                    // Вычисляем вектор идеального математического отскока сферы
                    Vector3 reflectDir = Vector3.Reflect(transform.forward, hit.normal);

                    // Поворачиваем пулю в сторону отскока
                    transform.forward = reflectDir;

                    // Телепортируем пулю ровно в точку удара, чуть отодвинув её по нормали на величину радиуса
                    transform.position = hit.point + hit.normal * radius;

                    currentBounces++;
                }
                else
                {
                    // Если режим зеркал выключен или лимит отскоков исчерпан — уничтожаем пулю
                    Destroy(gameObject);
                }
            }
            else
            {
                // Если врезались во что-то другое (не в стену)
                if (hit.collider.CompareTag("Enemy"))
                {
                    // Получаем скрипт дрона и наносим урон
                    EnemyDrone drone = hit.collider.GetComponentInParent<EnemyDrone>();

                    if (drone != null)
                    {
                        drone.TakeDamage();
                    }
                    Destroy(gameObject);
                    return;
                }

                Destroy(gameObject); // Уничтожаем пулю
            }
        }
        else
        {
            // Если впереди чисто, просто летим вперед
            transform.Translate(Vector3.forward * step);
        }
    }
}

