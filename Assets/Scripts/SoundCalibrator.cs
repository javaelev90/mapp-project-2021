using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class SoundCalibrator : MonoBehaviour
{
    int MIN_CALIBRATION_CLICKS = 4;
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
                float audioLatency = CalculateAudioLatency();
                if(audioLatency == -1)
                {
                    SetResultScreenText("You clicked too few times during the test.");
                }
                else
                {
                    SetResultScreenText("You have a sound latency of " + Mathf.Round(audioLatency * 1000) + " ms.");
                }
                ToggleResultsPanel(true);
                // TODO: Change this to be saved on a persistent place
                // calibrationSong.audioDelay = delay;
                
                Debug.Log("Median latency: " + audioLatency);
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

    float CalculateAudioLatency()
    {
         if(buttonClicks.Count < MIN_CALIBRATION_CLICKS)
        {
            return -1f;
        }

        List<float> clickTimes = TransFormClickTimeToSongTime();
        List<float> beatTimes = calibrationSong.beats;
        Dictionary<float, float> beatsAndClicks = FindClicksCloseToBeats(clickTimes, beatTimes);
        List<float> timeDifferences = new List<float>();

        foreach (KeyValuePair<float, float> times in beatsAndClicks)
        {
            float timeDifference = Mathf.Abs(times.Key - times.Value);
            timeDifferences.Add(timeDifference);
        }

        timeDifferences.Sort();

        return (timeDifferences.Count/2) > 1 ? timeDifferences[timeDifferences.Count/2] : -1f;
    }

    Dictionary<float, float> FindClicksCloseToBeats(List<float> clicks, List<float> beats)
    {
        Dictionary<float, float> closeClicks = new Dictionary<float, float>();
        float MAX_DIFFERENCE = 0.45f;

        // Loop through all beats and find clicks that are close in time to them
        foreach(float beat in beats)
        {
            float closestClick = float.MaxValue;
            float smallestDifference = float.MaxValue;
            
            foreach(float click in clicks)
            {
                float clickBeatDifference = Mathf.Abs(beat - click);
                bool clickBeforeBeat = (beat - click) > 0.1f;
                
                if(smallestDifference > clickBeatDifference && !clickBeforeBeat)
                {
                    closestClick = click;
                    smallestDifference = clickBeatDifference;
                }
            }
            // If the click is not close to the beat then it is not related
            if(smallestDifference < MAX_DIFFERENCE)
            {
                closeClicks.Add(beat, closestClick);
            }
        }

        return closeClicks;
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
        StopCalibration();
    }

    public void StopCalibration()
    {
        audioSource.Stop();
        startCalibration = false;
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
