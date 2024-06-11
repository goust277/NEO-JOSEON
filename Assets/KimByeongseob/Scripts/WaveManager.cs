using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{

    // ���� ���� (1: �ð����, 2: �ݽð����, 3: �߾ӹ���, 4: �߾ӹݴ����)
    public float influenceForce1;
    public float influenceForce2;  
    public float influenceForce3;  
    public float influenceForce4;
    public int waveType;           


    private void OnTriggerStay(Collider other)
    {
        //if (other.CompareTag("Enemy")) // �浹�� ��ü�� �÷��̾����� Ȯ��
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