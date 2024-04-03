using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    private Damage damage;
    private Rigidbody rb;
    private int ownerID = -1;
    private float range = 5f;
    private float distTrav = 0f; // ���� �̵��� �Ÿ�
    private bool piercing = false;

    private LayerMask layerRef = 0;

    private void Awake()
    {
        damage.amount = 0;
        damage.property = "";
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        distTrav += Time.deltaTime * rb.velocity.magnitude;
        if (distTrav >= range)
            gameObject.SetActive(false);
    }

    /// <summary>
    /// ����ü�� �����մϴ�.
    /// </summary>
    /// <param name="pos"></param>
    public void Fire(Vector3 pos)
    {
        transform.position = pos;
        gameObject.SetActive(true);
        distTrav = 0f;
    }

    public void SetLayerMask(ref LayerMask l)
    {
        layerRef = l;
    }

    /// <summary>
    /// �� ����ü�� �����ڸ� �����մϴ�.
    /// </summary>
    /// <param name="id"></param>
    public void SetOwnerID(int id)
    {
        ownerID = id;
    }

    /// <summary>
    /// �� ����ü�� �ִ� ��Ÿ��� �����մϴ�.
    /// </summary>
    /// <param name="r"></param>
    public void SetRange(float r)
    {
        range = r;
    }

    /// <summary>
    /// ���� ���� �� ���ط��� �����մϴ�.
    /// </summary>
    /// <param name="d"></param>
    public void SetDamage(Damage d)
    {
        damage = d;
    }

    /// <summary>
    /// ����ü�� �ӵ��� �����մϴ�. (���� * �ӷ�)
    /// </summary>
    /// <param name="v"></param>
    public void SetSpeedVector(Vector3 v)
    {
        rb.velocity = v;
    }

    /// <summary>
    /// ���� ���θ� �����մϴ�.
    /// </summary>
    /// <param name="b"></param>
    public void SetPiercing(bool b)
    {
        piercing = b;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & layerRef.value) == 0) return;

        if (other.gameObject.GetInstanceID() == ownerID) return;

        IDamageable target = other.GetComponent<IDamageable>();
        if (target == null) return;

        target.TakeDamage(damage);
        if (!piercing) gameObject.SetActive(false);
    }

    public bool IsAvailable()
    {
        return !gameObject.activeSelf;
    }
}
