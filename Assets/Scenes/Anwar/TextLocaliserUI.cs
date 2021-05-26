
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TextLocaliserUI : MonoBehaviour
{
    Text textField;

    public string key;

    private LocalisationSystem.Language language;


    // Start is called before the first frame update
    void Start()
    {
        textField = GetComponent<Text>(); //change to ours
        string value = LocalisationSystem.GetLocalisedValue(key);
        textField.text = value;
        Debug.Log(value);
        language = LocalisationSystem.language;
    }

    // Update is called once per frame
    void Update()
    {
        if(language != LocalisationSystem.language)
        {
            string value = LocalisationSystem.GetLocalisedValue(key);
            language = LocalisationSystem.language;
            textField.text = value;
        }
    }
}


