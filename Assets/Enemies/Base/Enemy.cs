using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public abstract class Enemy : MonoBehaviour, IDamageable
{
    protected NavMeshAgent mAgent;
    [Header("Ÿ�� ���� ����")]
    [SerializeField] private GameObject target = null;
    private bool isChasing = false;
    [SerializeField] private bool lookAtTarget = true;
    private Vector3 lookPos = Vector3.one;
    [SerializeField] [Min(0f)] private float rotationSpeed = 20f;

    [Header("���� ���")]
    [SerializeField] private EnemyState[] stateList;
    private EnemyState stateCurr = null;

    [Header("���� ����")]
    [SerializeField] private int stateCurrIdx = -1;
    [SerializeField] private float stateDuration = 0.0f;

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

    /// <summary>
    /// ���� ���°� �󸶳� ���ӵǾ������� ���Դϴ�.
    /// </summary>
    protected float StateDuration
    {
        get { return stateDuration; }
    }

    protected void Awake()
    {
        mAgent = GetComponent<NavMeshAgent>();
        mAgent.updateRotation = false;
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
        if (isChasing && target != null)
        {
            mAgent.SetDestination(target.transform.position);
        }

        if (lookAtTarget)
        {
            if (mAgent.hasPath)
                lookPos = mAgent.desiredVelocity;
            else
                lookPos = target.transform.position - transform.position;
        }

        Quaternion q = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * rotationSpeed);

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

    public GameObject GetTarget()
    {
        return target;
    }

    /// <summary>
    /// ������ �޾��� �� ȣ��˴ϴ�.
    /// </summary>
    /// <param name="damage">amount(float)�� property(string)�� �����ϴ� ����ü�Դϴ�.</param>
    public virtual void TakeDamage(Damage damage)
    {

    }
}