using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using Mono.Data.Sqlite;

public class SQLiteUtility
{

    private string CREATE_PLAYER_STATS_SQL = "CREATE TABLE IF NOT EXISTS PLAYER_STATS ( " +
                                "  ID INTEGER PRIMARY KEY, " +
                                "  SOUND_LATENCY TEXT, " +
                                "  LEVEL INTEGER" +
                                ");";

    private string CREATE_MAPS_SQL = "CREATE TABLE IF NOT EXISTS SONG ( " +
                                "  NAME TEXT PRIMARY KEY, " +
                                "  SCORE INTEGER" +
                                ");";

    private string dbPath;

    public SQLiteUtility()
    {
#if UNITY_EDITOR
        dbPath = "URI=file:"+ Application.dataPath +"/LocalTemporaryFiles/SoulConductorDatabase.db";
#else
        dbPath = "URI=file:" + Application.persistentDataPath + "/SoulConductorDatabase.db";
#endif
    }

    public void CreateSchema(string createSchemaString)
    {
        using (var conn = new SqliteConnection(dbPath)) {
            conn.Open();
            using (var cmd = conn.CreateCommand()) {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = createSchemaString;

                cmd.ExecuteNonQuery();
			}
		}
    }

    public void UpsertOperation(string sql, params KeyValuePair<string,object>[] values)
    {
        using (SqliteConnection conn = new SqliteConnection(dbPath)) {
            conn.Open();
            using (SqliteCommand cmd = conn.CreateCommand()) {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sql;
                foreach(KeyValuePair<string, object> keyValuePair in values)
                {
                    cmd.Parameters.Add(new SqliteParameter {
                        ParameterName = keyValuePair.Key,
                        Value = keyValuePair.Value
                    });
                }

                int result = cmd.ExecuteNonQuery();
                Debug.Log("Updated rows: " + result);
            }
        }
    }

    public object SelectOperation(string sql, KeyValuePair<string, object> parameter)
    {
        object result = null;
        using (SqliteConnection conn = new SqliteConnection(dbPath)) {
            conn.Open();
            using (SqliteCommand cmd = conn.CreateCommand()) {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sql;

                cmd.Parameters.Add(new SqliteParameter {
                    ParameterName = parameter.Key,
                    Value = parameter.Value
                });

                SqliteDataReader reader = cmd.ExecuteReader();
                
                result = reader.GetValue(0);
                Debug.Log("Select query returned: "+ result.ToString());
            }
        }
        return result;
    }

}
