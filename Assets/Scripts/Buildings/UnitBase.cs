using System;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// The base for every player. Fires the event when it gets destroyed.
/// </summary>
public class UnitBase : NetworkBehaviour
{
    [SerializeField] private Health health = null;

    public static event Action<UnitBase> ServerOnBaseSpawnend;
    public static event Action<UnitBase> ServerOnBaseDespawned;

    public static event Action<ulong> ServerOnPlayerDie;

    #region Server

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


    private void ServerHandleDeath()
    {
        ServerOnPlayerDie?.Invoke(OwnerClientId);

        Destroy(gameObject);
    }

    #endregion
}
