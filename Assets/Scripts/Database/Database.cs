using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Database : SingletonPatternPersistent<Database>, IInitializeAble
{
    private static SQLiteUtility sqliteUtility;
    public SongRepository songRepository;
    public PlayerStatsRepository playerStatsRepository;
    private bool initialized;

    public void Initialize()
    {
        if(!initialized)
        {
            sqliteUtility = new SQLiteUtility();
            songRepository = new SongRepository(sqliteUtility);
            playerStatsRepository = new PlayerStatsRepository(sqliteUtility);
            initialized = true;
        }
    }

}
