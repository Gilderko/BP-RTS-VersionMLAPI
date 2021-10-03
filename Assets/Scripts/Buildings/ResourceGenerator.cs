using MLAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceGenerator : NetworkBehaviour
{
    [SerializeField] private Health health = null;
    [SerializeField] private int resourcesPerInterval = 10;
    [SerializeField] private float interval = 2f;

    private float timer;
    private RTSPlayer player;

    #region Server

    public override void NetworkStart()
    {
        if (IsServer)
        {
            timer = interval;
            player = NetworkManager.ConnectedClients[OwnerClientId].PlayerObject.GetComponent<RTSPlayer>();

            health.ServerOnDie += ServerHandleDie;
            GameOverHandler.ServerOnGameOver += ServerHandleGameOver;
        }

        base.NetworkStart();
    }

    private void OnDestroy()
    {
        if (IsServer)
        {
            health.ServerOnDie -= ServerHandleDie;
            GameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
        }
    }

    private void ServerHandleGameOver()
    {
        enabled = false;
    }

    private void ServerHandleDie()
    {
        Destroy(gameObject);
    }
 
    private void Update()
    {
        if (IsServer)
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                player.AddResources(resourcesPerInterval);
                timer += interval;
            }
        }        
    }

    #endregion
}
