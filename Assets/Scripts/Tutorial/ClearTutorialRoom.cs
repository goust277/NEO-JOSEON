using System.Collections;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using UnityEngine;
using UnityEngine.UI;

public class ClearTutorialRoom : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] public GameObject txtUi;
    [SerializeField] private int openZone;
    [SerializeField] private ClipBoard clipboard;
    [SerializeField] private Text txt;

    [Header("¹®")]
    [SerializeField] private GameObject[] doors;

    private int MaxPoint;
    private int point = 0;

    [Header("Á¸")]
    [SerializeField] private GameObject zone;
    [SerializeField] private GameObject[] zones;

    private void Start()
    {
        txtUi.SetActive(false);
        zone.SetActive(false);
        MaxPoint = zones.Length;
    }
    void Update()
    {
        if (txtUi != null &&  txt != null) 
        {
            if (clipboard.id == openZone - 1)
            {
                zone.SetActive(true);
            }
            if (clipboard.id == openZone)
            {
                if (point == MaxPoint)
                {
                    for (int i = 0; i < doors.Length; i++)
                    {
                        Destroy(doors[i]);
                    }
                }
                else
                {
                    zones[point].SetActive(true);
                }

                txtUi.SetActive(true);
                txt.text = "(" + point + "/" + MaxPoint + ")";
            }
            else if (clipboard.id > openZone)
            {
                txtUi.SetActive (false);
                txtUi = null;
                txt = null;
            }
        }

            
    }

    public void PointUp()
    {
        point++;
    }
}
