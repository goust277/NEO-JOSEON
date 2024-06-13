using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorJanggu : MonoBehaviour
{
    public float projectileSpeed = 15f;   // 벽 반사 속도

    private void OnCollisionEnter(Collision collision)
    {
        if ((collision.gameObject.CompareTag("Projectile")))
        {
            Vector3 collisionDirection = collision.transform.forward;
            Vector3 direction = transform.forward;

            // 반사된 방향을 계산
            Vector3 reflectedDirection = Vector3.Reflect(collisionDirection, direction).normalized;

            Rigidbody projectileRigidbody = collision.gameObject.GetComponent<Rigidbody>();

            // 오브젝트 튕김 구현
            if (projectileRigidbody != null)
            {
                projectileRigidbody.transform.forward = reflectedDirection + Vector3.up;
                projectileRigidbody.velocity = reflectedDirection + Vector3.up * projectileSpeed;
            }
        }
    }
}
