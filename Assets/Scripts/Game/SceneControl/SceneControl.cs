using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneControl : MonoBehaviour
{
    // ���̵���/�ƿ��� ���� �̹���
    private Image fadeImage = null;
    // ȭ�� ��ȯ ������ ����
    private bool isFading = false;

    private delegate void Callback();

    private void Awake()
    {
        GameObject prefab = Resources.Load<GameObject>("Prefab/System/ScreenEffect");
        GameObject newInst = Instantiate(prefab);
        DontDestroyOnLoad(newInst);

        fadeImage = newInst.GetComponentInChildren<Image>();
        fadeImage.rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);
        fadeImage.color = Vector4.zero;
    }

    private bool CanChangeScene()
    {
        if (isFading)
            return false;
        return true;
    }

    private IEnumerator CFadeOut(float sec, float delay, Callback next)
    {
        if (sec <= 0) sec = 0.1f;

        isFading = true;
        while (fadeImage.color.a < 1)
        {
            Vector4 newColor= fadeImage.color;
            newColor.w = Mathf.Min(1, fadeImage.color.a + 1 / sec * Time.deltaTime);
            fadeImage.color = newColor;
            yield return null;
        }
        yield return new WaitForSecondsRealtime(delay);
        if (next != null)
            next();
        else
            isFading = false;
    }

    private IEnumerator CFadeIn(float sec)
    {
        if (sec <= 0) sec = 0.1f;

        while (fadeImage.color.a > 0)
        {
            Vector4 newColor = fadeImage.color;
            newColor.w = Mathf.Max(0, fadeImage.color.a - 1 / sec * Time.deltaTime);
            fadeImage.color = newColor;
            yield return null;
        }
        isFading = false;
    }

    // name ������ ȿ�� ���� ��ȯ
    public void ChngeScene(string name)
    {
        if (!CanChangeScene())
            return;
        SceneManager.LoadScene(name);
    }

    // name ������ ��ȯ, sec1�ʿ� ����ŭ ���̵�ƿ�, delay��ŭ ���, sec2�ʿ� ���� ���̵���
    public void ChangeScene(string name, float sec1, float delay, float sec2)
    {
        if (!CanChangeScene())
            return;
        StartCoroutine(CFadeOut(sec1, delay, () =>
        {
            SceneManager.LoadScene(name);
            StartCoroutine(CFadeIn(sec2));
        } ));
    }

    // sec�ʿ� ���� ���̵�ƿ� (���̵� �� ����)
    public void FadeOut(float sec)
    {
        if (!CanChangeScene())
            return;
        StartCoroutine(CFadeOut(sec, 0, null));
    }

    // sec1�ʿ� ���� ���̵�ƿ�, delay��ŭ ���, sec2�ʿ� ���� ���̵���
    public void FadeOut(float sec1, float delay, float sec2)
    {
        if (!CanChangeScene())
            return;
        StartCoroutine(CFadeOut(sec1, delay, () =>
        {
            StartCoroutine(CFadeIn(sec2));
        }));
    }

    // sec�ʿ� ���� ���̵���
    public void FadeIn(float sec)
    {
        if (CanChangeScene())
            return;
        StartCoroutine(CFadeIn(sec));
    }
}
