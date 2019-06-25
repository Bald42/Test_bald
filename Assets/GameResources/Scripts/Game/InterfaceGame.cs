using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Класс отвечающий за интерфейс в игре
/// </summary>
public class InterfaceGame : MonoBehaviour
{
    public delegate void ActiveSmokeEventHandler(bool isActive);
    public static event ActiveSmokeEventHandler onActiveSmoke = delegate { };

    [SerializeField]
    private Transform gameInterface = null;

    [SerializeField]
    private Transform losePanel = null;

    private Coroutine coroutineLose = null;

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
        CutSceneController.onStopCutScene += OnStopCutScene;
        EnemyController.onAttack += OnAttack;
    }

    /// <summary>Отписки</summary>
    private void UnSubscribe()
    {
        CutSceneController.onStopCutScene -= OnStopCutScene;
        EnemyController.onAttack -= OnAttack;
    }

    /// <summary>
    /// Обработчик события конца катсцены
    /// </summary>
    private void OnStopCutScene()
    {
        StartCoroutine(ScaleInterface(gameInterface, Vector3.one));
    }

    /// <summary>
    /// Обработчик атаки
    /// </summary>
    private void OnAttack (Vector3 empty)
    {
        StartCoroutine(DelayLose());
    }
    #endregion Subscribes / UnSubscribes 
    
    private void Start()
    {
        Init();
    }

    /// <summary>
    /// Инициализация
    /// </summary>
    private void Init ()
    {
        gameInterface.localScale = Vector3.zero;
        losePanel.localScale = Vector3.zero;
    }

    /// <summary>
    /// Задержка проигрыша
    /// </summary>
    /// <returns></returns>
    private IEnumerator DelayLose()
    {
        yield return new WaitForSecondsRealtime(1f);
        StartCoroutine(ScaleInterface(gameInterface, Vector3.zero));
        yield return new WaitForSecondsRealtime(1f);
        coroutineLose = StartCoroutine(ScaleInterface(losePanel, Vector3.one));
    }

    /// <summary>
    /// Карутина появления/исчезновения интерфейса
    /// </summary>
    private IEnumerator ScaleInterface(Transform panel, Vector3 newScale)
    {
        while (panel.localScale != newScale)
        {
            panel.localScale = Vector3.Lerp(panel.localScale, newScale, Time.fixedDeltaTime * 3);
            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        }
    }

    /// <summary>
    /// Выход в меню
    /// </summary>
    public void OnMenu ()
    {
        SoundPlayer.Instance.PlayClick();
        StartCoroutine(DelayExit("Menu"));
    }

    /// <summary>
    /// Рестарт
    /// </summary>
    public void OnRestart()
    {
        SoundPlayer.Instance.PlayClick();
        StartCoroutine(DelayExit("Game"));
    }

    /// <summary>
    /// Задержка выхода
    /// </summary>
    /// <returns></returns>
    private IEnumerator DelayExit (string scene)
    {
        onActiveSmoke(true);
        yield return new WaitForSecondsRealtime(1f);
        if (coroutineLose != null)
        {
            StopCoroutine(coroutineLose);
        }
        coroutineLose = StartCoroutine(ScaleInterface(losePanel, Vector3.zero));
        yield return new WaitForSecondsRealtime(3f);
        SceneManager.LoadScene(scene);
    }
}
