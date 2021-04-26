using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class SettingsRepository
{
    string CREATE_SETTINGS_SQL = "CREATE TABLE IF NOT EXISTS SETTINGS( " +
                            "  NAME TEXT PRIMARY KEY, " +
                            "  VALUE TEXT" +
                            ");";

    string UPSERT_FIELD_SQL = "INSERT INTO SETTINGS(NAME, VALUE) VALUES(@NAME, @VALUE) " +
                               " ON CONFLICT(NAME) DO UPDATE SET VALUE=EXCLUDED.VALUE;"; 
    string SELECT_FIELD_SQL = "SELECT VALUE FROM SETTINGS WHERE NAME=@NAME;"; 

    SQLiteUtility sqliteUtility;

    public SettingsRepository(SQLiteUtility sqliteUtility)
    {
        this.sqliteUtility = sqliteUtility;
        this.sqliteUtility.CreateSchema(CREATE_SETTINGS_SQL);
    }

    public void UpdateSetting(string settingName, string settingValue)
    {
        KeyValuePair<string, object> idParam = new KeyValuePair<string, object>("NAME", settingName);
        KeyValuePair<string, object> otherParam = new KeyValuePair<string, object>("VALUE", settingValue);
        sqliteUtility.UpsertOperation(UPSERT_FIELD_SQL, idParam, otherParam);
    }

    public object GetSetting(string settingName)
    {
        KeyValuePair<string, object> idParam = new KeyValuePair<string, object>("NAME", settingName);
        return sqliteUtility.SelectOperation(SELECT_FIELD_SQL, idParam);
    }

}
