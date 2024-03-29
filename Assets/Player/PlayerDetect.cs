using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PlayerDetect : MonoBehaviour
{
    public float viewRadius;
    [Range(0f, 360f)]
    public float viewAngle;

    public LayerMask targetMask, obstacleMask, InteractableMask;

    public List<Transform> visibleTargets = new List<Transform>();
    public List<Transform> Interactable = new List<Transform>();


    void Start()
    {
        StartCoroutine(FindTargetWithDelay(0.2f));
    }

    IEnumerator FindTargetWithDelay(float delay)
    {
        while (true) 
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
            FindInteractable();
        }
    }

    void FindVisibleTargets()
    {
        visibleTargets.Clear();
        Collider[] targetsinViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

        for (int i = 0; i < targetsinViewRadius.Length; i++) 
        {
            Transform target = targetsinViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                float dstToTarget = Vector3.Distance(transform.position, target.transform.position);

                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask)) 
                {
                    visibleTargets.Add(target);
                }
            }
        }
    }

    void FindInteractable()
    {
        Interactable.Clear();
        Collider[] targetsinViewRadius = Physics.OverlapSphere(transform.position, viewRadius, InteractableMask);

        for (int i = 0; i < targetsinViewRadius.Length; i++)
        {
            Transform target = targetsinViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                float dstToTarget = Vector3.Distance(transform.position, target.transform.position);

                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                {
                    Interactable.Add(target);
                }
            }
        }
    }

    public Vector3 DirFromAngle(float angleDegrees, bool anglelsGlobal)
    {
        if (!anglelsGlobal)
        {
            angleDegrees += transform.eulerAngles.y;
        }
        return new Vector3 (Mathf.Cos((-angleDegrees + 90) * Mathf.Deg2Rad), 0, Mathf.Sin((-angleDegrees + 90) * Mathf.Deg2Rad));
    }

    
}
