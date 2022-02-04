using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;

public class GameOverDisplay : MonoBehaviour
{
    [SerializeField] private GameObject gameOverUIParent = null;
    [SerializeField] private TextMeshProUGUI winnerText = null;

#if !UNITY_SERVER

    private void Start()
    {
        GameOverHandler.ClientOnGameOver += ClientHandleGameOver;
    }

    private void OnDestroy()
    {
        GameOverHandler.ClientOnGameOver -= ClientHandleGameOver;
    }

#endif

    public void LeaveGame()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            NetworkManager.Singleton.Shutdown();
        }
        else if (NetworkManager.Singleton.IsClient)
        {
            NetworkManager.Singleton.Shutdown();
        }        
    }

    private void ClientHandleGameOver(string playerName)
    {
        winnerText.text = $"{playerName} has won the game!";
        gameOverUIParent.SetActive(true);
    }
}
