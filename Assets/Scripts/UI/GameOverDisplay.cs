using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MLAPI;

public class GameOverDisplay : MonoBehaviour
{
    [SerializeField] private GameObject gameOverUIParent = null;
    [SerializeField] private TextMeshProUGUI winnerText = null;
    
    private void Start()
    {
        GameOverHandler.ClientOnGameOver += ClientHandleGameOver;
    }

    private void OnDestroy()
    {
        GameOverHandler.ClientOnGameOver -= ClientHandleGameOver;
    }

    public void LeaveGame()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            NetworkManager.Singleton.StopServer();
        }
        else if (NetworkManager.Singleton.IsClient)
        {
            NetworkManager.Singleton.StopClient();
        }        
    }

    private void ClientHandleGameOver(string playerName)
    {
        winnerText.text = $"{playerName} has won the game!";
        gameOverUIParent.SetActive(true);
    }
}
