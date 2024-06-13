using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
    public GameObject bounceEffectPrefab; // ����Ʈ �������� �巡�� �� ������� ����
    public float fDamage = 1f;
    private int bounceCount = 0;
    public Rigidbody rb;
    public float speedThreshold = 0.3f; // �ӵ��� 0���� ���ֵǴ� �Ӱ谪

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    void Update()
    {
        // �ӵ��� �Ӱ谪 �����̸� ������Ʈ ����
        if (rb.velocity.magnitude <= speedThreshold)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // �浹�� ������Ʈ�� ���� ��
        if (collision.gameObject.CompareTag("Janggu") || collision.gameObject.CompareTag("Buoy") || collision.gameObject.CompareTag("Plane"))
        {
            // �浹 ������ ���� ���͸� ����Ͽ� ����Ʈ�� ���� ����
            ContactPoint contact = collision.contacts[0];
            Vector3 effectPosition = contact.point - contact.normal * 1.0f; // �浹 �������� ���� �ݴ� �������� 0.1 ������ŭ �̵�

            // ����Ʈ ����
            GameObject effect = Instantiate(bounceEffectPrefab, effectPosition, Quaternion.LookRotation(contact.normal));

            // 1�� �ڿ� ����Ʈ�� ����
            Destroy(effect, 1.0f);
        }
        // �浹�� ������Ʈ�� �÷��̾��� ��
        else if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerDamage>().TakeDamage(fDamage);
            Destroy(gameObject);
            return;
        }
        // ���� �浹 �� ���� ���� ����
        else if (collision.gameObject.CompareTag("Buoy"))
        {
            // �ݻ簢 ���
            Vector3 reflectDir = Vector3.Reflect(rb.velocity.normalized, collision.contacts[0].normal);
            rb.velocity = reflectDir * 30f;

            Rigidbody Buoy = collision.gameObject.GetComponent<Rigidbody>();
            if (Buoy != null)
            {
                Buoy.AddForce(reflectDir * 30f, ForceMode.Impulse);
            }
        }

        bounceCount++;

        // ƨ�� Ƚ���� 5���� �Ǹ� �ı�
        if (bounceCount >= 8)
        {
            Destroy(gameObject);
        }
    }
}
