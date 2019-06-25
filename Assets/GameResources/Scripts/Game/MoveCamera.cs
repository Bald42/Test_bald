using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Скрипт перемещения камеры
/// </summary>
public class MoveCamera : MonoBehaviour
{
    [SerializeField]
    private Transform camPoint = null;

    [SerializeField]
    private Animation anim = null;

    [SerializeField]
    private float speed = 1f;

    enum StateCam
    {
        StartCutScene,
        MoveToPlayer
    }

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
        CutSceneController.onStartCutScene += OnStartCutScene;
        CutSceneController.onStopCutScene += OnStopCutScene;
    }

    /// <summary>Отписки</summary>
    private void UnSubscribe()
    {
        CutSceneController.onStartCutScene -= OnStartCutScene;
        CutSceneController.onStopCutScene -= OnStopCutScene;
    }

    /// <summary>
    /// Обработчик события начала катсцены
    /// </summary>
    private void OnStartCutScene ()
    {
        anim.Play();
    }

    /// <summary>
    /// Обработчик события конца катсцены
    /// </summary>
    private void OnStopCutScene ()
    {
        anim.enabled = false;
        stateCam = StateCam.MoveToPlayer;
    }
    #endregion Subscribes / UnSubscribes 

    [SerializeField]
    private StateCam stateCam = StateCam.MoveToPlayer;

    private void LateUpdate()
    {
        Move();
    }

    /// <summary>
    /// Перемещение камеры
    /// </summary>
    private void Move()
    {
        if (stateCam == StateCam.MoveToPlayer)
        {
            transform.position = Vector3.Lerp(transform.position,
                camPoint.position,
                Time.deltaTime * speed);

            transform.rotation = Quaternion.Lerp(transform.rotation,
                camPoint.rotation,
                Time.deltaTime * speed);
        }
    }    
}