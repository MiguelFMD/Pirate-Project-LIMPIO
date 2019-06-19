using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualPuzleSoundController : PuzleSoundController
{
    public AudioClip wrongSound;
    public AudioClip selectSound;

    public void PlayWrongSound()
    {
        soundEffectSource.clip = wrongSound;
        soundEffectSource.Play();
    }

    public void PlaySelectSound()
    {
        soundEffectSource.clip = selectSound;
        soundEffectSource.Play();
    }
}
