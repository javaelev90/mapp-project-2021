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
            Database.Instance.settingsRepository.UpdateLanguageSetting(LocalisationSystem.Language.English.ToString());
        }
        else if(chooseLanguage.ToLower() == LocalisationSystem.Language.Swedish.ToString().ToLower())
        {
            LocalisationSystem.language = LocalisationSystem.Language.Swedish;
            Database.Instance.settingsRepository.UpdateLanguageSetting(LocalisationSystem.Language.Swedish.ToString());
        }
    }
   
}
