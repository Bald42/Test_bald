using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Перемещение камеры мини карты
/// </summary>
public class MinimapCamera : MonoBehaviour
{
    [SerializeField]
    private Transform camPoint = null;

    [SerializeField]
    private float speed = 1f;

    private Vector3 vectorPorate = Vector3.zero;

    private void LateUpdate()
    {
        Move();
    }

    /// <summary>
    /// Перемещение камеры
    /// </summary>
    private void Move()
    {
        transform.position = Vector3.Lerp(transform.position,
            camPoint.position,
            Time.deltaTime * speed);

        vectorPorate = camPoint.eulerAngles;
        vectorPorate.x = 90f;
        vectorPorate.z = 0f;

        transform.eulerAngles = Vector3.Lerp(transform.eulerAngles,
                vectorPorate,
                Time.deltaTime * speed);
    }
}