using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public class PlayFabManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Login();
    }

    void Login() {
        var request = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true
        };
        PlayFabClientAPI.LoginWithCustomID(request, OnSuccess, OnError);

        }
    void OnSuccess(LoginResult result)
    {
        Debug.Log("Successful login/account create!");
    
    }
    void OnError(PlayFabError error)
    {
        Debug.Log("Error while logging in/creating account!");
        Debug.Log(error.GenerateErrorReport());
    }
        
    public void SendLeaderboard(int score) {
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate> {
                new StatisticUpdate {
                    StatisticName = "PlatformScore",
                    Value = score

                }

            }

        };
        PlayFabClientAPI.UpdatePlayerStatistics(request, OnLeaderboardUpdate, OnError);

    }
    void OnLeaderboardUpdate(UpdatePlayerStatisticsResult result) {
        Debug.Log("Successful leaderboard sent");
        //send leaderboard i gamecontroller

    }
    public void GetLeaderboard() {
        var request = new GetLeaderboardRequest
        {
            StatisticName = "PlatformScore",
            StartPosition = 0,
            MaxResultsCount = 10
        };
        PlayFabClientAPI.GetLeaderboard(request, OnLeaderboardGet, OnError);
    }
    void OnLeaderboardGet(GetLeaderboardResult result) {
        Debug.Log(result.ToString());
        foreach (var item in result.Leaderboard) {
            Debug.Log(item.Position + " " + item.PlayFabId + " " + item.StatValue);

        }

    }
}
