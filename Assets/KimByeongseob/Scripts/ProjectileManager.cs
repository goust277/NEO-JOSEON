using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
    public GameObject bounceEffectPrefab; // 이펙트 프리팹을 드래그 앤 드롭으로 설정
    public float fDamage = 1f;
    private int bounceCount = 0;
    public Rigidbody rb;
    public float speedThreshold = 0.3f; // 속도가 0으로 간주되는 임계값

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    void Update()
    {
        // 속도가 임계값 이하이면 오브젝트 삭제
        if (rb.velocity.magnitude <= speedThreshold)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // 충돌한 오브젝트가 벽일 때
        if (collision.gameObject.CompareTag("Janggu") || collision.gameObject.CompareTag("Buoy") || collision.gameObject.CompareTag("Plane"))
        {
            // 충돌 지점의 법선 벡터를 사용하여 이펙트의 방향 설정
            ContactPoint contact = collision.contacts[0];
            Vector3 effectPosition = contact.point - contact.normal * 1.0f; // 충돌 지점에서 법선 반대 방향으로 0.1 단위만큼 이동

            // 이펙트 생성
            GameObject effect = Instantiate(bounceEffectPrefab, effectPosition, Quaternion.LookRotation(contact.normal));

            // 1초 뒤에 이펙트를 삭제
            Destroy(effect, 1.0f);
        }
        // 충돌한 오브젝트가 플레이어일 때
        else if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerDamage>().TakeDamage(fDamage);
            Destroy(gameObject);
            return;
        }
        // 공과 충돌 시 공에 힘을 가함
        else if (collision.gameObject.CompareTag("Buoy"))
        {
            // 반사각 계산
            Vector3 reflectDir = Vector3.Reflect(rb.velocity.normalized, collision.contacts[0].normal);
            rb.velocity = reflectDir * 30f;

            Rigidbody Buoy = collision.gameObject.GetComponent<Rigidbody>();
            if (Buoy != null)
            {
                Buoy.AddForce(reflectDir * 30f, ForceMode.Impulse);
            }
        }

        bounceCount++;

        // 튕긴 횟수가 5번이 되면 파괴
        if (bounceCount >= 8)
        {
            Destroy(gameObject);
        }
    }
}
