using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeMenu : MonoBehaviour
{

    public AudioMixer audioMixer;
    public Slider musicSlider;
    public Slider effectSlider;

    private void Start()
    {
        float musicVolume = Database.Instance.settingsRepository.GetVolumeSetting();
        audioMixer.SetFloat("musicVolume", musicVolume);
        musicSlider.value = musicVolume;

    }

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("musicVolume", volume);
        Database.Instance.settingsRepository.UpdateVolumeSetting(volume);
        
    }

    public void SetEffectsVolume(float volume)
    {
        audioMixer.SetFloat("effectsVolume", volume);

    }
}
