using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Services.CloudSave;
using Unity.Services.Economy;
using Unity.Services.Economy.Model;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class GlobalUtil : Singleton<GlobalUtil>
{

    internal async void SaveDataCloudAsync(string json, int id)
    {
        var toSave = new Dictionary<string, object>() { { id.ToString(), json } };
        Task task;
        await (task = CloudSaveService.Instance.Data.ForceSaveAsync(toSave));

        if (task.IsCompletedSuccessfully)
        {


            Debug.Log("Saved");
        }
        else
        {
            Debug.Log("Error in saving");
        }
    }

    internal async void SaveDataCloudAsync(string json, int id, System.Action success)
    {
        var toSave = new Dictionary<string, object>() { { id.ToString(), json } };
        Task task;
        await (task = CloudSaveService.Instance.Data.ForceSaveAsync(toSave));

        if (task.IsCompletedSuccessfully)
        {
            success.Invoke();
            Debug.Log("Saved");
        }
        else
        {
            Debug.Log("Error in saving");
        }
    }

    internal async void SaveDataCloudAsync(string json, int id, System.Action fail, System.Action success = null)
    {
        var toSave = new Dictionary<string, object>() { { id.ToString(), json } };
        Task task;
        await (task = CloudSaveService.Instance.Data.ForceSaveAsync(toSave));

        if (task.IsCompletedSuccessfully)
        {

            if(success != null)
                success.Invoke();

            Debug.Log("Saved");
        }
        else
        {
            fail.Invoke();
            Debug.Log("Error in saving");
        }
    }

    internal void UpdateBalanceLocal(CurrencyData data, GameData gameData)
    {
        gameData.currencyData.food = data.food;
        gameData.currencyData.wood = data.wood;
        gameData.currencyData.iron = data.iron;
        gameData.currencyData.stone = data.stone;
        gameData.currencyData.gold = data.gold;

        PlayerPrefs.SetInt("food", data.food);
        PlayerPrefs.SetInt("wood", data.wood);
        PlayerPrefs.SetInt("iron", data.iron);
        PlayerPrefs.SetInt("stone", data.stone);
        PlayerPrefs.SetInt("gold", data.gold);
        PlayerPrefs.Save();
    }

    internal async IAsyncEnumerable<PlayerBalance> GetBalanceAsync()
    {
        //GetBalancesOptions balancesOptions = new GetBalancesOptions() { ItemsPerFetch = 5 };
        List<CurrencyDefinition> currencies = await EconomyService.Instance.Configuration.GetCurrenciesAsync();

        foreach (CurrencyDefinition currency in currencies)
        {
            //Debug.Log("ID: " + currency.Name);
            var balance = await currency.GetPlayerBalanceAsync();
            yield return balance;
        }
    }

    internal IEnumerator LoadSceneAsync(int id)
    {
        AsyncOperation loadNext = SceneManager.LoadSceneAsync(1);

        while (!loadNext.isDone)
        {
            yield return null;
        }
    }

    internal IEnumerable<BuildingData> GetData(Dictionary<string, string> keys)
    {
        for (int i = 0; i < keys.Count; i++)
        {
            var value = keys.ElementAt(i).Value;

            yield return (BuildingData)JsonUtility.FromJson(value, typeof(BuildingData));
        }
    }
}
