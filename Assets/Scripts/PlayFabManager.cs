using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.UI;
using TMPro;

public class PlayFabManager : MonoBehaviour
{
    
    public GetLeaderboardResult result;

    public GameObject rowPrefab;
    public Transform rowsParent;

    public GameObject nameWindow;
    public GameObject leaderboardWindow;

    [Header("Display name window")]
    public GameObject nameError;
    public InputField nameInput;

    private bool sentLeaderBoardRequest = false;

    void Start()
    {
        Login();
    }

    private void OnEnable()
    {
        if(!PlayFabClientAPI.IsClientLoggedIn())
        {
            Login();
        }
        else
        {
            GetLeaderboard();
        }
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
            if(nameWindow != null) nameWindow.SetActive(true);
        else
            if(leaderboardWindow != null) leaderboardWindow.SetActive(true);
        
        GetLeaderboard();
    }

    public void SubmitNameButton() {
        if (nameInput.text.Trim() != "") {

            var request = new UpdateUserTitleDisplayNameRequest
            {
                DisplayName = nameInput.text,

            };
            PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnDisplayNameUpdate, OnError);

            nameWindow.SetActive(false);

        }
            
    }

    void OnDisplayNameUpdate(UpdateUserTitleDisplayNameResult result)
    {
        Debug.Log("Updated display name");
        if(leaderboardWindow != null) leaderboardWindow.SetActive(true);
        Debug.Log(result.DisplayName);
    }


    void OnError(PlayFabError error)
    {
        Debug.Log("Error while logging in/creating account!");
        Debug.Log(error.GenerateErrorReport());
        sentLeaderBoardRequest = false;
    }

    public void SendLeaderboard(int score, string songName)
    {
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate> {
                new StatisticUpdate {
                    StatisticName = songName,
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
        if(!sentLeaderBoardRequest){
            var request = new GetLeaderboardRequest();
            request.StatisticName = SongHandler.Instance.GetUniqueSongName();
            request.StartPosition = 0;
            request.MaxResultsCount = 10;

            PlayFabClientAPI.GetLeaderboard(request, OnLeaderboardGet, OnError);
            sentLeaderBoardRequest = true;
        }
    }
    void OnLeaderboardGet(GetLeaderboardResult result) {
        sentLeaderBoardRequest = false;
        this.result = result;
        if(rowsParent == null || rowPrefab == null) return;

        foreach (Transform item in rowsParent)
        {
            Destroy(item.gameObject);
        }

        foreach (PlayerLeaderboardEntry item in result.Leaderboard)
        {
            GameObject newGo = Instantiate(rowPrefab, rowsParent);
            TMP_Text[] texts = newGo.GetComponentsInChildren<TMP_Text>();
            texts[0].text = (item.Position + 1).ToString();
            texts[1].text = item.DisplayName != null ? item.DisplayName : item.PlayFabId;
            texts[2].text = item.StatValue.ToString();

            Debug.Log(string.Format("PLACE: {0} | IDictionary {1} | VALUE: {2}",
                item.Position, item.PlayFabId, item.StatValue));
        }

    }
}
