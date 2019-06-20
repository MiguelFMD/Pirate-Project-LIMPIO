using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimonSaysSoundController : PuzleSoundController
{
    public AudioClip[] simonClips; //Rojo, azul, verde, amarillo
    public AudioClip wrongSound;

    public void PlaySimonSound(int i)
    {
        soundEffectSource.clip = simonClips[i];
        soundEffectSource.Play();
    }

    public void PlayWrongSound()
    {
        soundEffectSource.clip = wrongSound;
        soundEffectSource.Play();
    }
}
