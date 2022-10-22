using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchServerScene : NetworkBehaviour
{
    public GameObject player;

    private void Start()
    {
        if(NetworkManager.IsHost)
            ChangeServerRpc();
    }

    [ServerRpc]
    public void ChangeServerRpc()
    {
        Debug.Log("Loading");
        NetworkManager.SceneManager.LoadScene("Arena", UnityEngine.SceneManagement.LoadSceneMode.Single);
        NetworkManager.SceneManager.OnLoadComplete += LoadPlayers;
    }

    private void LoadPlayers(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
    {
        var activePlayer = NetworkManager.Instantiate(player);
        activePlayer.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
    }
}
