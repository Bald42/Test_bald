using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutSceneController : MonoBehaviour
{
    public delegate void EventHandler();
    public static event EventHandler onStopFog = delegate { };
    public static event EventHandler onStartCutScene = delegate { };
    public static event EventHandler onStopCutScene = delegate { };

    public delegate void ActiveSmokeEventHandler(bool isActive);
    public static event ActiveSmokeEventHandler onActiveSmoke = delegate { };

    private bool isSkip = false;

    private void Awake()
    {
        StartCoroutine(CutScene());
    }

    /// <summary>
    /// Катсцена
    /// </summary>
    private IEnumerator CutScene()
    {
        onActiveSmoke(false);
        yield return new WaitForSeconds(3f);
        onStartCutScene();
        yield return new WaitForSeconds(6f);
        onStopCutScene();
        isSkip = true;
    }

    private void Update()
    {
        if (!isSkip && Input.GetMouseButton(0))
        {
            StopAllCoroutines();
            onStopCutScene();
            isSkip = true;
        }
    }
}
