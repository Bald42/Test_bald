using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Управление интерфесом меню
/// </summary>
public class InterfaceMenu : MonoBehaviour
{
    public delegate void ActiveSmokeEventHandler (bool isActive);
    public static event ActiveSmokeEventHandler onActiveSmoke = delegate { };

    public delegate void EventHandler ();
    public static event EventHandler onMoveEnemy = delegate { };
    public static event EventHandler onChangeMusic = delegate { };
    public static event EventHandler onChangeSound = delegate { };

    [SerializeField]
    private Transform settingsPanel = null;

    [SerializeField]
    private Transform playButton = null;

    [SerializeField]
    private Toggle toggleSound = null;

    [SerializeField]
    private Toggle toggleMusic = null;

    private Coroutine activeSettingsPanel = null;

    private bool isStartSoundClick = false;

    private Image imagePlayButton = null;

    private void Start()
    {
        ApplyStartParametrs();
    }

    /// <summary>
    /// Применяем стартовые параметры
    /// </summary>
    private void ApplyStartParametrs ()
    {
        imagePlayButton = playButton.GetComponent<Image>();
        onActiveSmoke(false);
        onMoveEnemy();
        toggleSound.isOn = PlayerPrefs.GetInt("Sound", 1) == 1 ? true : false;
        toggleMusic.isOn = PlayerPrefs.GetInt("Music", 1) == 1 ? true : false;
        isStartSoundClick = true;
    }

    /// <summary>
    /// зывываем Открытие/закрытие сеттингc панель
    /// </summary>
    public void OnActiveSettingsPanel (bool isActive)
    {
        if (activeSettingsPanel != null)
        {
            StopCoroutine(activeSettingsPanel);
        }
        activeSettingsPanel = StartCoroutine(ActiveSettingsPanel(isActive));
        SoundPlayer.Instance.PlayClick();
    }

    /// <summary>
    /// Открываем/закрываем сеттингc панель
    /// </summary>
    private IEnumerator ActiveSettingsPanel (bool isActive)
    {
        Vector3 newScale = isActive ? Vector3.one : Vector3.zero;
        while (settingsPanel.localScale != newScale)
        {
            settingsPanel.localScale = Vector3.Lerp(settingsPanel.localScale, newScale, Time.fixedDeltaTime * 10f);
            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        }
    }

    /// <summary>
    /// Кнопка плей
    /// </summary>
    public void OnPlayButton()
    {
        StartCoroutine(Play());
        SoundPlayer.Instance.PlayClick();
    }

    /// <summary>
    /// Кнопка выхода
    /// </summary>
    public void OnExitButton()
    {
        StartCoroutine(DelayExit());
        SoundPlayer.Instance.PlayClick();
    }

    /// <summary>
    /// Кнопка политики
    /// </summary>
    public void OnPrivacy ()
    {
        Debug.LogError("Open privacy");
        SoundPlayer.Instance.PlayClick();
    }

    /// <summary>
    /// Включаем/отключаем звук
    /// </summary>
    public void OnToggleSound()
    {
        if (!toggleSound.isOn)
        {
            PlayerPrefs.SetInt("Sound", 0);
        }
        else
        {
            PlayerPrefs.SetInt("Sound", 1);
        }
        PlayerPrefs.Save();
        onChangeSound();

        if (isStartSoundClick)
        {
            SoundPlayer.Instance.PlayClick();
        }
    }

    /// <summary>
    /// Включаем/отключаем музыку
    /// </summary>
    public void OnToggleMusic ()
    {
        if (!toggleMusic.isOn)
        {
            PlayerPrefs.SetInt("Music", 0);
        }
        else
        {
            PlayerPrefs.SetInt("Music", 1);
        }
        PlayerPrefs.Save();
        onChangeMusic();
        if (isStartSoundClick)
        {
            SoundPlayer.Instance.PlayClick();
        }
    }

    /// <summary>
    /// Запускаем игру
    /// </summary>
    private IEnumerator Play()
    {
        onActiveSmoke(true);
        StartCoroutine(AnimPlay());
        yield return new WaitForSecondsRealtime(3f);
        SceneManager.LoadScene("Game");
    }

    /// <summary>
    /// Анимация кнопки плей
    /// </summary>
    private IEnumerator AnimPlay ()
    {
        Color colorPlayButton = imagePlayButton.color;
        while (true)
        {
            Vector3 newPosition = playButton.localPosition;
            newPosition.y -= Time.fixedDeltaTime * 50;
            colorPlayButton.a -= Time.fixedDeltaTime;
            playButton.Rotate(new Vector3(0, 0, -2));
            playButton.localPosition = newPosition;
            imagePlayButton.color = colorPlayButton;
            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        }
    }

    /// <summary>
    /// Задержка выхода
    /// </summary>
    private IEnumerator DelayExit()
    {
        onActiveSmoke(true);
        yield return new WaitForSecondsRealtime(3f);
        Application.Quit();
    }
}
