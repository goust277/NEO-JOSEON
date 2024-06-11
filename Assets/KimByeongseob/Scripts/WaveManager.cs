using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{

    // 물결 유형 (1: 시계방향, 2: 반시계방향, 3: 중앙방향, 4: 중앙반대방향)
    public float influenceForce1;
    public float influenceForce2;  
    public float influenceForce3;  
    public float influenceForce4;
    public int waveType;           


    private void OnTriggerStay(Collider other)
    {
        //if (other.CompareTag("Enemy")) // 충돌한 객체가 플레이어인지 확인
        //{
            Rigidbody EnemyRigidbody = other.GetComponent<Rigidbody>();
            if (EnemyRigidbody != null)
            {
                switch (waveType)
                {
                    case 1:
                        ClockwiseWave(EnemyRigidbody);
                        break;
                    case 2:
                        CounterClockwiseWave(EnemyRigidbody);
                        break;
                    case 3:
                        InwardWave(EnemyRigidbody);
                        break;
                    case 4:
                        OutwardWave(EnemyRigidbody);
                        break;
                    default:
                        Debug.Log("Invalid wave type");
                        break;
                }
            }
        //}
    }

    private void ClockwiseWave(Rigidbody EnemyRigidbody)
    {
        Vector3 directionToCenter = transform.position - EnemyRigidbody.transform.position;
        Vector3 perpendicularDirection = new Vector3(-directionToCenter.z, 0, directionToCenter.x).normalized;
        EnemyRigidbody.AddForce(perpendicularDirection * influenceForce1 * Time.deltaTime, ForceMode.Impulse);
        EnemyRigidbody.velocity = new Vector3(0, EnemyRigidbody.velocity.y, 0);
    }

    private void CounterClockwiseWave(Rigidbody EnemyRigidbody)
    {
        Vector3 directionToCenter = transform.position - EnemyRigidbody.transform.position;
        Vector3 perpendicularDirection = new Vector3(directionToCenter.z, 0, -directionToCenter.x).normalized;
        EnemyRigidbody.AddForce(perpendicularDirection * influenceForce2 * Time.deltaTime, ForceMode.Impulse);
        EnemyRigidbody.velocity = new Vector3(0, EnemyRigidbody.velocity.y, 0);
    }

    private void InwardWave(Rigidbody EnemyRigidbody)
    {
        Vector3 directionToCenter = transform.position - EnemyRigidbody.transform.position;
        EnemyRigidbody.AddForce(directionToCenter * influenceForce3 * Time.deltaTime, ForceMode.Impulse);
        EnemyRigidbody.velocity = new Vector3(0, EnemyRigidbody.velocity.y, 0);
    }

    private void OutwardWave(Rigidbody EnemyRigidbody)
    {
        Vector3 directionToCenter = EnemyRigidbody.transform.position - transform.position ;
        EnemyRigidbody.AddForce(directionToCenter * influenceForce4 * Time.deltaTime, ForceMode.Impulse);
        EnemyRigidbody.velocity = new Vector3(0, EnemyRigidbody.velocity.y, 0);
    }
}