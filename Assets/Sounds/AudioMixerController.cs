using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioMixerController : MonoBehaviour
{
    [SerializeField] private AudioMixer m_AudioMixer;
    [SerializeField] private Slider m_MusicBGMSlider;
    [SerializeField] private Slider m_MusicSFXSlider;

    public void AudioControl()
    {
        float sound = m_MusicSFXSlider.value;


        if (sound == -40f) m_AudioMixer.SetFloat("SFX", -80);
        else m_AudioMixer.SetFloat("SFX", sound);

    }
}