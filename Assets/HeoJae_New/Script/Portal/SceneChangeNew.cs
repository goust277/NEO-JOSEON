using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChangeNew : MonoBehaviour
{
    [SerializeField] private string SceneName;

    public Image FadInOut;
    public bool bBoss;

    private void Awake()
    {
        if(!bBoss)
        {
            FadInOut.color = new Color(FadInOut.color.r, FadInOut.color.g, FadInOut.color.b, 1f);
            FadeOut(2f);
        }
    }

    private void Update()
    {
        if (FadInOut.color.a == 0f)
        {
            FadInOut.gameObject.SetActive(false);
        }
        else
        {
            FadInOut.gameObject.SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            other.GetComponent<PlayerMove>().onTxt = true;
            FadeIn(2f);
        }
    }

    public void FadeIn(float duration)
    {
        StartCoroutine(Fade(FadInOut, 0f, 1f, duration, () =>
        {
            SceneManager.LoadScene(SceneName);
        }));
    }

    public void FadeOut(float duration)
    {
        StartCoroutine(Fade(FadInOut, 1f, 0f, duration, null));
    }

    IEnumerator Fade(Image image, float startAlpha, float targetAlpha, float duration, System.Action onComplete)
    {
        float startTime = Time.time;
        Color color = image.color;

        while (Time.time < startTime + duration)
        {
            float t = (Time.time - startTime) / duration;
            color.a = Mathf.Lerp(startAlpha, targetAlpha, t);
            image.color = color;
            yield return null;
        }

        color.a = targetAlpha;
        image.color = color;

        // 페이드가 완료되면 콜백을 호출합니다.
        onComplete?.Invoke();
    }
}
