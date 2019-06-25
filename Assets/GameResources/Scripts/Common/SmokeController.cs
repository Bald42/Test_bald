using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Контроллер дыма
/// </summary>
public class SmokeController : MonoBehaviour
{
    public static SmokeController Instance = null;

    [SerializeField]
    private ParticleSystem particleSystemSmoke = null;

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
        InterfaceMenu.onActiveSmoke += OnActiveSmoke;
        CutSceneController.onActiveSmoke += OnActiveSmoke;
        InterfaceGame.onActiveSmoke += OnActiveSmoke;
    }

    /// <summary>Отписки</summary>
    private void UnSubscribe()
    {
        InterfaceMenu.onActiveSmoke -= OnActiveSmoke;
        CutSceneController.onActiveSmoke -= OnActiveSmoke;
        InterfaceGame.onActiveSmoke -= OnActiveSmoke;
    }

    /// <summary>
    /// Обрабоьчик события активации дыма
    /// </summary>
    private void OnActiveSmoke(bool isActive)
    {
        if (isActive)
        {
            particleSystemSmoke.Play();
        }
        else
        {
            particleSystemSmoke.Stop();
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
    private void Init ()
    {
        if (!Instance)
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
