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
    private float distTrav = 0f; // 현재 이동한 거리
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
    /// 투사체를 생성합니다.
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
    /// 이 투사체의 소유자를 지정합니다.
    /// </summary>
    /// <param name="id"></param>
    public void SetOwnerID(int id)
    {
        ownerID = id;
    }

    /// <summary>
    /// 이 투사체의 최대 사거리를 지정합니다.
    /// </summary>
    /// <param name="r"></param>
    public void SetRange(float r)
    {
        range = r;
    }

    /// <summary>
    /// 공격 유형 및 피해량을 지정합니다.
    /// </summary>
    /// <param name="d"></param>
    public void SetDamage(Damage d)
    {
        damage = d;
    }

    /// <summary>
    /// 투사체의 속도를 지정합니다. (방향 * 속력)
    /// </summary>
    /// <param name="v"></param>
    public void SetSpeedVector(Vector3 v)
    {
        rb.velocity = v;
    }

    /// <summary>
    /// 관통 여부를 지정합니다.
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
