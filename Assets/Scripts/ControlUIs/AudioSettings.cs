using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DefinitiveScript;

public class AudioSettings : MonoBehaviour
{
    private VolumeController m_VolumeController;
    public VolumeController VolumeController
    {
        get {
            if(m_VolumeController == null) m_VolumeController = GameManager.Instance.SceneController.GetComponent<VolumeController>();
            return m_VolumeController;
        }
    }

    public Slider masterSlider;
    public Slider musicSlider;
    public Slider soundEffectSlider;

    // Start is called before the first frame update
    void Start()
    {
        masterSlider.value = VolumeController.GetMasterVolume();
        musicSlider.value = VolumeController.GetMusicVolume();
        soundEffectSlider.value = VolumeController.GetSoundEffectVolume();
    }

    public void OnMasterSliderChange()
    {
        VolumeController.SetMasterVolume(masterSlider.value);
    }

    public void OnMusicSliderChange()
    {
        VolumeController.SetMusicVolume(musicSlider.value);
    }

    public void OnSoundEffectSliderChange()
    {
        VolumeController.SetSoundEffectVolume(soundEffectSlider.value);
    }
}
