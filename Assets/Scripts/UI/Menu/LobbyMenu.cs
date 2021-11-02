using MLAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyMenu : MonoBehaviour
{
    [SerializeField] private GameObject lobbyUi;
    [SerializeField] private Button startGameButton;

    [SerializeField] private RectTransform playerParent;
    [SerializeField] private PlayerLobbyUIInstance playerLobbyUI;

#if (UNITY_SERVER == false)
    private void Start()
    {
        RTSNetworkManager.ClientOnConnected += HandleClientConnected;
        RTSPlayer.AuthorityOnPartyOwnerChanged += AuthorityHandlePartyOwnerStateUpdate;
        RTSPlayer.ClientOnInfoUpdated += ClientHandleInfoUpdated;
    }    

    private void OnDestroy()
    {
        RTSNetworkManager.ClientOnConnected -= HandleClientConnected;
        RTSPlayer.AuthorityOnPartyOwnerChanged -= AuthorityHandlePartyOwnerStateUpdate;
        RTSPlayer.ClientOnInfoUpdated -= ClientHandleInfoUpdated;
    }
#endif

    private void AuthorityHandlePartyOwnerStateUpdate(bool state)
    {
        startGameButton.gameObject.SetActive(state);
    }

    private void ClientHandleInfoUpdated()
    {
        foreach (Transform child in playerParent.transform)
        {
            Destroy(child.gameObject);
        }

        List<RTSPlayer> players = ((RTSNetworkManager) NetworkManager.Singleton).Players;

        for (int i = 0; i < players.Count; i++)
        {
            var playerUIInstance = Instantiate(playerLobbyUI,playerParent);
            playerUIInstance.SetName(players[i].GetDisplayName());
        }
    }

    private void HandleClientConnected()
    {
        lobbyUi.SetActive(true);
    }

    public void LeaveLobby()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            NetworkManager.Singleton.StopServer();
        }
        else
        {
            NetworkManager.Singleton.StopClient();

            SceneManager.LoadScene(0);
        }
    }

    public void StartGame()
    {
        Debug.Log("Client wants to start");
        Debug.Log((NetworkManager.Singleton as RTSNetworkManager).GetRTSPlayerByUID(NetworkManager.Singleton.LocalClientId).IsPartyOwner());
        (NetworkManager.Singleton as RTSNetworkManager).GetRTSPlayerByUID(NetworkManager.Singleton.LocalClientId).CmdStartGameServerRpc();
    }
}
