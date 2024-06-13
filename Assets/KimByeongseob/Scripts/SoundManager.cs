using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // ���� Ŭ�� ���
    public AudioClip[] audioClips;

    // ���� Ŭ�� ��ųʸ�
    private Dictionary<string, AudioClip> soundClips;

    // ����� �ҽ�
    [SerializeField] private AudioSource audioSource;

    void Awake()
    {
        // ���� Ŭ�� ��ųʸ� �ʱ�ȭ
        soundClips = new Dictionary<string, AudioClip>();
        foreach (AudioClip clip in audioClips)
        {
            soundClips[clip.name] = clip;
        }

        // AudioSource ������Ʈ �߰�
    }

    // ���� ��� �޼���
    public void PlaySound(string clipName)
    {
        if (soundClips.ContainsKey(clipName))
        {
            audioSource.PlayOneShot(soundClips[clipName]);
        }
        else
        {
            Debug.LogWarning("Sound not found: " + clipName);
        }
    }

    // ���� �ݺ� ��� ���� �޼���
    public void StartLoopSound(string clipName)
    {
        if (soundClips.ContainsKey(clipName))
        {
            audioSource.clip = soundClips[clipName];
            audioSource.loop = true;
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("Sound not found: " + clipName);
        }
    }

    // ���� �ݺ� ��� ���� �޼���
    public void StopLoopSound(string clipName)
    {
        if (audioSource.clip != null && audioSource.clip.name == clipName && audioSource.loop)
        {
            audioSource.Stop();
            audioSource.loop = false;
        }
    }
}
