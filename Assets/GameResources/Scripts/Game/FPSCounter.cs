using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Счётчик фпс
/// </summary>
public class FPSCounter : MonoBehaviour
{
    private Text textFPS = null;
    private int fpsCounter = 0;

    private void Awake()
    {
        textFPS = GetComponent<Text>();
        if (textFPS)
        {
            StartCoroutine(ViewFps());
        }
        else
        {
            Destroy(this);
        }
    }


    private IEnumerator ViewFps()
    {
        while (true)
        {
            textFPS.text = fpsCounter.ToString();
            fpsCounter = 0;
            yield return new WaitForSecondsRealtime(1f);
        }
    }

    private void Update()
    {
        fpsCounter++;
    }
}
