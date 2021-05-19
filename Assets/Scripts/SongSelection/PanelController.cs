using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelController : MonoBehaviour
{

    [SerializeField] private GameObject[] panels = new GameObject[0];
    Vector3 lastPosition;




    //�kar omkretsen ifall ny panel tillkommer.
    //Mellanrum mellan varje, de mpste ha ansiktet mot cylindern.
    //H�r kommer jag g�ra ett script som g�r en samling av alla paneler
    
    void Start()
    {

        panels = GameObject.FindGameObjectsWithTag("Panels");

    }


    void Update()
    {

        foreach (GameObject panel in panels) {
            panel.transform.position += (transform.position - lastPosition);
        }

        lastPosition = transform.position;
    }
}
