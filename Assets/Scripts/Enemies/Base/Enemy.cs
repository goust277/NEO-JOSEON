using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(Rigidbody))]
public abstract class Enemy : MonoBehaviour, IDamageable
{
    protected NavMeshAgent mAgent;

    [Header("ü��")]
    [SerializeField] protected float hpMax = 1;
    [SerializeField] protected float hpCurr = 1f;
    private float hitBlink = 0.3f;
    private float hitBlinkCurr = 0f;
    private Material material = null;
    private Color cOrigin;

    [Header("Ÿ�� ���� ����")]
    [SerializeField] private GameObject target = null;
    private bool isChasing = false;
    [SerializeField] private bool lookAtTarget = true;
    private Vector3 lookPos = Vector3.one;
    [SerializeField] [Min(0f)] private float rotationSpeed = 20f;

    private bool lockSight = false;
    private Vector3 lookPosLock = Vector3.zero;

    [Header("���� ���")]
    [SerializeField] private EnemyState[] stateList;
    private EnemyState stateCurr = null;

    [Header("���� ����")]
    [SerializeField] private int stateCurrIdx = -1;
    [SerializeField] private float stateDuration = 0.0f;

    private Animator animator = null;

    public Animator AnimControl
    {
        get { return animator; }
    }

    /// <summary>
    /// ���� ���� ����Դϴ�.
    /// </summary>
    protected EnemyState[] StateList
    {
        get { return stateList; }
    }

    /// <summary>
    /// ���� ���� ������ �ε����Դϴ�.
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
    /// ���� ���°� �󸶳� ���ӵǾ������� ���Դϴ�.
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
    /// Awake ��� ����ϼ���.
    /// </summary>
    protected abstract void OnAwake();

    private void Update()
    {
        // ���� ������ �ൿ�� ����
        if (stateCurr != null)
            stateCurr.OnUpdate();
        // ���� ���°� ���ӵ� �ð��� ������Ʈ
        stateDuration += Time.deltaTime;
        // ���� On / Off
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

        // �ǰ� �� ����
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
    /// Update ��� ����ϼ���.
    /// </summary>
    protected abstract void OnUpdate();

    /// <summary>
    /// ���� ������ ���� �Ǵ� �����մϴ�.
    /// </summary>
    /// <param name="b"></param>
    public void SetChase(bool b)
    {
        isChasing = b;
        if (!b)
            mAgent.ResetPath();
    }

    /// <summary>
    /// ���� ��ǥ ����� �����մϴ�.
    /// </summary>
    /// <param name="g"></param>
    public void SetTarget(GameObject g)
    {
        target = g;
    }

    /// <summary>
    /// ���� ��ǥ ������ �����մϴ�.
    /// </summary>
    /// <param name="v"></param>
    public void SetTarget(Vector3 v)
    {
        mAgent.SetDestination(v);
    }

    /// <summary>
    /// ��ǥ ����� �ȹٷ� �ٶ󺸴� �����Դϴ�.
    /// </summary>
    /// <param name="b"></param>
    public void SetLookAtTarget(bool b)
    {
        lookAtTarget = b;
    }

    /// <summary>
    /// ���� ���¸� �����ϸ�, ���ÿ� �ʱ�ȭ�մϴ�. ���ʿ� �� ���� ����ϼ���.
    /// </summary>
    /// <param name="idx">��ȯ�� ������ �ε����Դϴ�.</param>
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
    /// ���¸� ��ȯ�մϴ�.
    /// </summary>
    /// <param name="newState">��ȯ�� ������ �ε����Դϴ�.</param>
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
    /// ������ �޾��� �� ȣ��˴ϴ�.
    /// </summary>
    /// <param name="damage">amount(float)�� property(string)�� �����ϴ� ����ü�Դϴ�.</param>
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