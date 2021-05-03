using System.Collections;
using System.Collections.Generic;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.SceneManagement;

public class AudioVisualizer : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] TMP_Text musicTime;
    [SerializeField] TMP_Text songObjectName;
    [SerializeField] BeatMarker beatMarker;
    [Header("Song Object to Edit")]
    [SerializeField] SongObject songObject;
    [Header("")]
    [SerializeField] Color waveFormColor;
    [SerializeField] AudioSource audioSourceMusic;
    [SerializeField] AudioSource audioSourceSFX;

    RawImage audioRenderer;
    RectTransform waveFormTransform;
    Vector2 originalWaveFormPosition;

    Vector2 initialMousePosition;
    Vector2 initialClickedWaveFormPosition;
    bool clickHoldingOnWaveForm = false;
    bool addedMarkerOnClick = false;
    public bool isPaused = false;

    List<BeatMarker> markers = new List<BeatMarker>();
    SceneHandler sceneHandler;

    void Awake() {
        // Load the Managers scen
        if (!SceneManager.GetSceneByName("Managers").isLoaded)
            SceneManager.LoadSceneAsync("Managers", LoadSceneMode.Additive);
    }
    void Start()
    {
        audioSourceMusic.clip = songObject.song;
        SongHandler.Instance.SetSongObject(songObject);
        audioRenderer = GetComponent<RawImage>();
        waveFormTransform = audioRenderer.GetComponent<RectTransform>();

        RectTransform parentRect = GetComponentInParent<RectTransform>();
        Texture2D texture = WaveFormUtil.PaintWaveformSpectrum(WaveFormUtil.GetWaveform(songObject.song, 16384, 1f), 500, waveFormColor);

        waveFormTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, texture.width);
        waveFormTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, texture.height);

        audioRenderer.texture = texture;
        waveFormTransform.position = parentRect.position;
        originalWaveFormPosition = waveFormTransform.position;

        LoadSongObjectMapping();
        DisplaySongObjectName();
        sceneHandler = SceneHandler.Instance;

        ResumeEditWork();
        audioSourceMusic.Play();
        Pause();
    }
    void Update()
    {   
        UpdateWaveFormPosition();
        AddNewBeatMarker();
        UpdateMarkers();
        UpdateMusicTimePanel();
    }

    private void ResumeEditWork()
    {
        if(PlayerPrefs.HasKey("IsEditing") && PlayerPrefs.GetInt("IsEditing") == 1)
        {
            float musicStartTime = PlayerPrefs.GetFloat("MusicStartTime");
            audioSourceMusic.Play();
            Pause();
            audioSourceMusic.time = musicStartTime;

            float timePercentageOfClip = audioSourceMusic.time / songObject.song.length;
            float newX = timePercentageOfClip * waveFormTransform.rect.width;
            waveFormTransform.position = new Vector2(originalWaveFormPosition.x-newX, waveFormTransform.position.y); 
            PlayerPrefs.SetInt("IsEditing", 0);
        }
    }

    private void DisplaySongObjectName()
    {
        string assetPath = AssetDatabase.GetAssetPath(songObject.GetInstanceID());
        string fileName = System.IO.Path.GetFileNameWithoutExtension(assetPath);
        songObjectName.text = "<color=#000000>Song Object:</color> <color=#772fde>" + fileName + "</color>";
    }

    public void ExportMappingToSongObject()
    {
        songObject.ClearBeats();
        markers.Sort();
        foreach(BeatMarker marker in markers)
        {
            songObject.AddBeat(marker.beatTime);
        }
    }

    private void AddNewBeatMarker()
    {
        if(Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            AddBeatMaker(audioSourceMusic.time, originalWaveFormPosition);
        } 
        else if(Keyboard.current.altKey.isPressed && clickHoldingOnWaveForm && !addedMarkerOnClick)
        {
            float xValue = waveFormTransform.transform.InverseTransformPoint(initialMousePosition).x;
            float xPercentageOfWaveFormWidth = Mathf.Abs(xValue) / waveFormTransform.rect.width;
            float newTime = songObject.song.length * xPercentageOfWaveFormWidth;

            AddBeatMaker(newTime, initialMousePosition);
            addedMarkerOnClick = true;
        }
    }
    private void UpdateMusicTimePanel()
    {
        // Round text to 3 decimal places
        musicTime.text = "" + Math.Round(audioSourceMusic.time, 3);
    }
    private void UpdateWaveFormPosition()
    {
        // Can move the waveform with mouse when clicking and holding
        if(isPaused && clickHoldingOnWaveForm)
        {
            Vector3 mousePosition = Mouse.current.position.ReadValue();
            Vector2 newPosition = new Vector2(initialClickedWaveFormPosition.x - (initialMousePosition.x - mousePosition.x), waveFormTransform.position.y);
            waveFormTransform.position = newPosition;
            
            if(waveFormTransform.localPosition.x> 0)
            {
                waveFormTransform.position = new Vector2(originalWaveFormPosition.x, originalWaveFormPosition.y);
            } 
            else if(waveFormTransform.rect.width + waveFormTransform.localPosition.x + 0.1f < 0)
            {
                waveFormTransform.position = new Vector2(originalWaveFormPosition.x-waveFormTransform.rect.width+0.1f, originalWaveFormPosition.y);
            }

            float xPercentageOfWaveFormWidth = Mathf.Abs(waveFormTransform.localPosition.x) / waveFormTransform.rect.width;
            float newTime = songObject.song.length * xPercentageOfWaveFormWidth;
            audioSourceMusic.time = newTime;
        } 
        // The waveform will move left with the music
        else if(!isPaused)
        {
            float timePercentageOfClip = audioSourceMusic.time / songObject.song.length;
            float newX = timePercentageOfClip * waveFormTransform.rect.width;
            waveFormTransform.position = new Vector2(originalWaveFormPosition.x-newX, waveFormTransform.position.y); 
        }
    }
    private void UpdateMarkers()
    {
        for(int i = 0; i < markers.Count; i++)
        {
            BeatMarker marker = markers[i];
            
            // For performance only update markers if they have changed
            if(!marker.HasChanged()){
                continue;
            }

            // Destory markers that have been inactivated
            if(!marker.gameObject.activeSelf)
            {
                markers.Remove(marker);
                Destroy(marker);
                i--;
            }
            // Update position of moved markers
            else
            {
                Vector2 localPosition = marker.transform.localPosition;
                float beatTime = (localPosition.x / waveFormTransform.rect.width) * songObject.song.length;
                marker.beatTime = beatTime;
                marker.AcknowledgeChange();
            }
        }
    }
    private void AddBeatMaker(float newBeatTime, Vector2 startPosition)
    {
        BeatMarker marker = Instantiate<BeatMarker>(beatMarker, new Vector3(startPosition.x, waveFormTransform.position.y, 0f), Quaternion.identity, transform);
        marker.audioSourceSFX = audioSourceSFX;
        marker.audioSourceMusic = audioSourceMusic;
        marker.soundEffect = songObject.sfx;
        marker.beatTime = newBeatTime;
        markers.Add(marker);
    }
    private void LoadSongObjectMapping()
    {
        foreach(float beat in songObject.GetBeats())
        {
            float timePercentageOfClip = beat / songObject.song.length;
            float xPos = timePercentageOfClip * waveFormTransform.rect.width;
            AddBeatMaker(beat, new Vector2(originalWaveFormPosition.x + xPos, originalWaveFormPosition.y));
        }  
    }

    public void Pause()
    {
        isPaused = true;
        audioSourceMusic.Pause();
    }

    public void UnPause()
    {
        if(!isPaused && !audioSourceMusic.isPlaying)
        {
            audioSourceMusic.Play();
        }
        else
        {
            isPaused = false;
            audioSourceMusic.UnPause();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        clickHoldingOnWaveForm = true;
        addedMarkerOnClick = false;
        initialMousePosition = Mouse.current.position.ReadValue();
        initialClickedWaveFormPosition = transform.position;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        clickHoldingOnWaveForm = false;
    }

    public void SetGameAudioTime(float time)
    {
        PlayerPrefs.SetFloat("MusicStartTime", time != -1 ? time : audioSourceMusic.time);
    }
    public void ChangeScene(string sceneName)
    {
        ExportMappingToSongObject();
        PlayerPrefs.SetInt("IsEditing", 1);
        sceneHandler.ChangeScene(sceneName);
    }

}
