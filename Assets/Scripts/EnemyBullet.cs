using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float speed = 12f;
    public float lifetime = 4f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // Пуля летит строго вперед по направлению, в котором её заспавнили
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        // Если пуля попала в игрока
        if (other.CompareTag("Player"))
        {
            Debug.Log("💥 Игрок получил урон!");
            // Сюда позже можно добавить уменьшение здоровья игрока
            Destroy(gameObject);
        }
        // Если пуля попала в стену арены
        else if (other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
