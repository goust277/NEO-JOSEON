using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Image;


/* 
 * 월드 스페이스 캔버스
 * 픽셀 퍼 유닛 100
 * X 회전 값 901 기준 작성됨.
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

        // 위치 변경
        transform.localPosition = origin;

        Vector3 dirVector = target - origin;
        float lengthGoal = dirVector.magnitude;

        // 아웃라인 길이 설정
        Vector2 newSize = outline.rectTransform.sizeDelta;
        newSize.x = lengthGoal * 100f;
        outline.rectTransform.sizeDelta = newSize;

        // 내부 길이 초기화
        newSize.x = 0f;
        fill.rectTransform.sizeDelta = newSize;

        // 방향 지정
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
