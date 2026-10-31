using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 0.2f;

    [Header("Настройки Супер-Режима")]
    public int coresNeededForPowerUp = 3;
    public float powerUpDuration = 7f;

    [Header("Интерфейс / UI")]
    public TextMeshProUGUI uiCoreText; // Сюда мы перетащим наш текст из иерархии

    private int currentCores = 0;
    private float nextFireTime = 0f;
    private float powerUpTimer = 0f;
    private bool isPowerUpActive = false;

    [Header("Звуковые Эффекты")]
    public AudioClip waveStartSound; // Сюда перетащим звук начала волны
    private AudioSource audioSource;   // Компонент для воспроизведения

    void Start()
    {
        // Добавьте это внутри метода Start():
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        // Обновляем текст при старте игры
        UpdateUI();
    }

    void Update()
    {
        if (isPowerUpActive)
        {
            powerUpTimer -= Time.deltaTime;

            // Во время супера показываем оставшееся время
            if (uiCoreText != null)
            {
                uiCoreText.text = $"Super: {powerUpTimer:F1} sec";
            }

            if (powerUpTimer <= 0f)
            {
                DeactivatePowerUp();
            }
        }

        bool isMousePressed = Mouse.current != null && Mouse.current.leftButton.isPressed;
        if (isMousePressed && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            if (waveStartSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(waveStartSound); // Мгновенно проигрывает звук начала волны
            }

            GameObject newBullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Bullet bulletScript = newBullet.GetComponent<Bullet>();

            if (bulletScript != null)
            {
                bulletScript.isMirrorModeActive = isPowerUpActive;
                bulletScript.maxBounces = isPowerUpActive ? 10 : 0;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Core"))
        {
            Destroy(other.gameObject);
            currentCores++;

            UpdateUI(); // Обновляем текст на экране при сборе ядра

            if (currentCores >= coresNeededForPowerUp && !isPowerUpActive)
            {
                ActivatePowerUp();
            }
        }
    }

    void ActivatePowerUp()
    {
        isPowerUpActive = true;
        powerUpTimer = powerUpDuration;
        currentCores = 0;
    }

    void DeactivatePowerUp()
    {
        isPowerUpActive = false;
        currentCores = 0;
        UpdateUI(); // Возвращаем обычный текст после окончания супера
    }

    // Вспомогательный метод для обновления надписи
    void UpdateUI()
    {
        if (uiCoreText != null)
        {
            uiCoreText.text = $"Energy: {currentCores} / {coresNeededForPowerUp}";
        }
    }
}

