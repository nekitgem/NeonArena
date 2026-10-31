using UnityEngine;

public class EnemyDrone : MonoBehaviour
{
    public float speed = 5f;

    [Header("Настройки Стрельбы")]
    public GameObject bulletPrefab;      // Префаб красной пули EnemyBullet
    public Transform firePoint;          // Точка EnemyFirePoint на носу дрона
    public float fireRate = 2f;          // Раз во сколько секунд стрелять
    public float shootingDistance = 12f; // С какого расстояния дрон начнет стрелять

    private Transform player;
    private Rigidbody rb;
    private float stunTimer = 0f;
    private float nextFireTime = 0f;

    public GameObject corePrefab;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
    }

    void FixedUpdate()
    {
        if (stunTimer > 0f)
        {
            stunTimer -= Time.fixedDeltaTime;
            return;
        }

        if (player != null && rb != null)
        {
            
            Vector3 targetPosition = new Vector3(player.position.x, transform.position.y, player.position.z);
            float distanceToPlayer = Vector3.Distance(transform.position, targetPosition);

            // Дрон всегда смотрит на игрока
            transform.LookAt(targetPosition);

            // Если дрон еще далеко — он летит к игроку
            if (distanceToPlayer > shootingDistance - 2f)
            {
                Vector3 direction = (targetPosition - transform.position).normalized;
                rb.linearVelocity = new Vector3(direction.x * speed, rb.linearVelocity.y, direction.z * speed);
            }
            else
            {
                // Если подошел на дистанцию атаки — останавливается и держит позицию
                rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            }

            // Логика стрельбы: если игрок в зоне досягаемости и таймер перезарядки прошел
            if (distanceToPlayer <= shootingDistance && Time.time >= nextFireTime)
            {
                ShootAtPlayer();
                nextFireTime = Time.time + fireRate;
            }
        }
    }

    void ShootAtPlayer()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            // Спавним пулю на носу дрона с его текущим поворотом в сторону игрока
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        }
    }

    public void Stun(float duration)
    {
        stunTimer = duration;
    }

    public void TakeDamage()
    {
        Debug.Log("один 200");

        // ПРОВЕРЬТЕ ЭТОТ БЛОК: именно он отвечает за появление кристаллов!
        if (corePrefab != null)
        {
            Instantiate(corePrefab, transform.position, Quaternion.identity);
        }
        

        Destroy(gameObject);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Floor"))
        {
            Debug.Log("дрон выпал");
            
            Destroy(gameObject);
        }
    }
}
