using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    public static SoundPlayer Instance = null;

    [SerializeField]
    private AudioClip clipClick = null;

    [SerializeField]
    private AudioClip clipCry = null;

    [SerializeField]
    private AudioClip clipDead = null;

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
        InterfaceMenu.onChangeSound += OnChangeSound;
    }

    /// <summary>Отписки</summary>
    private void UnSubscribe()
    {
        InterfaceMenu.onChangeSound -= OnChangeSound;
    }

    /// <summary>
    /// Обработчик события вкл/выкл музыки
    /// </summary>
    private void OnChangeSound()
    {
        if (PlayerPrefs.GetInt("Sound") == 1)
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
            audioSource = GetComponent<AudioSource>();
            Instance = this;
            DontDestroyOnLoad(gameObject);
            if (PlayerPrefs.GetInt("Sound") == 1)
            {
                audioSource.volume = volume;
            }
            else
            {
                audioSource.volume = 0;
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Вызываем звук клика
    /// </summary>
    public void PlayClick ()
    {
        if (volume != 0)
        {
            audioSource.PlayOneShot(clipClick);
        }
    }

    /// <summary>
    /// Вызываем звук рёва
    /// </summary>
    public void PlayCry ()
    {
        if (volume != 0)
        {
            audioSource.PlayOneShot(clipCry);
        }
    }

    /// <summary>
    /// Вызываем звук смерти
    /// </summary>
    public void PlayDead()
    {
        if (volume != 0)
        {
            audioSource.PlayOneShot(clipDead);
        }
    }
}