using MLAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : NetworkBehaviour
{
    [SerializeField] private Sprite icon = null;
    [SerializeField] private int buildingID = -1;
    [SerializeField] private int price = 100;
    [SerializeField] private GameObject buildingPreview = null;

    public static event Action<Building> ServerOnBuildingSpawned;
    public static event Action<Building> ServerOnBuildingDespawned;

    public static event Action<Building> AuthorityOnBuildingSpawned;
    public static event Action<Building> AuthorityOnBuildingDespawned; 

    public override void NetworkStart()
    {
#if UNITY_SERVER
        if (IsServer)
        {
            ServerOnBuildingSpawned?.Invoke(this);
        }
#else
        if (IsOwner)
        {
            AuthorityOnBuildingSpawned?.Invoke(this);
        }
#endif
        base.NetworkStart();
    }

    private void OnDestroy()
    {
#if UNITY_SERVER
        if (IsServer)
        {
            ServerOnBuildingDespawned?.Invoke(this);
        }
#else
        if (IsOwner)
        {
            AuthorityOnBuildingDespawned?.Invoke(this);
        }
#endif
    }

    public GameObject GetBuildingPreview()
    {
        return buildingPreview;
    }

    public Sprite GetIcon()
    {
        return icon;
    }

    public int GetID()
    {
        return buildingID;
    }

    public int GetPrice()
    {
        return price;
    }
}
