using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // 사운드 클립 목록
    public AudioClip[] audioClips;

    // 사운드 클립 딕셔너리
    private Dictionary<string, AudioClip> soundClips;

    // 오디오 소스
    [SerializeField] private AudioSource audioSource;

    void Awake()
    {
        // 사운드 클립 딕셔너리 초기화
        soundClips = new Dictionary<string, AudioClip>();
        foreach (AudioClip clip in audioClips)
        {
            soundClips[clip.name] = clip;
        }

        // AudioSource 컴포넌트 추가
    }

    // 사운드 재생 메서드
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

    // 사운드 반복 재생 시작 메서드
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

    // 사운드 반복 재생 중지 메서드
    public void StopLoopSound(string clipName)
    {
        if (audioSource.clip != null && audioSource.clip.name == clipName && audioSource.loop)
        {
            audioSource.Stop();
            audioSource.loop = false;
        }
    }
}
