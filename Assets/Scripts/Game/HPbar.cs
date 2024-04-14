using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPbar : MonoBehaviour
{
    private Transform barTransform;
    private RectTransform fill;
    private Enemy target;
    private bool active = false;

    private Vector2 fullV;

    public void Initialize(GameObject screenEffect)
    {
        barTransform = screenEffect.transform.GetChild(0);
        fill = barTransform.GetChild(2).GetComponent<RectTransform>();
        fullV = fill.sizeDelta;

        Deactive();
    }

    public void Active(Enemy target)
    {
        active = true;
        barTransform.gameObject.SetActive(true);
        this.target = target;
    }

    public void Deactive()
    {
        active = false;
        barTransform.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (active)
        {
            float rate = 0;
            if (target)
                rate = target.HPPercentage;
            Vector2 currV = fullV;
            currV.x *= rate;
            fill.sizeDelta = currV;
        }
    }
}
