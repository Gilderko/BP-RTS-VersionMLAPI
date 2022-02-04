using Unity.Netcode;
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


    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            timer = interval;
            player = (NetworkManager.Singleton as RTSNetworkManager).GetRTSPlayerByUID(OwnerClientId);

            health.ServerOnDie += ServerHandleDie;
            GameOverHandler.ServerOnGameOver += ServerHandleGameOver;
        }

        base.OnNetworkSpawn();
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            health.ServerOnDie -= ServerHandleDie;
            GameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
        }
        base.OnNetworkDespawn();
    }

#if UNITY_SERVER

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

#endif


    private void ServerHandleGameOver()
    {
        enabled = false;
    }

    private void ServerHandleDie()
    {
        Destroy(gameObject);
    }
#endregion
}
