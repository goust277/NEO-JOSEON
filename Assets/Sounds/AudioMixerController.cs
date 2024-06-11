using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioMixerController : MonoBehaviour
{
    public AudioMixer mixer;
    public Slider BGM;
    public Slider SFX;
    float sound;
    float SFXsound;
    private void Awake()
    {
        BGM.value = GameData.BGMsound;
        SFX.value = GameData.SFXSound;
    }
    public void AudioControl()
    {
        sound = BGM.value;
        GameData.BGMsound = BGM.value;
        SFXsound = SFX.value;
        GameData.SFXSound = SFX.value;

        if (sound == -40f) mixer.SetFloat("BGM", -80);
        else mixer.SetFloat("BGM", sound);

        if (sound == -40f) mixer.SetFloat("SFX", -80);
        else mixer.SetFloat("SFX", SFXsound);
    }
}