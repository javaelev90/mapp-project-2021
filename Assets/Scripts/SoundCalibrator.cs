using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class SoundCalibrator : MonoBehaviour
{

    [SerializeField] SongObject calibrationSong;
    [SerializeField] AudioSource audioSource;
    [SerializeField] GameObject instructionPanel;
    [SerializeField] Button startButton;
    [SerializeField] Button calibrationButton;
    [SerializeField] GameObject resultPanel;
    [SerializeField] GameObject startScreen;

    float audioStartTime;

    bool startCalibration;

    List<float> buttonClicks;

    void Start()
    {
        calibrationButton.interactable = false;
    }

    void Update()
    {
        if(startCalibration)
        {

            if(!audioSource.isPlaying)
            {
                ToggleCalibrationButton(false);
                startCalibration = false;
                float delay = CalculateDelay();
                if(delay == -1)
                {
                    SetResultScreenText("You clicked too few times during the test.");
                }
                else
                {
                    SetResultScreenText("You have a sound latency of " + Mathf.Round(delay * 1000) + " ms.");
                }
                ToggleResultsPanel(true);
                calibrationSong.audioDelay = delay;
                Debug.Log("Median latency: " + delay);
            }
        }
    }

    public void StartCalibration()
    {
        buttonClicks = new List<float>();
        if(calibrationSong != null)
        {
            ToggleInstructionScreen(false);
            ToggleCalibrationButton(true);
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

    void ToggleInstructionScreen(bool active)
    {
        instructionPanel.SetActive(active);
    }

    void ToggleCalibrationButton(bool active)
    {
        calibrationButton.interactable = active;
    }

    void ToggleResultsPanel(bool active)
    {
        resultPanel.SetActive(active);
    }

    void SetResultScreenText(string resultText)
    {
        resultPanel.GetComponentInChildren<TMP_Text>().text = resultText;

    }

    float CalculateDelay()
    {
        if(buttonClicks.Count < 3)
        {
            return -1f;
        }

        List<float> clickTimes = TransFormClickTimeToSongTime();
        clickTimes = clickTimes.GetRange(2, clickTimes.Count - 2);
        
        List<float> beatTimes = calibrationSong.beats;
        beatTimes = beatTimes.GetRange(2, beatTimes.Count - 2);

        List<float> timeDifferences = new List<float>();

        for(int i = 0; i < beatTimes.Count; i++)
        {
            float beatTime = beatTimes[i];
            float clickTime = clickTimes[i];
            float timeDifference = Mathf.Abs(beatTime - clickTime);
            timeDifferences.Add(timeDifference);
        }
        clickTimes.ForEach(t => Debug.Log(t));
        timeDifferences.Sort();
        Debug.Log(timeDifferences.Count);
        return timeDifferences.Count/2 > 1 ? timeDifferences[timeDifferences.Count/2] : -1f;
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

    public void LoadGame(int sceneId){
        SceneManager.LoadScene(sceneId);
    }

    public void RestartCalibration()
    {
        ToggleInstructionScreen(true);
        ToggleCalibrationButton(false);
        ToggleResultsPanel(false);
    }

    public void ToggleStartScreen(bool active)
    {
        startScreen.SetActive(active);
    }

    public void RemoveCalibrationResult()
    {
        //TODO: add this functionality
    }
}
