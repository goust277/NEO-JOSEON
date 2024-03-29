using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Direction : MonoBehaviour
{
    [SerializeField][Range(0f, 180f)] float viewDegree = 45;
    [SerializeField] float viewDist = 5;

    [SerializeField] GameObject target = null;

    private void OnDrawGizmos()
    {
        float rad = Mathf.Deg2Rad * viewDegree;

        bool isIndist = false;
        float dist = (target.transform.position - transform.position).magnitude;
        if (dist <= viewDist)
            isIndist = true;

        bool isInSight = false;
        Vector3 va = transform.forward;
        Vector3 vb = (target.transform.position - transform.position).normalized;
        float dot = Vector3.Dot(va, vb);
        if (dot >= Mathf.Cos(rad))
            isInSight = true;

        if (target != null && isIndist && isInSight)
            Gizmos.color = Color.green;
        else
            Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, viewDist);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 5);

        float xx = Mathf.Sin(rad) * viewDist;
        float zz = Mathf.Cos(rad) * viewDist;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(xx, 0, zz));
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(-xx, 0, zz));
    }
}
