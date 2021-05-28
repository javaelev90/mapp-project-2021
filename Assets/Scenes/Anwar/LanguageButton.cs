using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class LanguageButton : MonoBehaviour
{
    

    public void ChangeLanguage(string chooseLanguage)
    {
        if(chooseLanguage.ToLower() == LocalisationSystem.Language.English.ToString().ToLower())
        {
            LocalisationSystem.language = LocalisationSystem.Language.English;
        }
        else if(chooseLanguage.ToLower() == LocalisationSystem.Language.Swedish.ToString().ToLower())
        {
            LocalisationSystem.language = LocalisationSystem.Language.Swedish;
        }
    }
   
}
