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
    private void Start()
    {
        BGM.value = GameData.BGMsound;
        SFX.value = GameData.SFXSound;
        Debug.Log("SFX value set to: " + SFX.value);
        Debug.Log("BGM value set to: " + BGM.value);

        GameData.LoadData();
    }
    public void AudioControl()
    {
        sound = BGM.value;
        GameData.BGMsound = BGM.value;
        SFXsound = SFX.value;
        GameData.SFXSound = SFX.value;

        if (sound == -40f) mixer.SetFloat("BGM", -80);
        else mixer.SetFloat("BGM", sound);

        if (SFXsound == -40f) mixer.SetFloat("SFX", -80);
        else mixer.SetFloat("SFX", SFXsound);
    }
}