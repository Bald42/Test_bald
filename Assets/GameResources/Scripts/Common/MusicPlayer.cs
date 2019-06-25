using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Класс отвечающий за музыку
/// </summary>
public class MusicPlayer : MonoBehaviour
{
    public static MusicPlayer Instance = null;

    [Range(0f, 1f)]
    [SerializeField]
    private float volume = 1f;

    private AudioSource audioSource = null;
        
    #region Subscribes / UnSubscribes
    private void OnEnable()
    {
        Subscribe();
    }

    private void OnDisable()
    {
        UnSubscribe();
    }

    /// <summary>Подписки</summary>
    private void Subscribe()
    {
        InterfaceMenu.onChangeMusic += OnChangeMusic;
    }

    /// <summary>Отписки</summary>
    private void UnSubscribe()
    {
        InterfaceMenu.onChangeMusic -= OnChangeMusic;
    }

    /// <summary>
    /// Обработчик события вкл/выкл музыки
    /// </summary>
    private void OnChangeMusic ()
    {
        if (PlayerPrefs.GetInt("Music") == 1)
        {
            audioSource.volume = volume;
        }
        else
        {
            audioSource.volume = 0;
        }
    }
    #endregion Subscribes / UnSubscribes 

    private void Awake()
    {
        Init();
    }

    /// <summary>
    /// Инициализация
    /// </summary>
    private void Init()
    {
        if (!Instance)
        {
            Instance = this;
            audioSource = GetComponent<AudioSource>();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ActiveMusic();
    }

    /// <summary>
    /// Включаем/ отключаем музыку
    /// </summary>
    private void ActiveMusic()
    {
        audioSource.Play();
        if (PlayerPrefs.GetInt("Music") == 1)
        {
            audioSource.volume = volume;
        }
        else
        {
            audioSource.volume = 0;
        }
    }
}
