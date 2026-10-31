using UnityEngine;
using UnityEngine.InputSystem;

public class ShockwaveAbility : MonoBehaviour
{
    public float waveRadius = 10f;       // Радиус действия волны
    public float explosionForce = 150f; // Сила отталкивания врагов
    public float cooldown = 3f;          // Перезарядка способности в секундах

    private float nextReadyTime = 0f;

    public GameObject shockwavePrefab;

    void Update()
    {
        // Проверяем нажатие Пробела через New Input System
        bool isSpacePressed = Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame;

        if (isSpacePressed && Time.time >= nextReadyTime)
        {
            TriggerShockwave();
            nextReadyTime = Time.time + cooldown; // Запуск перезарядки
        }
    }

    void TriggerShockwave()
    {
        Debug.Log("ЭМИ Волна активирована!");

        // Находим все коллайдеры в определенном радиусе вокруг игрока
        Collider[] colliders = Physics.OverlapSphere(transform.position, waveRadius);

        if (shockwavePrefab != null)
        {
            Instantiate(shockwavePrefab, transform.position, Quaternion.identity);
        }

        foreach (Collider hit in colliders)
        {
            // Проверяем, является ли объект врагом
            if (hit.CompareTag("Enemy"))
            {
                // Ищем Rigidbody на самом враге или в его родительских объектах
                Rigidbody enemyRb = hit.GetComponentInParent<Rigidbody>();

                if (enemyRb != null)
                {
                    // Применяем силу физического взрыва
                    // transform.position — эпицентр (центр игрока)
                    // 0f — смещение по высоте (взрыв плоский, на уровне пола)
                    // ForceMode.Impulse — мгновенный сильный толчок
                    enemyRb.AddExplosionForce(explosionForce, transform.position, waveRadius, 0f, ForceMode.Impulse);
                    hit.GetComponentInParent<EnemyDrone>()?.Stun(0.5f);

                    Debug.Log($"Отброшен дрон: {hit.name}");
                }
            }
        }
    }

    // Визуальное отображение радиуса волны в редакторе Unity (для удобства настройки)
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, waveRadius);
    }


}
