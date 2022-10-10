using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class RelayManager : MonoBehaviour
{
    [SerializeField]
    private string joinCode;
    private UnityTransport transport;
    private const int MAX_PLAYERS = 100;

    private async void Awake()
    {
        transport = FindObjectOfType<UnityTransport>();

        await Authenticate();
    }

    private static async Task Authenticate()
    {
        //Note: add fail safe for when the token passes
        if(!AuthenticationService.Instance.IsSignedIn)
            await AuthenticationService.Instance.SignInWithGooglePlayGamesAsync(GetNetworkData.authCode);
    }

    public async void CreateGame()
    {
        //Note: add reigons later
        Allocation a = await RelayService.Instance.CreateAllocationAsync(MAX_PLAYERS);
        joinCode = await RelayService.Instance.GetJoinCodeAsync(a.AllocationId);

        transport.SetHostRelayData(a.RelayServer.IpV4, (ushort)a.RelayServer.Port, a.AllocationIdBytes, a.Key, a.ConnectionData);
        NetworkManager.Singleton.StartHost();
    }

    public async void JoinGame(string id)
    {
        JoinAllocation a = await RelayService.Instance.JoinAllocationAsync(id);
        transport.SetClientRelayData(a.RelayServer.IpV4, (ushort)a.RelayServer.Port, a.AllocationIdBytes, a.Key, a.ConnectionData, a.HostConnectionData);

        NetworkManager.Singleton.StartClient();
    }

}
