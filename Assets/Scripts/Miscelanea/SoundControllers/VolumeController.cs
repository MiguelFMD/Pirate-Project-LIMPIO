using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class VolumeController : MonoBehaviour
{
    public AudioMixer generalAudioMixer;

    public float maxVolume = 10f;
    public float minVolume = -80f;

    public float GetMasterVolume()
    {
        float value = 0f;
        generalAudioMixer.GetFloat("masterVolume", out value);
        return Mathf.InverseLerp(minVolume, maxVolume, value);
    }

    public float GetMusicVolume()
    {
        float value = 0f;
        generalAudioMixer.GetFloat("musicVolume", out value);
        return Mathf.InverseLerp(minVolume, maxVolume, value);
    }
    
    public float GetSoundEffectVolume()
    {
        float value = 0f;
        generalAudioMixer.GetFloat("seVolume", out value);
        return Mathf.InverseLerp(minVolume, maxVolume, value);
    }

    public void SetMasterVolume(float value)
    {
        float volume = Mathf.Lerp(minVolume, maxVolume, value);
        generalAudioMixer.SetFloat("masterVolume", volume);
    }

    public void SetMusicVolume(float value)
    {
        float volume = Mathf.Lerp(minVolume, maxVolume, value);
        generalAudioMixer.SetFloat("musicVolume", volume);
    }

    public void SetSoundEffectVolume(float value)
    {
        float volume = Mathf.Lerp(minVolume, maxVolume, value);
        generalAudioMixer.SetFloat("seVolume", volume);
    }
}
