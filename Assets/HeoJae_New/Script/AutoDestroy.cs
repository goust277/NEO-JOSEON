using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    public float destoyTime;

    
    void Awake()
    {
        Destroy(gameObject, destoyTime);
    }

    
}
