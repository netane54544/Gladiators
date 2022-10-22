using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.CloudCode;
using Unity.Services.Core;
using Unity.Services.Economy;
using Unity.Services.Economy.Model;
using Unity.Services.Lobbies;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class TownNetworkData : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI userText;
    [SerializeField]
    private TextMeshProUGUI foodText;
    [SerializeField]
    private TextMeshProUGUI ironText;
    [SerializeField]
    private TextMeshProUGUI woodText;
    [SerializeField]
    private TextMeshProUGUI stoneText;
    [SerializeField]
    private TextMeshProUGUI goldText;
    [SerializeField]
    private GameData gameData;

    private async void Awake()
    {
        userText.text = Social.localUser.userName;

        //Note: add fail safe for the token passing
        if(UnityServices.State == ServicesInitializationState.Uninitialized)
            await UnityServices.InitializeAsync();

        //await AuthenticationService.Instance.SignInAnonymouslyAsync();

        //Set text
        foodText.text = gameData.currencyData.food.ToString();
        ironText.text = gameData.currencyData.iron.ToString();
        woodText.text = gameData.currencyData.wood.ToString();
        stoneText.text = gameData.currencyData.stone.ToString();
        goldText.text = gameData.currencyData.gold.ToString();
    }

}
