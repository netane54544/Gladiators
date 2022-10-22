#if UNITY_EDITOR
using ParrelSync;
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.CloudCode;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RelayManager : MonoBehaviour
{
    internal struct CloudResponse
    {
        public long expire;
    }

    private UnityTransport transport;
    private Lobby connectedLobby;
    private string playerID;
    private const int MAX_PLAYERS = 2;
    private const string JOINCODE_KEY = "jG";
    private Task search;
    private CancellationTokenSource cancellationToken;
    public GameObject roomSwitcherPrefab;
    public GameObject playerPrefab;
    public GameObject lobbyPanel;
    private Texture2D player1Texture;
    private Texture2D player2Texture;
    private const int COOLDOWN = 30;

    private async void Start()
    {
        transport = FindObjectOfType<UnityTransport>();
        string imageUrl = GooglePlayGames.PlayGamesPlatform.Instance.GetUserImageUrl();

#if UNITY_EDITOR
        await UnityServices.InitializeAsync();

        if (ClonesManager.IsClone())
        {
            Debug.Log("This is a clone project.");
            string customArgument = ClonesManager.GetArgument();
            AuthenticationService.Instance.SwitchProfile(customArgument);
            UnityEditor.PlayerSettings.productName = customArgument;
        }

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
#endif

        //Note: add fail safe for when the token passes
        if (!AuthenticationService.Instance.IsSignedIn)
            await AuthenticationService.Instance.SignInWithGooglePlayGamesAsync(GetNetworkData.authCode);

        playerID = AuthenticationService.Instance.PlayerId;

        connectedLobby = await JoinQuickLobbyAsync() ?? await CreateLobbyAsync();
        cancellationToken = new CancellationTokenSource();
        var token = cancellationToken.Token;

        UpdatePlayerOptions playerOptions = new UpdatePlayerOptions() { Data = new Dictionary<string, PlayerDataObject>() { { "url", new PlayerDataObject(visibility: PlayerDataObject.VisibilityOptions.Public, value: imageUrl) } } };
        connectedLobby = await LobbyService.Instance.UpdatePlayerAsync(connectedLobby.Id, playerID, playerOptions);
        
        try
        {
            await (search = Task.Run(async () =>
            {
                bool canExit = false;

                while (connectedLobby.Players.Count < connectedLobby.MaxPlayers || !canExit)
                {
                    if (token.IsCancellationRequested)
                        token.ThrowIfCancellationRequested();

                    if (connectedLobby.Players.Count == connectedLobby.MaxPlayers)
                        if (connectedLobby.Players[1].Data != null)
                            canExit = true;

                    try
                    {
                        connectedLobby = await LobbyService.Instance.GetLobbyAsync(connectedLobby.Id);
                    }
                    catch (LobbyServiceException)
                    {
                        Debug.LogError("Lobby not found");
                    }

                    await Task.Delay(1000);
                }

                if (token.IsCancellationRequested)
                    token.ThrowIfCancellationRequested();

                UnityToolbag.Dispatcher.Invoke(() =>
                {
                    StartCoroutine(LoadImageFromUrl(connectedLobby.Players));
                });
                
                if (NetworkManager.Singleton.IsHost)
                {
                    Debug.Log("Host Done");
                    UnityToolbag.Dispatcher.Invoke(() => 
                    {
                        /*
                        GameObject roomSw = NetworkManager.Instantiate(roomSwitcherPrefab);
                        roomSw.GetComponent<SwitchServerScene>().player = playerPrefab;
                        roomSw.GetComponent<NetworkObject>().Spawn();
                        */
                    });
                }
                else
                {
                    Debug.Log("Client Done");
                }
            }));
        }
        catch (OperationCanceledException)
        {
            Debug.Log("Task finished");
        }
    }

    private void Update()
    {

        if (NetworkManager.Singleton.ShutdownInProgress)
        {
            //Get back to the town
            //Note: add later win logic
            SceneManager.LoadScene(1);
            Destroy(this.gameObject);
        }
    }

    private async Task<Lobby> JoinQuickLobbyAsync()
    {
        try
        {
            var lobby = await Lobbies.Instance.QuickJoinLobbyAsync();
            var status = await RelayService.Instance.JoinAllocationAsync(lobby.Data[JOINCODE_KEY].Value);

            transport.SetClientRelayData(status.RelayServer.IpV4, (ushort)status.RelayServer.Port, status.AllocationIdBytes, status.Key, status.ConnectionData, status.HostConnectionData);

            NetworkManager.Singleton.StartClient();

            return lobby;
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            //Lobby has not been found
            return null;
        }
    }

    private async Task<Lobby> CreateLobbyAsync()
    {
        try
        {
            var allocation = await RelayService.Instance.CreateAllocationAsync(MAX_PLAYERS);
            var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            var lobby = await Lobbies.Instance.CreateLobbyAsync("Quick Join Lobby", MAX_PLAYERS, new CreateLobbyOptions() {Data=new Dictionary<string, DataObject>() { { JOINCODE_KEY, new DataObject(DataObject.VisibilityOptions.Public, joinCode)} }, IsPrivate=false });

            StartCoroutine(HeartBeatLobby(lobby.Id, 15));
            transport.SetHostRelayData(allocation.RelayServer.IpV4, (ushort)allocation.RelayServer.Port, allocation.AllocationIdBytes, allocation.Key, allocation.ConnectionData);

            NetworkManager.Singleton.StartHost();
            return lobby;
        }
        catch (Exception)
        {
            return null;
        }
    }

    private static IEnumerator HeartBeatLobby(string lobbyId, float waitTimeSecounds)
    {
        var delay = new WaitForSecondsRealtime(waitTimeSecounds);

        while (true)
        {
            Lobbies.Instance.SendHeartbeatPingAsync(lobbyId);
            yield return delay;
        }
    }

    private string url;
    internal IEnumerator LoadImageFromUrl(List<Player> players)
    {
        if (url == null)
            url = players[0].Data["url"].Value;

        using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.DataProcessingError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Couldn't return the image");
            }
            else
            {
                var texture = DownloadHandlerTexture.GetContent(webRequest);
                
                if(player1Texture == null)
                {
                    player1Texture = texture;
                    url = players[1].Data["url"].Value;
                    yield return StartCoroutine(LoadImageFromUrl(players));
                }
                else
                {
                    player2Texture = texture;
                    url = null;

                    StartWait();
                }
            }
        }
    }

    private void StartWait()
    {
        var images = lobbyPanel.GetComponentsInChildren<Image>();
        images[1].sprite = Sprite.Create(player1Texture, new Rect(0, 0, player1Texture.width, player1Texture.height), new Vector2(0.5f, 0.5f));
        images[2].sprite = Sprite.Create(player2Texture, new Rect(0, 0, player2Texture.width, player2Texture.height), new Vector2(0.5f, 0.5f));

        lobbyPanel.SetActive(true);

        WaitForCooldown();
    }

    private async void WaitForCooldown()
    {
        int cooldown = 0;
        long timePrev = -1;
        var token = cancellationToken.Token;

        while (cooldown < COOLDOWN)
        {
            if (token.IsCancellationRequested)
                return;

            var response = await CloudCodeService.Instance.CallEndpointAsync<CloudResponse>("Servertime", null);

            if (timePrev == -1)
            {
                timePrev = response.expire;
            }
            else
            {
                int newCooldown = (int)(response.expire - timePrev);
                cooldown = (newCooldown - cooldown > 1) ? cooldown + 1 : newCooldown - cooldown;
                UnityToolbag.Dispatcher.Invoke(() =>
                {
                    Debug.Log(cooldown);
                });
            }

            await Task.Delay(900);
        }

        //Starts the arena
        UnityToolbag.Dispatcher.Invoke(() =>
        {
            if (NetworkManager.Singleton.IsHost)
            {
                //We create new object so the new object will be of type NetworkBehaviour,
                //we will be able to call the server for change of scene for our lobby
                GameObject roomSw = NetworkManager.Instantiate(roomSwitcherPrefab);
                roomSw.GetComponent<SwitchServerScene>().player = playerPrefab;
                roomSw.GetComponent<NetworkObject>().Spawn();
            }
        });
    }

    private void OnDestroy()
    {
        try
        {
            StopAllCoroutines();
            cancellationToken.Cancel();

            if(connectedLobby != null && connectedLobby.HostId == AuthenticationService.Instance.PlayerId)
            {
                Lobbies.Instance.DeleteLobbyAsync(connectedLobby.Id);
            }
            else
            {
                //Note: add logic for when the lobby is closed
                Lobbies.Instance.RemovePlayerAsync(connectedLobby.Id, AuthenticationService.Instance.PlayerId);
            }
        }catch(Exception)
        {
            //Note: add later error management
        }

        cancellationToken.Dispose();
    }

}
