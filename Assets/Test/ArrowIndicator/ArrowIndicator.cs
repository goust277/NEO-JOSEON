using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Image;


/* 
 * ���� �����̽� ĵ����
 * �ȼ� �� ���� 100
 * X ȸ�� �� 901 ���� �ۼ���.
*/
public class ArrowIndicator : MonoBehaviour
{
    private Image outline = null;
    private Image fill = null;
    private bool isFilling = false;

    private void Awake()
    {
        outline = transform.GetChild(0).GetComponent<Image>();
        fill = transform.GetChild(1).GetComponent<Image>();
        SetActive(false);
    }

    public void SetActive(bool b)
    {
        outline.gameObject.SetActive(b);
        fill.gameObject.SetActive(b);
    }

    private Vector3 ConvertVector(Vector3 v)
    {
        return new Vector3(v.x, v.z, -v.y);
    }

    public void SetThickness(float t)
    {
        Vector2 newSize = outline.rectTransform.sizeDelta;
        newSize.y = t * 100f;
        outline.rectTransform.sizeDelta = newSize;
        fill.rectTransform.sizeDelta = newSize;
    }

    public float ChangeVector(Vector3 origin, Vector3 target)
    {
        origin = ConvertVector(origin);
        target = ConvertVector(target);

        // ��ġ ����
        transform.localPosition = origin;

        Vector3 dirVector = target - origin;
        float lengthGoal = dirVector.magnitude;

        // �ƿ����� ���� ����
        Vector2 newSize = outline.rectTransform.sizeDelta;
        newSize.x = lengthGoal * 100f;
        outline.rectTransform.sizeDelta = newSize;

        // ���� ���� �ʱ�ȭ
        newSize.x = 0f;
        fill.rectTransform.sizeDelta = newSize;

        // ���� ����
        Vector3 newRotation = transform.localRotation.eulerAngles;
        newRotation.z = Mathf.Atan2(dirVector.y, dirVector.x) * Mathf.Rad2Deg;
        transform.localRotation = Quaternion.Euler(newRotation);
        return lengthGoal;
    }

    public void StartFill(Vector3 origin, Vector3 target, float fillTime)
    {
        if (isFilling || origin == target) return;
        isFilling = true;

        SetActive(true);
        float lengthGoal = ChangeVector(origin, target);

        StartCoroutine(CFill(lengthGoal, fillTime));
    }

    private IEnumerator CFill(float lengthGoal, float fillTime)
    {
        float fillLength = 0f;
        Vector2 newSize = fill.rectTransform.sizeDelta;
        while (lengthGoal != fillLength)
        {
            fillLength = Mathf.Min(lengthGoal, fillLength + lengthGoal / fillTime * Time.deltaTime);
            newSize.x = fillLength * 100f;
            fill.rectTransform.sizeDelta = newSize;
            yield return null;
        }
        isFilling = false;
        SetActive(false);
    }
}
