using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
    public GameObject bounceEffectPrefab; // ����Ʈ �������� �巡�� �� ������� ����
    private int bounceCount = 0;

    void Start()
    {
    }

    void OnCollisionEnter(Collision collision)
    {
        // �浹�� ������Ʈ�� ���� ��
        if (collision.gameObject.CompareTag("Janggu"))
        {
            // �浹 ������ ���� ���͸� ����Ͽ� ����Ʈ�� ���� ����
            ContactPoint contact = collision.contacts[0];
            Vector3 effectPosition = contact.point - contact.normal * 1.0f; // �浹 �������� ���� �ݴ� �������� 0.1 ������ŭ �̵�

            // ����Ʈ ����
            GameObject effect = Instantiate(bounceEffectPrefab, effectPosition, Quaternion.LookRotation(contact.normal));

            // 1�� �ڿ� ����Ʈ�� ����
            Destroy(effect, 1.0f);

            bounceCount++;

            // ƨ�� Ƚ���� 5���� �Ǹ� �ı�
            if (bounceCount >= 5)
            {
                Destroy(gameObject);
            }
        }
        // �浹�� ������Ʈ�� �÷��̾��� ��
        else if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("�ƾ߾ƾ�");
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
