using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

public class NewHitPad : MonoBehaviour
{
    public int hitGauge;
    public Material[] materialHitPower;
    private bool bIsFire = false;

    public ParticleSystem fireCylinderParticle;
    public GameObject AttackArea;

    Renderer renderer;

    void Start()
    {
        renderer = GetComponent<Renderer>();
    }

    public void GetHitGauge()
    {
        if(!bIsFire)
        {
            hitGauge++;
            if (hitGauge >= 3)
            {
                bIsFire = true;
                StartCoroutine(ResetHitGauge());
            }
            renderer.material = materialHitPower[hitGauge];
        } 
    }

    IEnumerator ResetHitGauge()
    {
        yield return new WaitForSeconds(2f);
        AttackArea.SetActive(true);
        fireCylinderParticle.Play();
        hitGauge = 0;
        renderer.material = materialHitPower[hitGauge];
        yield return new WaitForSeconds(0.85f);
        AttackArea.SetActive(false);
        bIsFire = false;
    }

    public void CoolingHitGauge()
    {
        Debug.Log("작동하였습니다.");
        if (!bIsFire && hitGauge >= 1)
        {
            Debug.Log("쿨링되었삳");
            hitGauge--;
            renderer.material = materialHitPower[hitGauge];
        }
    }
}
