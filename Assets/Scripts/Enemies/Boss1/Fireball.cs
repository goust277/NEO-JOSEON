using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    [Header("발사체 큐")]
    [SerializeField] private ProjectileQueue projQueue = null;

    [Header("발사체 속성")]
    [SerializeField] private float velocity = 1f;
    [SerializeField] private float range = 5f;
    [SerializeField] private LayerMask layerMask;

    [Header("활성화 주기")]
    [SerializeField] private float periodCurr = 0f;
    [SerializeField] private float periodMax = 5f;
    private bool Activated = false;
    [SerializeField] private float fireTime = 2.4f;
    private float fireTimeCurr = 0;
    [SerializeField] private float fireTiming = 1.2f;

    private GameObject model = null;

    private void Awake()
    {
        model = transform.GetChild(0).gameObject;
        model.SetActive(false);
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;
        if (!Activated && periodCurr < periodMax)
        {
            periodCurr += deltaTime;
            if (periodCurr >= periodMax)
            {
                Reposition();
                Activated = true;
                model.SetActive(true);
                periodCurr = 0;
                return;
            }
        }
        if (Activated)
        {
            if (fireTimeCurr < fireTiming && fireTimeCurr + deltaTime >= fireTiming)
            {
                Fire();
            }
            fireTimeCurr += deltaTime;
            if (fireTimeCurr >= fireTime)
            {
                fireTimeCurr = 0;
                Activated = false;
                model.SetActive(false);
            }
        }
    }

    private void Fire()
    {
        Damage d;
        d.amount = 0;
        d.property = "fire";
        Projectile p = projQueue.GetProjectile();
        if (p == null) return;

        p.SetOwnerID(gameObject.GetInstanceID());
        p.SetDamage(d);
        p.SetRange(range);
        p.SetPiercing(true);
        p.SetLayerMask(ref layerMask);

        p.SetSpeedVector(model.transform.forward * velocity);
        p.Fire(model.transform.position);
    }

    private void Reposition()
    {
        Vector3 newPos = model.transform.position;
        quaternion lookRot = quaternion.identity;
        float rand = 9.5f - UnityEngine.Random.Range(0, 20);

        switch(UnityEngine.Random.Range(0, 4))
        {
            case 0:
                newPos.z = -9.5f;
                newPos.x = rand;
                lookRot = quaternion.Euler(0, 0, 0);
                break;
            case 1:
                newPos.x = 9.5f;
                newPos.z = rand;
                lookRot = quaternion.Euler(0, math.PI * 1.5f, 0);
                break;
            case 2:
                newPos.z = 9.5f;
                newPos.x = rand;
                lookRot = quaternion.Euler(0, math.PI, 0);
                break;
            case 3:
                newPos.x = -9.5f;
                newPos.z = rand;
                lookRot = quaternion.Euler(0, math.PI * 0.5f, 0);
                break;
        }
        
        model.transform.rotation = lookRot;
        model.transform.position = newPos;
    }
}
