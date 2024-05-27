using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PArticleCycle : MonoBehaviour
{
    public float cycleTime;
    public ParticleSystem asdasd;

    private void Start()
    {
        StartCoroutine(PerformActionRepeatedly());
    }

    private IEnumerator PerformActionRepeatedly()
    {
        while (true)
        {
            PerformAction();
            yield return new WaitForSeconds(cycleTime);
        }
    }

    private void PerformAction()
    {
        asdasd.Play();
    }
}
