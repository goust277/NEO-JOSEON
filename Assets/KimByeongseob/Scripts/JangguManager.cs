using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JangguManager : MonoBehaviour
{
    public float projectileSpeed = 15f;   // 벽 반사 속도
    public bool sideProjectile = false;
    public bool upProjectile = false;
    public bool downProjectile = false;

    public bool linoleumJanggu = true;     // 장구 장판 활성화 상태

    private void OnCollisionEnter(Collision collision)
    {
        Vector3 collisionDirection = collision.transform.forward;
        Vector3 direction = transform.forward;

        // 반사된 방향을 계산
        Vector3 reflectedDirection = Vector3.Reflect(collisionDirection, direction).normalized;

        Rigidbody projectileRigidbody = collision.gameObject.GetComponent<Rigidbody>();

        // 오브젝트 튕김 구현
        if (projectileRigidbody != null)
        {
            if ((collision.gameObject.CompareTag("Player")))
            {
                if (upProjectile && linoleumJanggu)
                {
                    //projectileRigidbody.transform.forward = reflectedDirection + Vector3.up;
                    projectileRigidbody.velocity = reflectedDirection + Vector3.up * projectileSpeed / 2;
                }
                else if (downProjectile)
                {
                    //projectileRigidbody.transform.forward = reflectedDirection + Vector3.down;
                    projectileRigidbody.velocity = reflectedDirection + Vector3.down * projectileSpeed / 2;
                }
                else if (sideProjectile)
                {
                    //projectileRigidbody.transform.forward = reflectedDirection;
                    projectileRigidbody.velocity = reflectedDirection * projectileSpeed / 2;
                }
            }
            else if ((collision.gameObject.CompareTag("Buoy")))
            {
                if (upProjectile && linoleumJanggu)
                {
                    //projectileRigidbody.transform.forward = reflectedDirection + Vector3.up;
                    projectileRigidbody.velocity = reflectedDirection + Vector3.up * projectileSpeed * 1.5f;
                }
                else if (downProjectile)
                {
                    //projectileRigidbody.transform.forward = reflectedDirection + Vector3.down;
                    projectileRigidbody.velocity = reflectedDirection + Vector3.down * projectileSpeed * 1.5f;
                }
                else if (sideProjectile)
                {
                    //projectileRigidbody.transform.forward = reflectedDirection;
                    projectileRigidbody.velocity = reflectedDirection * projectileSpeed * 1.5f;
                }
            }
            else
            {
                if (upProjectile && linoleumJanggu)
                {
                    projectileRigidbody.transform.forward = reflectedDirection + Vector3.up;
                    projectileRigidbody.velocity = reflectedDirection + Vector3.up * projectileSpeed;
                }
                else if (downProjectile)
                {
                    projectileRigidbody.transform.forward = reflectedDirection + Vector3.down;
                    projectileRigidbody.velocity = reflectedDirection + Vector3.down * projectileSpeed;
                }
                else if (sideProjectile)
                {
                    projectileRigidbody.transform.forward = reflectedDirection;
                    projectileRigidbody.velocity = reflectedDirection * projectileSpeed;
                }
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        Vector3 collisionDirection = collision.transform.forward;
        Vector3 direction = transform.forward;

        // 반사된 방향을 계산
        Vector3 reflectedDirection = Vector3.Reflect(collisionDirection, direction).normalized;

        Rigidbody projectileRigidbody = collision.gameObject.GetComponent<Rigidbody>();

        if ((collision.gameObject.CompareTag("Player")))
        {
            if (upProjectile && linoleumJanggu)
            {
                //projectileRigidbody.transform.forward = reflectedDirection + Vector3.up;
                projectileRigidbody.velocity = reflectedDirection + Vector3.up * projectileSpeed / 2;
            }
        }
    }
}



