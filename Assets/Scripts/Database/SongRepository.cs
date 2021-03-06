using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SongRepository
{
    
    string CREATE_SONG_SQL = "CREATE TABLE IF NOT EXISTS SONG ( " +
                            "  NAME TEXT PRIMARY KEY, " +
                            "  SCORE INTEGER" +
                            ");";

    string UPSERT_FIELD_SQL = "INSERT INTO SONG(NAME, SCORE) VALUES(@NAME, @SCORE) " +
                               " ON CONFLICT(NAME) DO UPDATE SET SCORE=EXCLUDED.SCORE;"; 
    string SELECT_FIELD_SQL = "SELECT SCORE FROM SONG WHERE NAME=@NAME;"; 

    SQLiteUtility sqliteUtility;

    public SongRepository(SQLiteUtility sqliteUtility)
    {
        this.sqliteUtility = sqliteUtility;
        this.sqliteUtility.CreateSchema(CREATE_SONG_SQL);
    }

    public void UpdateSongScore(string songName, int score)
    {
        KeyValuePair<string, object> idParam = new KeyValuePair<string, object>("NAME", songName);
        KeyValuePair<string, object> otherParam = new KeyValuePair<string, object>("SCORE", score);
        sqliteUtility.UpsertOperation(UPSERT_FIELD_SQL, idParam, otherParam);
    }

    public int GetSongScore(string songName)
    {
        KeyValuePair<string, object> idParam = new KeyValuePair<string, object>("NAME", songName);
        object result = sqliteUtility.SelectOperation(SELECT_FIELD_SQL, idParam);
        return result != null && !result.Equals(DBNull.Value) ? Convert.ToInt32(result) : -1;
    }
}
