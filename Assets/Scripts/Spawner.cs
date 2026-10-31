using System.Collections;
using UnityEngine;
using TMPro;

public class Spawner : MonoBehaviour
{
    [Header("Настройки Префабов")]
    public GameObject dronePrefab;     // Префаб дрона (EnemyPos)
    public TextMeshProUGUI waveText;   // Сюда перетаскиваем WaveNoticeText

    [Header("Настройки Волн")]
    public int minEnemiesInWave = 3;   // Минимальное кол-во врагов на 1-й волне
    public int maxEnemiesInWave = 7;   // Максимальное кол-во врагов на 1-й волне
    public float difficultyScale = 1.2f; // На сколько умножать кол-во врагов с каждой новой волной
    public float timeBetweenSpawns = 1f; // Задержка между появлением отдельных дронов
    public float spawnRadius = 20f;

    private Transform playerTransform;
    private int currentWave = 0;
    private int enemiesToSpawnThisWave;
    private bool isWaveActive = false;

    [Header("Звуковые Эффекты")]
    public AudioClip waveStartSound; // Сюда перетащим звук начала волны
    private AudioSource audioSource;   // Компонент для воспроизведения

    void Start()
    {
        // Добавьте это внутри метода Start():
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }

        if (waveText != null)
        {
            waveText.text = ""; // Очищаем текст при старте
        }

        // Запускаем первую волну
        StartCoroutine(NextWaveRoutine());
    }

    void Update()
    {
        // Проверяем: если волна идет, но все заспавненные враги на карте уничтожены
        if (isWaveActive && GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
        {
            isWaveActive = false;
            StartCoroutine(NextWaveRoutine()); // Запускаем следующую волну
        }
    }

    // Корутина (временной поток) для логики смены волн
    IEnumerator NextWaveRoutine()
    {
        yield return new WaitForSeconds(1f); // Небольшая пауза перед анонсом

        currentWave++;

        // Математика волны: рассчитываем случайное число врагов между MIN и MAX для текущей волны
        // С каждой волной диапазон MIN и MAX плавно увеличивается за счет difficultyScale
        int currentMin = Mathf.RoundToInt(minEnemiesInWave * Mathf.Pow(difficultyScale, currentWave - 1));
        int currentMax = Mathf.RoundToInt(maxEnemiesInWave * Mathf.Pow(difficultyScale, currentWave - 1));
        enemiesToSpawnThisWave = Random.Range(currentMin, currentMax + 1);

        Debug.Log($"Начало волны {currentWave}. Будет спавниться врагов: {enemiesToSpawnThisWave} (Диапазон: {currentMin}-{currentMax})");

        // 1. Показываем сообщение о новой волне
        if (waveText != null)
        {
            waveText.text = $"Horde {currentWave}";
        }

        if (waveStartSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(waveStartSound); // Мгновенно проигрывает звук начала волны
        }

        

        // Ждем 2.5 секунды, пока игрок видит надпись
        yield return new WaitForSeconds(2.5f);

        // Убираем надпись с экрана
        if (waveText != null)
        {
            waveText.text = "";
        }

        // 2. Начинаем спавнить врагов по очереди
        for (int i = 0; i < enemiesToSpawnThisWave; i++)
        {
            SpawnEnemyRandomly();
            yield return new WaitForSeconds(timeBetweenSpawns); // Ждем секунду перед следующим дроном
        }

        // Запоминаем, что спавн окончен и теперь мы просто ждем зачистки карты игроком
        isWaveActive = true;
    }

    void SpawnEnemyRandomly()
    {
        if (playerTransform == null || dronePrefab == null) return;

        float randomAngle = Random.Range(0f, Mathf.PI * 2f);
        float spawnX = playerTransform.position.x + Mathf.Cos(randomAngle) * spawnRadius;
        float spawnZ = playerTransform.position.z + Mathf.Sin(randomAngle) * spawnRadius;

        Vector3 spawnPosition = new Vector3(spawnX, playerTransform.position.y, spawnZ);

        InsthesizeDrone(spawnPosition);
    }

    void InsthesizeDrone(Vector3 position)
    {
        Instantiate(dronePrefab, position, Quaternion.identity);
    }
}
