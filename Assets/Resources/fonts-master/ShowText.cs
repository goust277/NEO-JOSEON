using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class ShowText : MonoBehaviour
{
    [SerializeField] private GameObject txt;
    [SerializeField] private GameObject arrow;
    void Start()
    {
        txt.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            txt.SetActive(true);
            arrow.SetActive(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            txt.SetActive(false);
            arrow.SetActive(true);
        }

    }
}
