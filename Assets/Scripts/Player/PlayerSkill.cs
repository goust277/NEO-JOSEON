using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkill : MonoBehaviour
{
    public GameObject range;

    public float skillRange;
    public float pushForce;

    public LayerMask enemyLayer, HitLayer;

    private float delay;

    private void Awake()
    {
        range.SetActive(false);
    }
    private void Update()
    {
        if (delay < 3f)
        {
            delay += Time.deltaTime;
        }

    }
    public void TriggerSkill()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, skillRange, enemyLayer);
        Collider[] hidCols = Physics.OverlapSphere(transform.position, skillRange, HitLayer);

        foreach (Collider collider in colliders)
        {
            Rigidbody enemyRigidbody = collider.GetComponent<Rigidbody>();
            if (enemyRigidbody != null)
            {
                enemyRigidbody.isKinematic = false;
                Vector3 pushDirection = (collider.transform.position - transform.position).normalized;

                enemyRigidbody.AddForce(pushDirection * pushForce, ForceMode.Impulse);
            }
            delay = 0;
            if (delay > 1f)
            {
                enemyRigidbody.isKinematic = true;
            }

        }

        foreach (Collider collider in hidCols)
        {
            HitPad hitPad = collider.GetComponent<HitPad>();
            hitPad.HitDown();
            
        }
        range.SetActive(true);

        Invoke("SkillOff", 0.2f);
    }

    private void SkillOff()
    {
        range.SetActive(false);
    }
}
