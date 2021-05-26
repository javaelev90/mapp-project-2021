using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LanguageButton : MonoBehaviour
{
    

    public void ChangeLanguage()
    {
        if(LocalisationSystem.language == LocalisationSystem.Language.English)
        {
            LocalisationSystem.language = LocalisationSystem.Language.Swedish;
        }
        else
        {
            LocalisationSystem.language = LocalisationSystem.Language.English;
        }
    }
   
}
