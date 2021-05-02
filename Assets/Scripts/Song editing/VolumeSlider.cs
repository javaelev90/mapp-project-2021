using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{

    [SerializeField] AudioSource audioSource;
    Slider volumeSlider;
    void Start()
    {
        volumeSlider = GetComponent<Slider>();
    }
    void Update()
    {
        audioSource.volume = volumeSlider.value;
    }
}
