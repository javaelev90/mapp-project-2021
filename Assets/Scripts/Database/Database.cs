using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Database : MonoBehaviour
{

    private static Database _instance;
    private static SQLiteUtility sqliteUtility;
    public static SongRepository songRepository;
    public static PlayerStatsRepository playerStatsRepository;
    private bool initialized;
    public static Database Instance
    {
        get
        {
            return _instance;
        }

    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            GameObject.Destroy(Instance);
            return;
        } 
        else
        {
            _instance = this;
        }

        if(!initialized)
        {
            Initialize();
        }

        DontDestroyOnLoad(this);
    }

    private void Initialize()
    {
        sqliteUtility = new SQLiteUtility();
        sqliteUtility.Initialize();
        songRepository = new SongRepository(sqliteUtility);
        playerStatsRepository = new PlayerStatsRepository(sqliteUtility);

        initialized = true;
    }

}
