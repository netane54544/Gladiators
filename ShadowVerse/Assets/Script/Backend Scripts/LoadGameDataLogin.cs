using System;
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
        //Do the local logic first
        ZPlayerPrefs.useSecure = true;
        ZPlayerPrefs.Initialize("0188ulsa", "#has$1223mdddsw!sd,raeewsd#");

        gameData.playersIcons = Resources.LoadAll<Texture2D>("CharIcons");
        gameData.playersIcons.OrderBy(x => Convert.ToInt32(x.name));

        Task<Dictionary<string, string>> task;
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInWithGooglePlayGamesAsync(GetNetworkData.authCode);
        await (task = CloudSaveService.Instance.Data.LoadAllAsync());

        //PlayersInventoryItem createdInventoryItem = await EconomyService.Instance.PlayerInventory.AddInventoryItemAsync("P_FIRST");
        //Debug.Log("Item date: " + createdInventoryItem.Created);

        List<PlayersInventoryItem> inventoryList = await GlobalUtil.Instance.GetItemsAsync();
        List<PlayersInventoryItem> playersList = inventoryList.Where(x => x.InventoryItemId.StartsWith("P")).ToList();

        //Loads the players in the inventory first
        //Note: add local save
        if (playersList.Count > 0)
        {
            gameData.playerInventoryData = new();

            foreach (var item in playersList)
            {
                var itemDef = await item.GetItemDefinitionAsync();
                var data = itemDef.CustomData;

                PlayerData tempData = new()
                {
                    name = data["name"].ToString(),
                    hp = Convert.ToInt32(data["hp"]),
                    attack = Convert.ToInt32(data["attack"]),
                    level = Convert.ToInt32(data["level"]),
                    image = gameData.playersIcons[Convert.ToInt32(data["image"])]
                };

                //Debug.Log("Name: " + tempData.name + " Image: " + tempData.image);

                gameData.playerInventoryData.Add(tempData);
            }
        }
        else
        {
            gameData.playerInventoryData = new();
        }

        if (task.IsCompletedSuccessfully)
        {
            Dictionary<string, string> valuesOfTask = task.Result;

            var storeData = valuesOfTask.ToList().Where(pair => pair.Key == "-1").FirstOrDefault();

            if (!storeData.Equals(default(KeyValuePair<string, string>)))
            {
                valuesOfTask.Remove("-1");
                gameData.capacityConstruction = JsonUtility.FromJson<Amount>(storeData.Value);
                //Debug.Log("Loaded capacity");
            }

            foreach (var item in GlobalUtil.Instance.GetData(valuesOfTask))
            {
                gameData.data.Add(item);
                //Debug.Log("ID: " + item.id + ", Position: " + item.position);
            }

            //Debug.Log(gameData.data.Count);
            progressBar.fillAmount = 0.75f;

            //If we have the local data it's faster than getting it online
            if (ZPlayerPrefs.GetInt("local") != 1 || gameData.canSaveLocal == false)
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
                ZPlayerPrefs.SetInt("local", 1);
                ZPlayerPrefs.Save();
            }
            else
            {
                Debug.Log("Local");

                //Load the local data
                gameData.currencyData.food = ZPlayerPrefs.GetInt("food");
                gameData.currencyData.wood = ZPlayerPrefs.GetInt("wood");
                gameData.currencyData.stone = ZPlayerPrefs.GetInt("stone");
                gameData.currencyData.iron = ZPlayerPrefs.GetInt("iron");
                gameData.currencyData.gold = ZPlayerPrefs.GetInt("gold");
            }

            SceneManager.sceneLoaded += OnTownLoad;

            StartCoroutine(GlobalUtil.Instance.LoadSceneAsync(1));
            //Done
            progressBar.fillAmount = 1f;
        }
    }

    private void OnTownLoad(Scene scene, LoadSceneMode load)
    {
        foreach (var item in gameData.data)
        {
            Instantiate(gameData.GetConfig(item).prefab, item.position, item.rotation);
        }

        SceneManager.sceneLoaded -= OnTownLoad;
    }
}
