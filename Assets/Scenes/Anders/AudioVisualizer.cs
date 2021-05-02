using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using TMPro;


public class AudioVisualizer : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] TMP_Text musicTime;
    [SerializeField] BeatMarker beatMarker;
    [SerializeField] SongObject songObject;
    [SerializeField] Color waveFormColor;
    [SerializeField] AudioSource audioSourceMusic;
    [SerializeField] AudioSource audioSourceSFX;

    RawImage audioRenderer;
    RectTransform waveFormTransform;
    Vector2 originalWaveFormPosition;

    Vector2 initialMousePosition;
    Vector2 initialClickedWaveFormPosition;
    bool clickedOnWaveForm = false;
    public bool isPaused = false;

    List<BeatMarker> markers = new List<BeatMarker>();

    // Start is called before the first frame update
    void Start()
    {
        audioSourceMusic.clip = songObject.song;
        audioRenderer = GetComponent<RawImage>();
        waveFormTransform = audioRenderer.GetComponent<RectTransform>();

        RectTransform parentRect = GetComponentInParent<RectTransform>();
        Texture2D texture = PaintWaveformSpectrum(GetWaveform(songObject.song, 16384, 1f), 500, waveFormColor);

        waveFormTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, texture.width);
        waveFormTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, texture.height);

        audioRenderer.texture = texture;
        waveFormTransform.position = parentRect.position;
        originalWaveFormPosition = waveFormTransform.position;

        LoadSongObjectMapping();

        // audioSourceMusic.Play();
    }
    // Update is called once per frame
    void Update()
    {   
        UpdateWaveFormPosition();
        AddNewBeatMarker();
        UpdateMarkers();
        UpdateMusicTimePanel();
        
    }

    public void ExportMappingToSongObject()
    {
        songObject.beats.Clear();
        markers.Sort();
        foreach(BeatMarker marker in markers)
        {
            songObject.beats.Add(marker.beatTime);
        }
    }

    private void AddNewBeatMarker()
    {
        if(Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            AddBeatMaker(audioSourceMusic.time, originalWaveFormPosition);
        }
    }
    private void UpdateMusicTimePanel()
    {
        musicTime.text = "" + audioSourceMusic.time;
    }
    private void UpdateWaveFormPosition()
    {
        if(isPaused && clickedOnWaveForm)
        {
            Vector3 mousePosition = Mouse.current.position.ReadValue();
            Vector2 newPosition = new Vector2(initialClickedWaveFormPosition.x - (initialMousePosition.x - mousePosition.x), waveFormTransform.position.y);
            waveFormTransform.position = newPosition;

            float xPercentageOfWaveFormWidth = Mathf.Abs(waveFormTransform.localPosition.x) / waveFormTransform.rect.width;
            float newTime = songObject.song.length * xPercentageOfWaveFormWidth;
            audioSourceMusic.time = newTime;

        } 
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

            // Destory markers that have been "removed"
            if(!marker.gameObject.activeSelf)
            {
                markers.Remove(marker);
                Destroy(marker);
                i--;
            } 
            else
            {
                Vector2 localPosition = marker.transform.localPosition;
                float beatTime = (localPosition.x / waveFormTransform.rect.width) * songObject.song.length;
                marker.beatTime = beatTime;

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
        foreach(float beat in songObject.beats)
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
        clickedOnWaveForm = true;
        initialMousePosition = Mouse.current.position.ReadValue();
        initialClickedWaveFormPosition = transform.position;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        clickedOnWaveForm = false;
    }

    public static float[] GetWaveform (AudioClip audio, int size, float sat) 
    {
        float[] samples = new float[audio.channels * audio.samples];
        float[] waveform = new float[size];
        audio.GetData(samples, 0);
        int packSize = audio.samples * audio.channels / size;
        float max = 0f;
        int c = 0;
        int s = 0;
        for (int i = 0; i < audio.channels * audio.samples; i++)
        {
            waveform[c] += Mathf.Abs(samples[i]);
            s++;
            if (s > packSize)
            {
                if (max < waveform[c])
                    max = waveform[c];
                c++;
                s = 0;
            }
        }
        for (int i = 0; i < size; i++)
        {
            waveform[i] /= (max * sat);
            if (waveform[i] > 1f)
                waveform[i] = 1f;
        }

        return waveform;
    }
 
    public static Texture2D PaintWaveformSpectrum(float[] waveform, int height, Color c) 
    {
        Texture2D tex = new Texture2D (waveform.Length, height, TextureFormat.RGBA32, false);

        for (int x = 0; x < waveform.Length; x++) {
            for (int y = 0; y <= waveform [x] * (float)height / 2f; y++) {
                tex.SetPixel (x, (height / 2) + y, c);
                tex.SetPixel (x, (height / 2) - y, c);
            }
        }
        tex.Apply ();

        return tex;
    }
}
