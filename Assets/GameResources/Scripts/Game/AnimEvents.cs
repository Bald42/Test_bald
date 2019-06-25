using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// События анимаций
/// </summary>
public class AnimEvents : MonoBehaviour
{
    public delegate void EventHandler ();
    public static event EventHandler onAttack = delegate { };
    public static event EventHandler onStep = delegate { };

    /// <summary>
    /// Событие шага
    /// </summary>
    public void OnStep()
    {
        onStep();
    }

    /// <summary>
    /// Событие атаки
    /// </summary>
    public void OnAttack ()
    {
        onAttack();
    }
}
