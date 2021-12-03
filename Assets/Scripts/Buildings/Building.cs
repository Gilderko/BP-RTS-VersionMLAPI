using Unity.Netcode;
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
    [SerializeField] private string buildingName = "";

    public static event Action<Building> ServerOnBuildingSpawned;
    public static event Action<Building> ServerOnBuildingDespawned;

    public static event Action<Building> AuthorityOnBuildingSpawned;
    public static event Action<Building> AuthorityOnBuildingDespawned; 

    public override void OnNetworkSpawn()
    {
        Debug.Log("Building networkspawn");

        Debug.Log("Building networkspawn server");
        if (NetworkManager.Singleton.IsServer)
        {
            Debug.Log("Building spawned on server");
            ServerOnBuildingSpawned?.Invoke(this);
        }

        if (IsOwner)
        {
            AuthorityOnBuildingSpawned?.Invoke(this);
        }

        base.OnNetworkSpawn();
    }

    public override void OnNetworkDespawn()
    {

        if (IsServer)
        {
            ServerOnBuildingDespawned?.Invoke(this);
        }

        if (IsOwner)
        {
            AuthorityOnBuildingDespawned?.Invoke(this);
        }

        base.OnNetworkDespawn();
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

    public string GetName()
    {
        return buildingName;
    }
}
