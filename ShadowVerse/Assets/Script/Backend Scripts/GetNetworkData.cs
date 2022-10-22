using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;
using System.Threading.Tasks;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GetNetworkData : MonoBehaviour
{
    [SerializeField]
    private Image progressBar;
    public static string authCode = "";
    [SerializeField]
    private GameData gameData;

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);

        progressBar.fillAmount = 0.25f;
        Screen.sleepTimeout = SleepTimeout.NeverSleep; //Stops the game screen from turing off

        SetGoogle();
    }

    internal void SetGoogle()
    {
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();

        PlayGamesPlatform.Instance.Authenticate( (code) =>
        {
            if (code == SignInStatus.Success)
            {
                progressBar.fillAmount = 0.50f;

                PlayGamesPlatform.Instance.RequestServerSideAccess(false, (getAuthCode) =>
                {
                    authCode = getAuthCode;
                    Debug.Log("Code: " + authCode);

                    gameObject.AddComponent<LoadGameDataLogin>();
                    gameObject.GetComponent<LoadGameDataLogin>().progressBar = progressBar;
                    gameObject.GetComponent<LoadGameDataLogin>().gameData = gameData;
                });
            }
            else
            {
                Debug.LogError("Failed");
            }
        });
    }

}
