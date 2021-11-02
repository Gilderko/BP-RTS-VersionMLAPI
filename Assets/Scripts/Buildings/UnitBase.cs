using Unity.Netcode;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBase : NetworkBehaviour
{
    [SerializeField] private Health health = null;

    public static event Action<UnitBase> ServerOnBaseSpawnend;
    public static event Action<UnitBase> ServerOnBaseDespawned;

    public static event Action<ulong> ServerOnPlayerDie;

    #region Server

#if UNITY_SERVER
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            health.ServerOnDie += ServerHandleDeath;
            ServerOnBaseSpawnend?.Invoke(this);
        }

        base.OnNetworkSpawn();
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            ServerOnBaseDespawned?.Invoke(this);
            health.ServerOnDie -= ServerHandleDeath;
        }
        base.OnNetworkDespawn();
    }
#endif

    private void ServerHandleDeath()
    {
        ServerOnPlayerDie?.Invoke(OwnerClientId);

        Destroy(gameObject);
    }


#endregion
}
