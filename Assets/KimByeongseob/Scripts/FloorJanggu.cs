using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorJanggu : MonoBehaviour
{
    public float projectileSpeed = 15f;   // �� �ݻ� �ӵ�

    private void OnCollisionEnter(Collision collision)
    {
        if ((collision.gameObject.CompareTag("Projectile")))
        {
            Vector3 collisionDirection = collision.transform.forward;
            Vector3 direction = transform.forward;

            // �ݻ�� ������ ���
            Vector3 reflectedDirection = Vector3.Reflect(collisionDirection, direction).normalized;

            Rigidbody projectileRigidbody = collision.gameObject.GetComponent<Rigidbody>();

            // ������Ʈ ƨ�� ����
            if (projectileRigidbody != null)
            {
                projectileRigidbody.transform.forward = reflectedDirection + Vector3.up;
                projectileRigidbody.velocity = reflectedDirection + Vector3.up * projectileSpeed;
            }
        }
    }
}
