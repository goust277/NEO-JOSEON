using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
    public GameObject bounceEffectPrefab; // 이펙트 프리팹을 드래그 앤 드롭으로 설정
    private int bounceCount = 0;

    void Start()
    {
    }

    void OnCollisionEnter(Collision collision)
    {
        // 충돌한 오브젝트가 벽일 때
        if (collision.gameObject.CompareTag("Janggu"))
        {
            // 충돌 지점의 법선 벡터를 사용하여 이펙트의 방향 설정
            ContactPoint contact = collision.contacts[0];
            Vector3 effectPosition = contact.point - contact.normal * 1.0f; // 충돌 지점에서 법선 반대 방향으로 0.1 단위만큼 이동

            // 이펙트 생성
            GameObject effect = Instantiate(bounceEffectPrefab, effectPosition, Quaternion.LookRotation(contact.normal));

            // 1초 뒤에 이펙트를 삭제
            Destroy(effect, 1.0f);

            bounceCount++;

            // 튕긴 횟수가 5번이 되면 파괴
            if (bounceCount >= 5)
            {
                Destroy(gameObject);
            }
        }
        // 충돌한 오브젝트가 플레이어일 때
        else if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("아야아야");
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
