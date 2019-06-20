using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzleSoundController : MonoBehaviour
{
    public AudioSource soundEffectSource;
    
    public AudioClip successSound;

    public void PlaySuccessSound()
    {
        soundEffectSource.clip = successSound;
        soundEffectSource.Play();
    }
}
