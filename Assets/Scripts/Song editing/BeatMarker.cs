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

    bool changedPosition = false;
    void Start()
    {
        parent = transform.parent.GetComponentInParent<RectTransform>();
    }

    void Update()
    {  
        if(clickedOnMarker)
        {
            Vector3 mousePosition = Mouse.current.position.ReadValue();
            Vector2 newPosition = new Vector2(mousePosition.x, transform.position.y);
            
            transform.position = newPosition;

            if(transform.localPosition.x <= 0f)
            {
                transform.localPosition = new Vector2(0f, transform.localPosition.y);
            } 
            else if(transform.localPosition.x >= parent.rect.width)
            {
                transform.localPosition = new Vector2(parent.rect.width, transform.localPosition.y);
            }

            changedPosition = true;
        }
        
        if(soundEffect != null){
            // If music time has passed beat time it should play SFX once, can be music time == beat time because it might be missed
            if(!playedSoundEffect && audioSourceMusic.time > beatTime && audioSourceMusic.isPlaying)
            {
                audioSourceSFX.PlayOneShot(soundEffect);
                playedSoundEffect = true;
            }

            // If the track has been rewinded the SFX should be played again
            if(audioSourceMusic.time < beatTime)
            {
                playedSoundEffect = false;
            }
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

    public bool HasChanged()
    {
        return changedPosition || !gameObject.activeSelf;
    }

    public void AcknowledgeChange()
    {
        changedPosition = false;
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
