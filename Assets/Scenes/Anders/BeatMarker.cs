using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class BeatMarker : MonoBehaviour, IComparable<BeatMarker>, IPointerDownHandler, IPointerUpHandler
{
    public AudioClip soundEffect;
    public AudioSource audioSourceSFX;
    public AudioSource audioSourceMusic;
    RectTransform parent;
    public float beatTime;
    bool clickedOnMarker = false;
    bool playedSoundEffect = false;
    // Start is called before the first frame update
    void Start()
    {
        parent = GetComponentInParent<RectTransform>();
        transform.position = parent.position;
    }

    // Update is called once per frame
    void Update()
    {  
        if(clickedOnMarker)
        {
            Vector3 mousePosition = Mouse.current.position.ReadValue();
            Vector2 newPosition = new Vector2(mousePosition.x, transform.position.y);
            
            transform.position = newPosition;

        }

        // Time interval for the SFX to be played, can't use equals since the exact time might be missed
        if(!playedSoundEffect && audioSourceMusic.time > beatTime - 0.01f && audioSourceMusic.time < beatTime + 0.02f)
        {
            audioSourceSFX.PlayOneShot(soundEffect);
            playedSoundEffect = true;
        }

        // If the track has been rewinded the SFX should be played again
        if(audioSourceMusic.time < beatTime - 0.01f)
        {
            playedSoundEffect = false;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(Keyboard.current.ctrlKey.isPressed){
            gameObject.SetActive(false);
        } 
        else {
            clickedOnMarker = true;
        }

    }
    public void OnPointerUp(PointerEventData eventData)
    {
        clickedOnMarker = false;
    }
    public int CompareTo(BeatMarker other)
    {

        if(beatTime > other.beatTime) 
        {
            return 1;
        } 
        else if (beatTime < other.beatTime)
        {
            return -1;
        } 
        else
        {
            return 0;
        }
    }

}
