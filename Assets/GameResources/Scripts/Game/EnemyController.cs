using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Класс управления противником
/// </summary>
public class EnemyController : MonoBehaviour
{
    public delegate void AttackEventHandler (Vector3 emenyPosition);
    public static event AttackEventHandler onAttack = delegate { };

    private enum EnemyState
    {
        Idle,
        Walk,
        Run,
        Found,
        Attack,
        Searсh,
        LookAtBuild
    }

    [SerializeField]
    private EnemyState m_state;

    [Header("Параметры поиска")]
    [SerializeField]
    private int rays = 6;

    [SerializeField]
    private float angle = 20;

    [SerializeField]
    private float offset = 0.5f;

    [SerializeField]
    private float searchFrequency = 0.1f;

    [SerializeField]
    private Vector3 rotateVector = Vector3.zero;

    [SerializeField]
    private Vector2 randomTimeLookAtBuild = Vector2.zero;

    private Coroutine coroutineSearchPlayer = null;
    private Coroutine coroutineColorChange = null;

    private Transform target = null;

    [Header("Инидкаторы")]
    [SerializeField]
    private Material materialIndicator = null;

    [SerializeField]
    private Material fadeMaterialIndicator = null; 

    [Header("Параметры аниматора")]
    [SerializeField]
    private Animator animator = null;

    [Header("Параметры перемещения")]
    [SerializeField]
    private NavMeshAgent agent = null;

    [SerializeField]
    private Transform build = null;

    [SerializeField]
    private float speedWalk = 1f;

    [SerializeField]
    private float speedRun = 2f;

    [SerializeField]
    private List<Transform> wayPoints = new List<Transform> ();

    [Header("Звуки")]
    [SerializeField]
    private List<AudioClip> soundsSteps = new List<AudioClip>();

    private AudioSource audioSource = null;

    private bool isSoundStep = false;

    private Vector3 nextPoint = Vector3.zero;

    private int numberNextWayPoint = 0;

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
        InterfaceMenu.onMoveEnemy += OnMoveEnemy;
        AnimEvents.onStep += OnStep;
        InterfaceMenu.onChangeSound += OnChangeSound;
    }

    /// <summary>Отписки</summary>
    private void UnSubscribe()
    {
        CutSceneController.onStopCutScene -= OnStopCutScene;
        InterfaceMenu.onMoveEnemy -= OnMoveEnemy;
        AnimEvents.onStep -= OnStep;
        InterfaceMenu.onChangeSound -= OnChangeSound;
    }

    /// <summary>
    /// Обработчик события конца катсцены
    /// </summary>
    private void OnStopCutScene()
    {
        MyStart();
    }

    /// <summary>
    /// Обработчик события старта в меню
    /// </summary>
    private void OnMoveEnemy()
    {
        MyStart();
    }

    /// <summary>
    /// Обработчик события анимации шагов
    /// </summary>
    private void OnStep()
    {
        if (isSoundStep)
        {
            int rndSound = Random.Range(0, soundsSteps.Count-1);
            audioSource.PlayOneShot(soundsSteps[rndSound]);
        }
    }

    /// <summary>
    /// Обработчик события включения отключения звука
    /// </summary>
    private void OnChangeSound()
    {
        if (PlayerPrefs.GetInt("Sound") == 1)
        {
            isSoundStep = true;
        }
        else
        {
            isSoundStep = false;
        }
    }
    #endregion Subscribes / UnSubscribes 

    #region CalculatedParameters
    /// <summary>
    /// Смена состояния
    /// </summary>
    private EnemyState state
    {
        get { return m_state; }

        set
        {
            m_state = value;

            switch (m_state)
            {
                case EnemyState.Walk:
                    {                        
                        animator.SetTrigger("walk");
                        MoveToPoint(wayPoints[numberNextWayPoint].position);
                        agent.speed = speedWalk;
                        StartColorCoroutine(Color.blue);
                        break;
                    }
                case EnemyState.Run:
                    {
                        animator.SetTrigger("run");
                        MoveToPoint(target.position);
                        agent.speed = speedRun;
                        StartColorCoroutine(Color.red);
                        break;
                    }
                case EnemyState.Found:
                    {
                        animator.SetTrigger("found");
                        MoveToPoint(transform.position);
                        StartCoroutine(DelayRun());
                        SoundPlayer.Instance.PlayCry();
                        break;
                    }
                case EnemyState.Attack:
                    {
                        onAttack(transform.position);
                        animator.SetTrigger("attack");
                        MoveToPoint(transform.position);
                        transform.LookAt(target.position);
                        break;
                    }
                case EnemyState.Searсh:
                    {
                        animator.SetTrigger("search");
                        MoveToPoint(transform.position);
                        coroutineSearchPlayer = StartCoroutine(SearchPlayer());
                        StartColorCoroutine(Color.yellow);
                        break;
                    }
                case EnemyState.LookAtBuild:
                    {
                        animator.SetTrigger("search");
                        MoveToPoint(transform.position);
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }
    }

    /// <summary>
    /// Запускаем корутину смены цвета
    /// </summary>
    private void StartColorCoroutine (Color newColor)
    {
        if (coroutineColorChange != null)
        {
            StopCoroutine(coroutineColorChange);
        }
        coroutineColorChange = StartCoroutine(ColorChange(newColor));
    }

    /// <summary>
    /// Пускаем и отрисовываем луч
    /// </summary>
    private bool GetRaycast(Vector3 dir)
    {
        bool result = false;
        RaycastHit hit = new RaycastHit();
        Vector3 pos = transform.position;
        float _offset = offset;
        if (target)
        {
            _offset += target.position.y;
        }
        pos.y += _offset;

        if (Physics.Raycast(pos, dir, out hit))
        {
            if (hit.transform == target)
            {
                result = true;
                Debug.DrawLine(pos, hit.point, Color.green);
            }
            else
            {
                Debug.DrawLine(pos, hit.point, Color.blue);
            }
        }
        else
        {
            Debug.DrawRay(pos, dir, Color.red);
        }
        return result;
    }

    /// <summary>
    /// Пускаем лучи для нахождения цели
    /// </summary>
    private bool RayToScan()
    {
        bool result = false;
        bool a = false;
        bool b = false;
        float j = 0;

        for (int i = 0; i < rays; i++)
        {
            var x = Mathf.Sin(j);
            var y = Mathf.Cos(j);

            j += angle * Mathf.Deg2Rad / rays;

            Vector3 dir = transform.TransformDirection(new Vector3(x, 0, y));
            if (GetRaycast(dir))
            {
                a = true;
            }

            if (x != 0)
            {
                dir = transform.TransformDirection(new Vector3(-x, 0, y));
                if (GetRaycast(dir))
                {
                    b = true;
                }
            }
        }

        if (a || b)
        {
            result = true;
        }
        return result;
    }
    #endregion CalculatedParameters

    #region Coroutines
    /// <summary>
    /// Поиск игрока
    /// </summary>
    private IEnumerator FindPlayer ()
    {
        while (true)
        {
            switch (state)
            {
                case EnemyState.Walk:
                    {
                        if (RayToScan())
                        {
                            state = EnemyState.Found;
                        }
                        break;
                    }
                case EnemyState.Searсh:
                    {
                        if (RayToScan())
                        {
                            state = EnemyState.Run;
                            if (coroutineSearchPlayer != null)
                            {
                                StopCoroutine(coroutineSearchPlayer);
                            }
                        }
                        break;
                    }
                case EnemyState.LookAtBuild:
                    {
                        if (RayToScan())
                        {
                            state = EnemyState.Run;
                        }
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
            yield return new WaitForSecondsRealtime (searchFrequency);
        }
    }

    /// <summary>
    /// Проверяем дошли ли мы до текущей точки
    /// </summary>
    private IEnumerator FindNextPoint()
    {
        float distance = 0f;
        while (true)
        {
            switch (state)
            {
                case EnemyState.Walk:
                    {
                        distance = (transform.position - nextPoint).magnitude;
                        if (distance <= 0.5f)
                        {
                            numberNextWayPoint++;
                            if (numberNextWayPoint == wayPoints.Count)
                            {
                                numberNextWayPoint = 0;
                            }
                            MoveToPoint(wayPoints[numberNextWayPoint].position);
                        }
                        break;
                    }
                case EnemyState.Run:
                    {
                        distance = (transform.position - nextPoint).magnitude;
                        if (distance <= 1f)
                        {
                            if ((transform.position - target.position).magnitude < 1f)
                            {
                                state = EnemyState.Attack;
                            }
                            else
                            {
                                state = EnemyState.Searсh;
                            }
                        }
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
            yield return new WaitForSecondsRealtime(searchFrequency);
        }   
    }

    /// <summary>
    /// Задержка перед бегом для анимации ярости
    /// </summary>
    private IEnumerator DelayRun()
    {
        yield return new WaitForSeconds (1f);
        state = EnemyState.Run;
    }

    /// <summary>
    /// Поиск игрока с поворотом
    /// </summary>
    private IEnumerator SearchPlayer()
    {
        float _timeStartSearch = Time.timeSinceLevelLoad;
        while (Time.timeSinceLevelLoad - _timeStartSearch < 5f)
        {
            transform.Rotate(rotateVector);
            yield return new WaitForSeconds(Time.deltaTime);
        }

        if (state == EnemyState.Searсh)
        {
            state = EnemyState.Walk;
        }
    }

    /// <summary>
    /// Поворачиваемся к зданию
    /// </summary>
    private IEnumerator LookAtBuild ()
    {
        float randomTimeLook = Random.Range(randomTimeLookAtBuild.x, randomTimeLookAtBuild.y);

        yield return new WaitForSeconds(randomTimeLook);

        if (state == EnemyState.Walk)
        {
            state = EnemyState.LookAtBuild;
        }

        if (state == EnemyState.LookAtBuild)
        {
            float _timeLookAtBuild = Time.timeSinceLevelLoad;
            Vector3 direction = (build.transform.position - transform.position).normalized;
            direction.y = 0f;

            while (Time.timeSinceLevelLoad - _timeLookAtBuild < 3f)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(direction), 2f);
                yield return new WaitForSeconds(Time.deltaTime);
            }
            state = EnemyState.Walk;
        }

        StartCoroutine(LookAtBuild());
    }

    /// <summary>
    /// Изменение цвета
    /// </summary>
    private IEnumerator ColorChange (Color newColor)
    {
        Color fadeColor = newColor;
        while (materialIndicator.color != newColor)
        {
            materialIndicator.color = Color.Lerp(materialIndicator.color, newColor, Time.deltaTime * 2);
            fadeColor = materialIndicator.color;
            fadeColor.a = 0.3f;
            fadeMaterialIndicator.color = fadeColor;
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }
    #endregion Coroutines

    private void Awake()
    {
        Init();
    }

    /// <summary>
    /// Инициализация
    /// </summary>
    private void Init ()
    {
        if (GameObject.FindGameObjectWithTag("Player"))
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }
        audioSource = GetComponent<AudioSource>();

        if (PlayerPrefs.GetInt("Sound") == 1)
        {
            isSoundStep = true;
        }
        
        materialIndicator.color = Color.blue;
        Color fadeColor = materialIndicator.color;
        fadeColor.a = 0.3f;
        fadeMaterialIndicator.color = fadeColor;
    }

    /// <summary>
    /// Применяем стартовые параметры
    /// </summary>
    private void MyStart ()
    {
        state = EnemyState.Walk;
        StartCoroutine(FindPlayer());
        StartCoroutine(FindNextPoint());
        StartCoroutine(LookAtBuild());
    }

    /// <summary>
    /// Указывает точку для перемещения
    /// </summary>
    private void MoveToPoint (Vector3 newPoint)
    {
        nextPoint = newPoint;
        agent.SetDestination(newPoint);
    }
}
