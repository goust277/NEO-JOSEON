using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuoyManager : NewEnemy
{
    private SoundManager soundManager;
    public float fDamage = 1f;
    public Rigidbody rb;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        soundManager = FindObjectOfType<SoundManager>();
    }

    void OnCollisionEnter(Collision collision)
    {

        if(collision.gameObject.CompareTag("Projectile") || collision.gameObject.CompareTag("Janggu"))
        {
            soundManager.PlaySound("BuoyBounce");
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerDamage>().TakeDamage(fDamage);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        soundManager.PlaySound("BuoyBounce");
        if (other.gameObject.CompareTag("Melee"))
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        if (other.CompareTag("Wave"))
        {
            // Calculate the direction opposite to the collision point
            Vector3 collisionNormal = other.ClosestPoint(transform.position) - transform.position;
            Vector3 bounceDirection = -collisionNormal + Vector3.up;

            // Apply the force to the Rigidbody
            rb.AddForce(bounceDirection.normalized * 20f, ForceMode.Impulse);
        }
    }
}
