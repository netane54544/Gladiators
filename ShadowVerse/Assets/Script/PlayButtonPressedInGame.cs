using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayButtonPressedInGame : MonoBehaviour
{
    private Button btn;
    [SerializeField]
    private GameObject networkManager;
    [SerializeField]
    private GameObject networkRoomSwitcher;
    [SerializeField]
    private GameObject playerPrefab;
    [SerializeField]
    private GameObject lobbyPanel;

    private void Awake()
    {
        btn = gameObject.GetComponent<Button>();
        btn.onClick.AddListener(() => 
        {
            var relay = networkManager.AddComponent<RelayManager>();
            relay.roomSwitcherPrefab = networkRoomSwitcher;
            relay.playerPrefab = playerPrefab;
            relay.lobbyPanel = lobbyPanel;
        });
    }
}
