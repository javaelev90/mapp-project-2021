using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab.ClientModels;

public class LeaderboardUIController : MonoBehaviour
{
    public GameObject rowPrefab;
    public Transform rowsParent;

    bool hasRecievedLeaderboard;
    public PlayFabManager playFabManager;

    public void GetLeaderboard()
    {
        hasRecievedLeaderboard = false;
        playFabManager.GetLeaderboard();
    }

    // Update is called once per frame
    void Update()
    {
        if (hasRecievedLeaderboard == false)
        {
            if (playFabManager.result != null)
            {
                hasRecievedLeaderboard = true;

                foreach (Transform item in rowsParent)
                {
                    Destroy(item.gameObject);
                }

                foreach (var item in playFabManager.result.Leaderboard)
                {
                    GameObject newGo = Instantiate(rowPrefab, rowsParent);
                    Text[] texts = newGo.GetComponentsInChildren<Text>();
                    texts[0].text = (item.Position + 1).ToString();
                    texts[1].text = item.PlayFabId;
                    texts[2].text = item.StatValue.ToString();

                    Debug.Log(string.Format("PLACE: {0} | IDictionary {1} | VALUE: {2}",
                        item.Position, item.PlayFabId, item.StatValue));


                }
                playFabManager.result = null;
            }
        }
    }
}
