using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.UI;

public class PlayFabManager : SingletonPatternPersistent<PlayFabManager>, IInitializeAble
{
    
    public GetLeaderboardResult result;
    public void Initialize() { }

    public GameObject nameWindow;
    public GameObject leaderboardWindow;

    [Header("Display name window")]
    public GameObject nameError;
    public InputField nameInput;


    // Start is called before the first frame update
    void Start()
    {
        Login();
    }

    void Login() {
        var request = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams {
                GetPlayerProfile = true

            }
        };
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnError);

        }
    void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Successful login/account create!");
        string name = null;
        if (result.InfoResultPayload.PlayerProfile != null)
        name = result.InfoResultPayload.PlayerProfile.DisplayName;

        if (name == null)
            nameWindow.SetActive(true);
        else
            leaderboardWindow.SetActive(true);
    
    }

    public void SubmitNameButton() {
        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = nameInput.text,

        };
        PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnDisplayNameUpdate, OnError);

    }

    void OnDisplayNameUpdate(UpdateUserTitleDisplayNameResult result)
    {
        Debug.Log("Updated display name");
        leaderboardWindow.SetActive(true);
        Debug.Log(result.DisplayName);
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
        

    }
    public void GetLeaderboard() {
        var request = new GetLeaderboardRequest();
        request.StatisticName = "PlatformScore";
        request.StartPosition = 0;
        request.MaxResultsCount = 10;
        //{
            //StatisticName = "PlatformScore",
            //StartPosition = 0,
            //MaxResultsCount = 10
        //};
        PlayFabClientAPI.GetLeaderboard(request, OnLeaderboardGet, OnError);
    }
    void OnLeaderboardGet(GetLeaderboardResult result) {
        this.result = result;
        //Debug.Log(result.ToString());
        //foreach (var item in result.Leaderboard) {
        //    Debug.Log(item.Position + " " + item.PlayFabId + " " + item.StatValue);


        //}

    }
}
