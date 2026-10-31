using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [Header("Плейлист фоновой музыки")]
    public AudioClip[] musicPlaylist; // Массив для ваших песен

    private AudioSource audioSource;
    private int currentTrackIndex = 0;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (musicPlaylist != null && musicPlaylist.Length > 0)
        {
            // Перемешиваем плейлист при старте (по желанию)
            ShufflePlaylist();

            // Запускаем первый трек
            PlayTrack(currentTrackIndex);
        }
        else
        {
            Debug.LogWarning("SoundManager: Плейлист пуст! Добавьте аудиофайлы в инспекторе.");
        }
    }

    void Update()
    {
        // Проверяем: если музыка включена, но текущий трек доиграл до конца
        if (audioSource != null && !audioSource.isPlaying && musicPlaylist.Length > 0)
        {
            PlayNextTrack();
        }
    }

    void PlayTrack(int index)
    {
        audioSource.clip = musicPlaylist[index];
        audioSource.Play();
        Debug.Log($"Сейчас играет трек {index + 1}: {musicPlaylist[index].name}");
    }

    void PlayNextTrack()
    {
        // Переходим к следующему индексу, а если плейлист кончился — возвращаемся к 0 (зацикливание)
        currentTrackIndex = (currentTrackIndex + 1) % musicPlaylist.Length;
        PlayTrack(currentTrackIndex);
    }

    void ShufflePlaylist()
    {
        // Простой алгоритм случайной тасовки массива
        for (int i = 0; i < musicPlaylist.Length; i++)
        {
            AudioClip temp = musicPlaylist[i];
            int randomIndex = Random.Range(i, musicPlaylist.Length);
            musicPlaylist[i] = musicPlaylist[randomIndex];
            musicPlaylist[randomIndex] = temp;
        }
    }
}
