using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WWWater : MonoBehaviour
{
    public bool bIsTurn;

    public float influenceForce1;
    public float influenceForce2;
    public int waveType;

    private List<Monster_Sangmo> monstersInTrigger = new List<Monster_Sangmo>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Monster_Sangmo monsterSangmo = other.gameObject.GetComponent<Monster_Sangmo>();
            if (monsterSangmo != null)
            {
                monstersInTrigger.Add(monsterSangmo);
                monsterSangmo.isInWater = bIsTurn;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Monster_Sangmo monsterSangmo = other.gameObject.GetComponent<Monster_Sangmo>();
            if (monsterSangmo != null)
            {
                monstersInTrigger.Remove(monsterSangmo);
            }
        }
    }

    private void Update()
    {
        foreach (var monster in monstersInTrigger)
        {
            monster.isInWater = bIsTurn;
        }
    }



    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Rigidbody enemyRigidbody = other.GetComponent<Rigidbody>();

            if (enemyRigidbody != null)
            {
                switch (waveType)
                {
                    case 0:
                        break;
                    case 1:
                        ClockwiseWave(enemyRigidbody);
                        break;
                    case 2:
                        CounterClockwiseWave(enemyRigidbody);
                        break;
                    default:
                        break;
                }
            }
        }
    }

    private void ClockwiseWave(Rigidbody enemyRigidbody)
    {
        Vector3 directionToCenter = transform.position - enemyRigidbody.transform.position;
        Vector3 perpendicularDirection = new Vector3(-directionToCenter.z, 0, directionToCenter.x).normalized;
        enemyRigidbody.AddForce(perpendicularDirection * influenceForce1 * Time.deltaTime, ForceMode.Impulse);
        enemyRigidbody.velocity = new Vector3(0, enemyRigidbody.velocity.y, 0);

        Debug.Log("미는 중");
    }

    private void CounterClockwiseWave(Rigidbody enemyRigidbody)
    {
        Vector3 directionToCenter = transform.position - enemyRigidbody.transform.position;
        Vector3 perpendicularDirection = new Vector3(directionToCenter.z, 0, -directionToCenter.x).normalized;
        enemyRigidbody.AddForce(perpendicularDirection * influenceForce2 * Time.deltaTime, ForceMode.Impulse);
        enemyRigidbody.velocity = new Vector3(0, enemyRigidbody.velocity.y, 0);

        Debug.Log("미는 중");
    }
}
