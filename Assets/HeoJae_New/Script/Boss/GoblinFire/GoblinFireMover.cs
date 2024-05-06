using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GoblinFireMover : MonoBehaviour
{
    [Header("ÆÄÆ¼Å¬")]
    public ParticleSystem particleFire;

    private float interval = 0.1f;
    private float timer = 0f;

    public int direcNum;
    public float moveSpeed;
    Vector3 moveDirection = Vector3.zero;

    private void Awake()
    {
        Destroy(gameObject,2.7f);
    }


    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= interval)
        {
            CreateFireEffect();
            timer = 0f;
        }

        transform.position += moveDirection * Time.deltaTime * moveSpeed;


    }

    private void CreateFireEffect()
    {
        ParticleSystem particle = Instantiate(particleFire, gameObject.transform.position, Quaternion.identity);
        Destroy(particle, 1f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HitPad"))
        {
            NewHitPad newHitPad = other.gameObject.GetComponent<NewHitPad>();
            newHitPad.GetHitGauge();
            return;
        }
    }

    public void DirectionSetting(int num)
    {
        switch (num)
        {
            case 1: // Line_1
                moveDirection = transform.forward;
                break;
            case 2: // Line_2
                moveDirection = -transform.forward;
                break;
            case 3: // Line_3
                moveDirection = -transform.right;
                break;
            case 4: // Line_4
                moveDirection = transform.right;
                break;
        }
    }
}
