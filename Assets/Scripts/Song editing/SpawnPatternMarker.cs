using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SpawnPatternMarker : MonoBehaviour, IComparable<SpawnPatternMarker>, IPointerDownHandler, IPointerUpHandler
{
    RectTransform parent;
    // public float changeTime;
    public SpawnPatternChange spawnPatternChange;
    [SerializeField] RawImage pinImage;
    [SerializeField] RawImage pinHeadImage;
    [SerializeField] RawImage pinFootImage;
    bool clickedOnMarker = false;
    bool changedPosition = false;
    void Start()
    {
        parent = transform.parent.GetComponentInParent<RectTransform>();
    }

    public void Initialize()
    {
        spawnPatternChange = new SpawnPatternChange();
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
    }

    public void ChangeColor(Color color)
    {
        pinHeadImage.color = color;
        pinFootImage.color = color;
        pinImage.color = color;
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

    public int CompareTo(SpawnPatternMarker other)
    {

        if(spawnPatternChange.activationTime > other.spawnPatternChange.activationTime) 
        {
            return 1;
        } 
        else if (spawnPatternChange.activationTime < other.spawnPatternChange.activationTime)
        {
            return -1;
        } 
        else
        {
            return 0;
        }
    }

}
