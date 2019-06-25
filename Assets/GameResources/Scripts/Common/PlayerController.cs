using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Класс управления персонажем
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("Назначаем объекты")]
    [SerializeField]
    private Transform transformPointCam = null;

    [SerializeField]
    private bl_Joystick joystickMove;

    [SerializeField]
    private bl_Joystick joystickLook;

    [Header("Назначаем праметры")]
    [SerializeField]
    private float deadZone = 0.1f;

    [SerializeField]
    private float speedPosition = 1f;

    [SerializeField]
    private float speedRotation = 1f;
    
    [SerializeField]
    private Vector2 rotationLimit = new Vector2(-40f, 60f);

    [SerializeField]
    private bool isInversionRotation = true;

    private Rigidbody rigidbody = null;

    private Vector3 translate = Vector3.zero;
    private Vector3 rotation = Vector3.zero;

    private float horizontalKeyboard = 0f;
    private float verticalKeyboard = 0f;
    private float horizontalMouse = 0f;
    private float verticalMouse = 0f;

    private Vector2 startMousePosition = Vector2.zero;

    private Vector3 vectorCamRotation = Vector3.zero;

    private bool isDead = false;

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
        EnemyController.onAttack += OnAttack;
        AnimEvents.onAttack += OnAttackSound;
    }

    /// <summary>Отписки</summary>
    private void UnSubscribe()
    {
        EnemyController.onAttack -= OnAttack;
        AnimEvents.onAttack -= OnAttackSound;
    }

    /// <summary>
    /// Обработчик события атаки
    /// </summary>
    private void OnAttack(Vector3 enemyPosition)
    {
        transformPointCam.LookAt(enemyPosition + Vector3.up * 0.5f);
        isDead = true;
    }

    /// <summary>
    /// Обработчик события анимации атаки
    /// </summary>
    private void OnAttackSound()
    {
        SoundPlayer.Instance.PlayDead();
    }
    #endregion Subscribes / UnSubscribes 

    private void Awake()
    {
        Init();
        ApplyStartParametrs();        
    }

    /// <summary>
    /// Инициализация
    /// </summary>
    private void Init ()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void ApplyStartParametrs()
    {
        vectorCamRotation = transformPointCam.localEulerAngles;
        //В редакторе вращается быстрее чем на девайсе
        
//#if UNITY_EDITOR
       // speedRotation *= 5f;
       // speedPosition *= 5f;
//#endif
    }

    private void FixedUpdate()
    {
        if (!isDead)
        {
            EditorInput();
            JoysticksInput();
        }
    }

    /// <summary>
    /// Перемещение персонажа
    /// </summary>
    private void Move (float positionX, float positionZ)
    {  
        if (positionX != 0 || positionZ != 0)
        {
            translate.x = positionX * Time.deltaTime * speedPosition;
            translate.z = positionZ * Time.deltaTime * speedPosition;
            rigidbody.MovePosition(transform.position + transform.TransformDirection(translate));
            //rigidbody.AddForce(translate * speedPosition);
            //transform.Translate(translate);
        }
    }

    /// <summary>
    /// Поворот персонажа
    /// </summary>
    private void Rotation (float rotationX, float rotationY)
    {
        if (rotationY != 0)
        {
            //transform.Rotate(0f, rotationY * speedRotation, 0f);

            rotation.y = rotationY;
            Quaternion deltaRotation = Quaternion.Euler(rotation * Time.deltaTime * speedRotation * 10);
            rigidbody.MoveRotation(rigidbody.rotation * deltaRotation);
        }

        if (rotationX != 0)
        {
            vectorCamRotation.x += rotationX * speedRotation * (isInversionRotation == true ? 1 : -1);
            vectorCamRotation.x = Mathf.Clamp(vectorCamRotation.x, rotationLimit.x, rotationLimit.y);
            transformPointCam.localEulerAngles = vectorCamRotation;
        }
    }

    /// <summary>
    /// Получаем значение с джостика с учётом мёртвой зоны
    /// </summary>
    private float FindDeltaPositionJoystick(float positionJoystick)
    {
        if (Mathf.Abs(positionJoystick) > deadZone)
        {
            return positionJoystick;
        }
        else
        {
            return 0;
        }
    }

    /// <summary>
    /// Ввод в редакторе для тестов
    /// </summary>
    private void EditorInput()
    {
#if UNITY_EDITOR
        KeyboardInput();
        MouseInput();
#endif
    }

    /// <summary>
    /// Ввод с клавы
    /// </summary>
    private void KeyboardInput ()
    {
        if (Input.GetKey(KeyCode.W) ||
            Input.GetKey(KeyCode.S) ||
            Input.GetKey(KeyCode.A) ||
            Input.GetKey(KeyCode.D))
        {
            if (Input.GetKey(KeyCode.W))
            {
                verticalKeyboard += Time.deltaTime * 5;
            }

            if (Input.GetKey(KeyCode.S))
            {
                verticalKeyboard -= Time.deltaTime * 5;
            }

            if (Input.GetKey(KeyCode.A))
            {
                horizontalKeyboard -= Time.deltaTime * 5;
            }

            if (Input.GetKey(KeyCode.D))
            {
                horizontalKeyboard += Time.deltaTime * 5;
            }

            horizontalKeyboard = Mathf.Clamp(horizontalKeyboard, -5, 5);
            verticalKeyboard = Mathf.Clamp(verticalKeyboard, -5, 5);

            Move(horizontalKeyboard, verticalKeyboard);
        }
        else
        {
            horizontalKeyboard = 0f;
            verticalKeyboard = 0f;
        }
    }

    /// <summary>
    /// Ввод мыши
    /// </summary>
    private void MouseInput ()
    {
        if (Input.GetMouseButton(1))
        {
            if (startMousePosition == Vector2.zero)
            {
                startMousePosition = Input.mousePosition;
            }

            float _horizontalMouse = FindDeltaPositionJoystick((Input.mousePosition.x - startMousePosition.x) * 0.1f);
            float _verticalMouse = FindDeltaPositionJoystick((Input.mousePosition.y - startMousePosition.y) * 0.1f);

            _verticalMouse = Mathf.Clamp(_verticalMouse, -5, 5);
            _horizontalMouse = Mathf.Clamp(_horizontalMouse, -5, 5);
            
            Rotation (_verticalMouse, _horizontalMouse);
        }
        else
        {
            startMousePosition = Vector2.zero;
        }
    }

    /// <summary>
    /// Ввод на девайсе
    /// </summary>
    private void JoysticksInput ()
    {
        Move(FindDeltaPositionJoystick(joystickMove.Horizontal),
            FindDeltaPositionJoystick(joystickMove.Vertical));

        Rotation(FindDeltaPositionJoystick(joystickLook.Vertical),
            FindDeltaPositionJoystick(joystickLook.Horizontal));
    }
}
