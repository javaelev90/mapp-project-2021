using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatsRepository 
{

    SQLiteUtility sqliteUtility;
    int PLAYER_ID = 0;
    string UPSERT_LATENCY_SQL = "INSERT INTO PLAYER_STATS(ID, SOUND_LATENCY) VALUES(@ID, @SOUND_LATENCY) " +
                               " ON CONFLICT(ID) DO UPDATE SET SOUND_LATENCY=EXCLUDED.SOUND_LATENCY;"; 
    string SELECT_LATENCY_SQL = "SELECT SOUND_LATENCY FROM PLAYER_STATS WHERE ID=@ID;"; 
    string UPSERT_LEVEL_SQL = "INSERT INTO PLAYER_STATS(ID, LEVEL) VALUES(@ID, @LEVEL) " +
                               " ON CONFLICT(ID) DO UPDATE SET LEVEL=EXCLUDED.LEVEL;"; 
    string SELECT_LEVEL_SQL = "SELECT LEVEL FROM PLAYER_STATS WHERE ID=@ID;"; 

    public PlayerStatsRepository(SQLiteUtility sqliteUtility)
    {
        this.sqliteUtility = sqliteUtility;
    }

    public void UpdateLatency(int latency)
    {
        UpdateField(UPSERT_LATENCY_SQL, "SOUND_LATENCY", latency);
    }

    public void UpdateLevel(int level)
    {
        UpdateField(UPSERT_LEVEL_SQL, "LEVEL", level);
    }

    public int GetLatency()
    {
        object result = SelectField(SELECT_LATENCY_SQL);
        return result != null && !result.Equals(DBNull.Value) ? Convert.ToInt32(result) : -1;
    }

    public int GetLevel()
    {
        object result = SelectField(SELECT_LEVEL_SQL);
        return result != null && !result.Equals(DBNull.Value) ? Convert.ToInt32(result) : -1;
    }

    void UpdateField(string sql, string name, object value)
    {
        KeyValuePair<string, object> idParam = new KeyValuePair<string, object>("ID", PLAYER_ID);
        KeyValuePair<string, object> otherParam = new KeyValuePair<string, object>(name, value);
        sqliteUtility.UpsertOperation(sql, idParam, otherParam);
    }

    object SelectField(string sql)
    {
        KeyValuePair<string, object> idParam = new KeyValuePair<string, object>("ID", PLAYER_ID);
        return sqliteUtility.SelectOperation(sql, idParam);
    }

}
