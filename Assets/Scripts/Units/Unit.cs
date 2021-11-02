using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Events;
using System;

public class Unit : NetworkBehaviour
{
    [SerializeField] private UnitMovement unitMovement = null;
    [SerializeField] private Targeter targeter = null;
    [SerializeField] private Health health = null;

    [SerializeField] private UnityEvent onSelected = null;
    [SerializeField] private UnityEvent onDeselected = null;

    [SerializeField] private int resourceCost = 5;

    public static event Action<Unit> ServerOnUnitSpawned;
    public static event Action<Unit> ServerOnUnitDespawned;

    public static event Action<Unit> AuthorityOnUnitSpawned;
    public static event Action<Unit> AuthorityOnUnitDespawned;

    public override void OnNetworkSpawn()
    {

        if (IsServer)
        {
            ServerOnUnitSpawned?.Invoke(this);
            health.ServerOnDie += ServerHandleDie;
        }

        if (IsOwner)
        {
            AuthorityOnUnitSpawned?.Invoke(this);
        }

        base.OnNetworkSpawn();
    }

    public override void OnNetworkDespawn()
    { 

        if (IsServer)
        {
            health.ServerOnDie -= ServerHandleDie;
            ServerOnUnitDespawned?.Invoke(this);
        }

        if (IsOwner)
        {
            AuthorityOnUnitDespawned?.Invoke(this);
        }

        base.OnNetworkDespawn();
    }

#region Server

    private void ServerHandleDie()
    {
        Destroy(gameObject);
    }

#endregion

#region Client


    public void Select()
    {
        if (!IsOwner)
        {
            return;
        }

        onSelected.Invoke();
    }

    public void Deselect()
    {
        if (!IsOwner)
        {
            return;
        }

        onDeselected.Invoke();
    }

#endregion

    public UnitMovement GetUnitMovement()
    {
        return unitMovement;
    }

    public Targeter GetTargeter()
    {
        return targeter;
    }

    public int GetResourceCost()
    {
        return resourceCost;
    }
}
