
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class LocalisationSystem

{

    public enum Language
    {
        English,
        Swedish
    }

    public static Language language = Language.Swedish;

    private static Dictionary<string, string> localisedEN;
    private static Dictionary<string, string> localisedSV;

    public static bool isInit;

    public static CSVLoader csvLoader;

    public static void Init()
    {
        csvLoader = new CSVLoader();
        csvLoader.LoadCSV();

        UpdateDictionaires();
        // Get saved language setting from database
        SetLanguage(Database.Instance.settingsRepository.GetLanguageSetting());
        isInit = true;
    }

    private static void SetLanguage(string language)
    {
        if(language == null) return;
        if(language.Equals("")) return;

        if(language.ToLower() == LocalisationSystem.Language.English.ToString().ToLower())
        {
            LocalisationSystem.language = LocalisationSystem.Language.English;
        }
        else if(language.ToLower() == LocalisationSystem.Language.Swedish.ToString().ToLower())
        {
            LocalisationSystem.language = LocalisationSystem.Language.Swedish;
        }

    }

    public static void UpdateDictionaires()
    {

        localisedEN = csvLoader.GetDictionaryValues("ENGLISH");
        localisedSV = csvLoader.GetDictionaryValues("SWEDISH");
       
    }

    public static string GetLocalisedValue(string key)
    {
        if(!isInit) { Init(); }

        string value = key;

        switch (language)
        {
            case Language.English:
                localisedEN.TryGetValue(key, out value);
                break;
            case Language.Swedish:
                localisedSV.TryGetValue(key, out value);
                break;
        }

        return value;
    }
#if UNITY_EDITOR 
    public static void Add(string key, string value)
    {
        if (value.Contains("\""))
        {
            value.Replace('"', '\"');
        }

       if(csvLoader == null)
        {
            csvLoader = new CSVLoader();
        }

        csvLoader.LoadCSV();
        csvLoader.Add(key, value);
        csvLoader.LoadCSV();

        UpdateDictionaires();
    }

    public static void Replace(string key, string value)
    {
        if (value.Contains("\""))
        {
            value.Replace('"', '\"');
        }

        if (csvLoader == null)
        {
            csvLoader = new CSVLoader();
        }

        csvLoader.LoadCSV();
        csvLoader.Edit(key, value);
        csvLoader.LoadCSV();

        UpdateDictionaires();
    }

    public static void Remove(string key)
    {
        if (csvLoader == null)
        {
            csvLoader = new CSVLoader();
        }

        csvLoader.LoadCSV();
        csvLoader.Remove(key );
        csvLoader.LoadCSV();

        UpdateDictionaires();
    }
#endif

}

