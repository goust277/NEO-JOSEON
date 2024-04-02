using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlacer : MonoBehaviour
{
    public GameObject prefab;
    private void Awake()
    {
        GetComponent<MeshRenderer>().enabled = false;
        for (int w = 0; w < 16; w++)
        {
            for (int h = 0; h < 16; h++)
            {
                Vector3 v = Vector3.zero;
                v.x = 15 - 2 * w;
                v.z = 15 - 2 * h;
                GameObject g = Instantiate(prefab, null);
                g.transform.position = v;
                g.transform.SetParent(transform);
            }
        }
    }
}
