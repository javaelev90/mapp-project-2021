using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundCalibrator : MonoBehaviour
{

    [SerializeField] SongObject calibrationSong;
    [SerializeField] AudioSource audioSource;
    [SerializeField] GameObject instructionPanel;
    [SerializeField] Button startButton;
    [SerializeField] Button calibrationButton;

    float audioStartTime;

    bool startCalibration;

    List<float> buttonClicks;

    void Start()
    {
        buttonClicks = new List<float>();
        calibrationButton.interactable = false;
    }

    void Update()
    {
        if(startCalibration)
        {

            if(!audioSource.isPlaying)
            {
                ToggleCalibrationButton();
                startCalibration = false;
                CalculateDelay();
            }
        }
    }

    public void StartCalibration()
    {
        
        if(calibrationSong != null)
        {
            ToggleInstructionScreen();
            ToggleCalibrationButton();
            audioSource.clip = calibrationSong.song;
            audioSource.Play();
            audioStartTime = Time.time;
            startCalibration = true;
        }
        else 
        {
            Debug.Log("No calibration song selected!");
        }
    }

    public void ClickOnCalibrationButton()
    {
        buttonClicks.Add(Time.time);
    }

    void ToggleInstructionScreen()
    {
        instructionPanel.SetActive(!instructionPanel.activeSelf);
    }

    void ToggleCalibrationButton()
    {
        calibrationButton.interactable = !calibrationButton.interactable;
    }

    void CalculateDelay()
    {
        List<float> clickTimes = TransFormClickTimeToSongTime();
        clickTimes.ForEach(t => Debug.Log(t));
    }

    List<float> TransFormClickTimeToSongTime()
    {
        List<float> transformedTimes = new List<float>();
        foreach(float clickTime in buttonClicks)
        {
            float transformedTime = clickTime - audioStartTime;
            transformedTimes.Add(transformedTime);
        }
        return transformedTimes;
    }

}
