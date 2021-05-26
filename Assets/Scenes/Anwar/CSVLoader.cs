
using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;
using System.Text;
using System;

public class CSVLoader
{
    private TextAsset csvFile;
    private string lineSeparator = "\r\n";
    private char surround = '"';
    private string[] fieldSeparator = { "\",\"" };
    private string contents;

    public void LoadCSV()
    {
        csvFile = Resources.Load<TextAsset>("translations_SC4");
        Debug.Log(csvFile);
        contents = csvFile.text;

    }

    public Dictionary<string, string> GetDictionaryValues(string attributeID)
    {
        Dictionary<string, string> dictionary = new Dictionary<string, string>();

        

        string[] lines = contents.Split(new string[] { lineSeparator }, StringSplitOptions.None);

        int attributeIndex = -1;

        string[] headers = lines[0].Split(fieldSeparator, System.StringSplitOptions.None);

        for (int i = 0; i < headers.Length; i++)
        {
            if (headers[i].Contains(attributeID))
            {
                attributeIndex = i;
                break;
            }
        }

        Regex CSVParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];

            string[] fields = CSVParser.Split(line);

            for (int f = 0; f < fields.Length; f++)
            {
                fields[f] = fields[f].TrimStart(' ', surround);
                fields[f] = fields[f].TrimEnd(surround);
                Debug.Log(fields[f]);
                
            }

            if (fields.Length > attributeIndex)
            {
                var key = fields[0];

                if (dictionary.ContainsKey(key)) { continue; }

                var value = fields[attributeIndex];

                dictionary.Add(key, value);
            }
        }

        return dictionary;
    }

//#if UNITY_EDITOR 
  
    public void Add(string key, string value)  // to be able to add values from our file
    {
        string appended = string.Format("\n\"{0}, \"{1}\",\"\"", key, value);
        File.AppendAllText("Assets/Resourcers/translation_soulConductor.csv", appended);
        
        UnityEditor.AssetDatabase.Refresh();
    }

        public void Remove(string key)
    {
        string[] lines = contents.Split(new string[] { lineSeparator }, StringSplitOptions.None);

        string[] keys = new string[lines.Length];

        for(int i=0; i<lines.Length; i++)
        {
            string line = lines[i];

            keys[i] = line.Split(fieldSeparator, System.StringSplitOptions.None)[0];
        }

        int index = -1;

        for (int i = 0; i < keys.Length; i++)
        {
            if (keys[i].Contains(key))
            {
                index = index;
                break;
            }
        }

        if(index > -1)
        {
            string[] newLines;
            newLines = lines.Where(w => w != lines[index]).ToArray();

            string replaced = string.Join(lineSeparator.ToString(), newLines);
            File.WriteAllText("Assets/Resources/translation_soulConductor.csv", replaced);
        }
    }   

    public void Edit(string key, string value)
    {
        Remove(key);
        Add(key, value); 
    }
     
//#endif



}



