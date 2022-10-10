using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using Unity.Services.Economy;
using Unity.Services.Economy.Model;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadGameDataLogin : MonoBehaviour
{
    public Image progressBar;
    public GameData gameData;

    private async void Start()
    {
        Task<Dictionary<string, string>> task;
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInWithGooglePlayGamesAsync(GetNetworkData.authCode);
        await (task = CloudSaveService.Instance.Data.LoadAllAsync());

        if (task.IsCompletedSuccessfully)
        {
            Dictionary<string, string> valuesOfTask = task.Result;
            foreach (var item in GlobalUtil.Instance.GetData(valuesOfTask))
            {
                gameData.data.Add(item);
                Debug.Log("ID: " + item.id + ", Position: " + item.position);
            }

            Debug.Log(gameData.data.Count);
            progressBar.fillAmount = 0.75f;

            //If we have the local data it's faster than getting it online
            if (PlayerPrefs.GetInt("local") != 1 || gameData.canSaveLocal == false)
            {
                Debug.Log("Not Local");
                await foreach (PlayerBalance balance in GlobalUtil.Instance.GetBalanceAsync())
                {
                    switch (balance.CurrencyId)
                    {
                        case "FOOD":
                            gameData.currencyData.food = (int)balance.Balance;
                            break;
                        case "WOOD":
                            gameData.currencyData.wood = (int)balance.Balance;
                            break;
                        case "STONE":
                            gameData.currencyData.stone = (int)balance.Balance;
                            break;
                        case "IRON":
                            gameData.currencyData.iron = (int)balance.Balance;
                            break;
                        case "0":
                            gameData.currencyData.gold = (int)balance.Balance;
                            break;
                    }
                }

                GlobalUtil.Instance.UpdateBalanceLocal(gameData.currencyData, gameData);
                PlayerPrefs.SetInt("local", 1);
                PlayerPrefs.Save();
            }
            else
            {
                Debug.Log("Local");

                //Load the local data
                gameData.currencyData.food = PlayerPrefs.GetInt("food");
                gameData.currencyData.wood = PlayerPrefs.GetInt("wood");
                gameData.currencyData.stone = PlayerPrefs.GetInt("stone");
                gameData.currencyData.iron = PlayerPrefs.GetInt("iron");
                gameData.currencyData.gold = PlayerPrefs.GetInt("gold");
            }

            StartCoroutine(GlobalUtil.Instance.LoadSceneAsync(1));
            //Done
            progressBar.fillAmount = 1f;
        }
    }
    
}
