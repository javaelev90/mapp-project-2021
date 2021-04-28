using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class VolumeMenu : MonoBehaviour
{
    public AudioMixer audioMixer;

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("musicVolume", volume);
        
    }

    public void SetEffectsVolume(float volume)
    {
        audioMixer.SetFloat("effectsVolume", volume);
    }
}
