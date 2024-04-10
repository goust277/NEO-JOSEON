using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(Rigidbody))]
public abstract class Enemy : MonoBehaviour, IDamageable
{
    protected NavMeshAgent mAgent;

    [Header("체력")]
    [SerializeField] protected float hpMax = 1;
    [SerializeField] protected float hpCurr = 1f;
    private float hitBlink = 0.3f;
    private float hitBlinkCurr = 0f;
    private Material material = null;
    private Color cOrigin;

    [Header("타겟 추적 정보")]
    [SerializeField] private GameObject target = null;
    private bool isChasing = false;
    [SerializeField] private bool lookAtTarget = true;
    private Vector3 lookPos = Vector3.one;
    [SerializeField] [Min(0f)] private float rotationSpeed = 20f;

    private bool lockSight = false;
    private Vector3 lookPosLock = Vector3.zero;

    [Header("상태 목록")]
    [SerializeField] private EnemyState[] stateList;
    private EnemyState stateCurr = null;

    [Header("현재 상태")]
    [SerializeField] private int stateCurrIdx = -1;
    [SerializeField] private float stateDuration = 0.0f;

    private Animator animator = null;

    public Animator AnimControl
    {
        get { return animator; }
    }

    /// <summary>
    /// 적의 상태 목록입니다.
    /// </summary>
    protected EnemyState[] StateList
    {
        get { return stateList; }
    }

    /// <summary>
    /// 적의 현재 상태의 인덱스입니다.
    /// </summary>
    protected int StateCurrIdx
    {
        get { return stateCurrIdx; }
    }

    public bool IsChasing
    {
        get { return isChasing; }
    }

    /// <summary>
    /// 현재 상태가 얼마나 지속되었는지의 값입니다.
    /// </summary>
    protected float StateDuration
    {
        get { return stateDuration; }
    }

    public float Speed
    {
        get { return mAgent.speed; }
        set { mAgent.speed = value; }
    }

    public float Acceleration
    {
        get { return mAgent.acceleration; }
        set { mAgent.acceleration = value; }
    }

    public bool HasPath
    {
        get { return mAgent.hasPath; }
    }

    public Vector3 Dest
    {
        get { return mAgent.destination; }
    }

    protected void Awake()
    {
        mAgent = GetComponent<NavMeshAgent>();
        mAgent.updateRotation = false;

        GetComponent<Rigidbody>().isKinematic = true;

        Renderer enemyRenderer = GetComponentInChildren<Renderer>();
        material = Instantiate(enemyRenderer.material);
        enemyRenderer.material = material;
        cOrigin = material.color;

        hpCurr = hpMax;

        animator = GetComponentInChildren<Animator>();
        OnAwake();
    }

    /// <summary>
    /// Awake 대신 사용하세요.
    /// </summary>
    protected abstract void OnAwake();

    private void Update()
    {
        // 현재 상태의 행동을 실행
        if (stateCurr != null)
            stateCurr.OnUpdate();
        // 현재 상태가 지속된 시간을 업데이트
        stateDuration += Time.deltaTime;
        // 추적 On / Off
        if (isChasing && target != null && mAgent.enabled)
            mAgent.SetDestination(target.transform.position);

        if (lockSight)
            lookPos = lookPosLock - transform.position;
        else if (lookAtTarget)
            lookPos = target.transform.position - transform.position;
        else if (mAgent.enabled && mAgent.hasPath)
                lookPos = mAgent.desiredVelocity;
        lookPos.y = 0;

        if (lookPos.magnitude != 0)
        {
            Quaternion q = Quaternion.LookRotation(lookPos);
            transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * rotationSpeed);
        }

        float moveSpeed = 0;
        if (mAgent.enabled)
            moveSpeed = mAgent.desiredVelocity.magnitude;
        TrySetAnimFloat("MoveSpeed", moveSpeed);

        // 피격 시 점멸
        if (hitBlinkCurr > 0)
        {
            Color newColor = Color.white;
            float cChange = hitBlinkCurr / hitBlink;
            newColor.r = Mathf.Lerp(cOrigin.r, 1, cChange);
            newColor.g = Mathf.Lerp(cOrigin.g, 1, cChange);
            newColor.b = Mathf.Lerp(cOrigin.b, 1, cChange);
            material.color = newColor;
            hitBlinkCurr -= Time.deltaTime;
            if (hitBlink < 0)
                hitBlinkCurr = 0;
        }

        OnUpdate();
    }

    /// <summary>
    /// Update 대신 사용하세요.
    /// </summary>
    protected abstract void OnUpdate();

    /// <summary>
    /// 적의 추적을 시작 또는 정지합니다.
    /// </summary>
    /// <param name="b"></param>
    public void SetChase(bool b)
    {
        isChasing = b;
        if (!b)
            mAgent.ResetPath();
    }

    /// <summary>
    /// 적의 목표 대상을 지정합니다.
    /// </summary>
    /// <param name="g"></param>
    public void SetTarget(GameObject g)
    {
        target = g;
    }

    /// <summary>
    /// 적의 목표 지점을 지정합니다.
    /// </summary>
    /// <param name="v"></param>
    public void SetTarget(Vector3 v)
    {
        mAgent.SetDestination(v);
    }

    /// <summary>
    /// 목표 대상을 똑바로 바라보는 여부입니다.
    /// </summary>
    /// <param name="b"></param>
    public void SetLookAtTarget(bool b)
    {
        lookAtTarget = b;
    }

    /// <summary>
    /// 최초 상태를 설정하며, 동시에 초기화합니다. 최초에 한 번만 사용하세요.
    /// </summary>
    /// <param name="idx">전환할 상태의 인덱스입니다.</param>
    protected void SetDefaultState(int idx)
    {
        if (idx >= stateList.Length) return;
        if (stateCurr != null) stateCurr.OnExit();
        stateCurr = stateList[idx];
        stateCurrIdx = idx;
        stateCurr.LoadActor();
        stateCurr.OnEnter();
        stateDuration = 0.0f;
    }

    /// <summary>
    /// 상태를 전환합니다.
    /// </summary>
    /// <param name="newState">전환할 상태의 인덱스입니다.</param>
    public void SetState(int idx)
    {
        if (idx >= stateList.Length) return;
        if (stateCurr != null) stateCurr.OnExit();
        stateCurr = stateList[idx];
        stateCurrIdx = idx;
        stateCurr.OnEnter();
        stateDuration = 0.0f;
    }

    public void SetAiActive(bool b)
    {
        mAgent.enabled = b;
    }

    public GameObject GetTarget()
    {
        return target;
    }

    public bool IsArrived(float threshold)
    {
        if ((transform.position - mAgent.destination).magnitude <= threshold + 1)
            return true;
        return false;
    }

    protected void SetSightLock(Vector3 v)
    {
        lockSight = true;
        lookPosLock = v;
    }

    protected void ResetSightLock()
    {
        lockSight = false;
    }

    private bool HasAnimParam(string name)
    {
        if (AnimControl == null)
            return false;
        AnimatorControllerParameter[] a = AnimControl.parameters;
        foreach (AnimatorControllerParameter param in a)
        {
            if (param.name == name)
                return true;
        }
        return false;
    }

    protected bool TrySetAnimBool(string name, bool b)
    {
        if (HasAnimParam(name))
        {
            AnimControl.SetBool(name, b);
            return true;
        }
        return false;
    }

    protected bool TrySetAnimFloat(string name, float f)
    {
        if (HasAnimParam(name))
        {
            AnimControl.SetFloat(name, f);
            return true;
        }
        return false;
    }

    protected bool TrySetAnimTrigger(string name)
    {
        if (HasAnimParam(name))
        {
            AnimControl.SetTrigger(name);
            return true;
        }
        return false;
    }

    /// <summary>
    /// 공격을 받았을 때 호출됩니다.
    /// </summary>
    /// <param name="damage">amount(float)와 property(string)을 포함하는 구조체입니다.</param>
    public virtual void TakeDamage(Damage damage)
    {
        hpCurr -= damage.amount;
        hitBlinkCurr = hitBlink;

        if (hpCurr <= 0)
        {
            Destroy(gameObject);
        }
    }
}