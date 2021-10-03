using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class RTSPlayer : NetworkBehaviour
{
    [SerializeField]
    private LayerMask buildingBlockCollisionLayer = new LayerMask();

    [SerializeField]
    private LayerMask buildingKeepDistanceLayer = new LayerMask();

    [SerializeField] 
    private Building[] buildings = new Building[0];

    [SerializeField]
    private float buildingRangeLimit = 10f;

    [SerializeField]
    private float buildingFromEnemyLimit = 5f;

    [SerializeField]
    private Transform cameraTransform;

    [SerializeField]
    private Vector2 cameraStartOffset = new Vector2(-5, -5);

    private NetworkVariable<int> resources = new NetworkVariable<int>(
        new NetworkVariableSettings() {WritePermission = NetworkVariablePermission.ServerOnly, ReadPermission = NetworkVariablePermission.Everyone },
        500);
    
    private NetworkVariable<bool> isPartyOwner = new NetworkVariable<bool>(
        new NetworkVariableSettings() { WritePermission = NetworkVariablePermission.ServerOnly, ReadPermission = NetworkVariablePermission.Everyone },
        false);
    
    private NetworkVariable<string> playerName = new NetworkVariable<string>(
        new NetworkVariableSettings() { WritePermission = NetworkVariablePermission.ServerOnly, ReadPermission = NetworkVariablePermission.Everyone },
        "");

    private NetworkVariable<Vector3> startPosition = new NetworkVariable<Vector3>(
        new NetworkVariableSettings() { WritePermission = NetworkVariablePermission.ServerOnly, ReadPermission = NetworkVariablePermission.Everyone },
        new Vector3(0,0,21));

    public event Action<int> ClientOnResourcesUpdated;

    public static event Action ClientOnInfoUpdated;
    public static event Action<bool> AuthorityOnPartyOwnerChanged;

    private Color teamColor = new Color();

    [SerializeField] 
    private List<Unit> myUnits = new List<Unit>();

    [SerializeField] 
    private List<Building> myBuildings = new List<Building>();

    public override void NetworkStart()
    {
        if (IsServer)
        {
            OnStartServer();
        }
        else if (IsClient)
        {
            OnStartAuthority();
            OnStartClient();
        }

        base.NetworkStart();
    }

    public void OnDestroy()
    {
        if (IsServer)
        {
            OnStopServer();
        }
        else if (IsClient)
        {
            OnStopClient();
        }
    }


    #region Server

    public void OnStartServer()
    {
        Unit.ServerOnUnitSpawned += ServerHandleUnitSpawned;
        Unit.ServerOnUnitDespawned += ServerHandleUnitDespawned;
        Building.ServerOnBuildingSpawned += ServerHandleBuildingSpawned;
        Building.ServerOnBuildingDespawned += ServerHandleBuildingDespawned;

        DontDestroyOnLoad(gameObject);
    }

    public void SetPlayerName(string displayName)
    {
        playerName.Value = displayName;
    }

    private void ServerHandleBuildingDespawned(Building building)
    {
        if (building.OwnerClientId != OwnerClientId) { return; }

        myBuildings.Remove(building);
    }

    private void ServerHandleBuildingSpawned(Building building)
    {
        if (building.OwnerClientId != OwnerClientId) { return; }

        myBuildings.Add(building);
    }

    private void ServerHandleUnitSpawned(Unit unit)
    {
        // Check if the same person who owns this player also owns this unit
        if (unit.OwnerClientId != OwnerClientId)
        {
            return;
        }

        myUnits.Add(unit);
    }

    public void OnStopServer()
    {
        Unit.ServerOnUnitSpawned -= ServerHandleUnitSpawned;
        Unit.ServerOnUnitDespawned -= ServerHandleUnitDespawned;
        Building.ServerOnBuildingSpawned -= ServerHandleBuildingSpawned;
        Building.ServerOnBuildingDespawned -= ServerHandleBuildingDespawned;
    }

    private void ServerHandleUnitDespawned(Unit unit)
    {
        if (unit.OwnerClientId != OwnerClientId)
        {
            return;
        }

        myUnits.Remove(unit);
    }

    public void AddResources(int resourcesToAdd)
    {
        resources.Value += resourcesToAdd;
    }

    public void SetTeamColor(Color newColor)
    {
        teamColor = newColor;
    }

    public void SetPartyOwner(bool state)
    {
        isPartyOwner.Value = state;
    }

    public void ChangeStartingPosition(Vector3 pos)
    {
        startPosition.Value = pos;
    }

    [ServerRpc]
    public void CmdStartGameServerRpc()
    {
        if (!isPartyOwner.Value) 
        { 
            return; 
        }

        ((RTSNetworkManager)NetworkManager.Singleton).StartGame();
    }

    [ServerRpc]
    public void CmdTryPlaceBuildingServerRpc(int buildingID, Vector3 positionToSpawn)
    {       
        Building buildingToPlace = buildings.First(build => build.GetID() == buildingID);

        if (buildingToPlace == null)
        {
            return;
        }

        if (resources.Value < buildingToPlace.GetPrice())
        {
            return;
        }

        BoxCollider buildingCollider = buildingToPlace.GetComponent<BoxCollider>();        

        if (!CanPlaceBuilding(buildingCollider, positionToSpawn))
        {
            return;
        }

        GameObject building = Instantiate(buildingToPlace.gameObject, positionToSpawn, Quaternion.identity);
        
        building.GetComponent<NetworkObject>().ChangeOwnership(OwnerClientId);
        building.GetComponent<NetworkObject>().Spawn();

        AddResources(-buildingToPlace.GetPrice());
    }

    #endregion

    #region Client


    public void OnStartAuthority()
    {
        if (IsOwner)
        {
            return;
        }

        isPartyOwner.OnValueChanged += AuthorityHandlePartyOwnerStateUpdated;                

        Unit.AuthorityOnUnitSpawned += AuthorityHandleUnitSpawned;
        Unit.AuthorityOnUnitDespawned += AuthorityHandleUnitDespawned;
        Building.AuthorityOnBuildingSpawned += AuthorityHandleBuildingSpawned;
        Building.AuthorityOnBuildingDespawned += AuthorityHandleBuildingDespawned;
    }

    public void OnStartClient()
    {
        DontDestroyOnLoad(gameObject);

        resources.OnValueChanged += ClientHandleResourcesUpdated;
        playerName.OnValueChanged += ClientHandleDisplayNameUpdated;
        startPosition.OnValueChanged += ClientHandleStartCameraPositionUpdated;

        ((RTSNetworkManager)NetworkManager.Singleton).Players.Add(this);        
    }

 
    private void AuthorityHandlePartyOwnerStateUpdated(bool oldState, bool newState)
    {
        if (!IsOwner)
        {
            return;
        }

        AuthorityOnPartyOwnerChanged?.Invoke(newState);
    }

    private void ClientHandleDisplayNameUpdated(string oldVal, string newVal)
    {
        ClientOnInfoUpdated?.Invoke();
    }


    private void AuthorityHandleBuildingDespawned(Building building)
    {
        myBuildings.Remove(building);
    }


    private void AuthorityHandleBuildingSpawned(Building building)
    {
        myBuildings.Add(building);
    }


    private void ClientHandleStartCameraPositionUpdated(Vector3 oldVec, Vector3 newVec)
    {
        if (!IsOwner)
        {
            return;
        }

        cameraTransform.position = new Vector3(newVec.x + cameraStartOffset.x, 21, newVec.z + cameraStartOffset.y);
    }


    public void OnStopClient()
    {
        ClientOnInfoUpdated?.Invoke();

        if (!IsClient)
        {
            return;
        }

        ((RTSNetworkManager)NetworkManager.Singleton).Players.Add(this);

        if (!IsOwner)
        {
            return;
        }

        Unit.AuthorityOnUnitSpawned -= AuthorityHandleUnitSpawned;
        Unit.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned;
        Building.AuthorityOnBuildingSpawned -= AuthorityHandleBuildingSpawned;
        Building.AuthorityOnBuildingDespawned -= AuthorityHandleBuildingDespawned;
    }

    private void AuthorityHandleUnitSpawned(Unit unit)
    {
        myUnits.Add(unit);
    }

    private void AuthorityHandleUnitDespawned(Unit unit)
    {
        myUnits.Remove(unit);
    } 

    private void ClientHandleResourcesUpdated(int oldValue, int newValue)
    {
        ClientOnResourcesUpdated?.Invoke(newValue);
    }

    #endregion

    public int GetResources()
    {
        return resources.Value;
    }

    public IEnumerable<Unit> GetMyUnits()
    {
        return myUnits;
    }

    public IEnumerable<Building> GetMyBuildings()
    {
        return myBuildings;
    }

    public bool CanPlaceBuilding(BoxCollider buildingCollider, Vector3 positionToSpawn)
    {
        if (Physics.CheckBox(
            positionToSpawn + buildingCollider.center,
            buildingCollider.size / 2, 
            Quaternion.identity,
            buildingBlockCollisionLayer))
        {
            return false;
        }

        RaycastHit[] hits = Physics.SphereCastAll(positionToSpawn, buildingFromEnemyLimit, Vector3.up, buildingKeepDistanceLayer);
        foreach (RaycastHit hit in hits)
        {
            Unit possibleUnit = hit.transform.GetComponent<Unit>();
            
            if (possibleUnit != null)
            {                
                bool hasAuth = IsClient ? possibleUnit.IsOwner : possibleUnit.OwnerClientId == OwnerClientId;
                if (!hasAuth)
                {
                    return false;
                }
            }

            Building possibleBuilding = hit.transform.GetComponent<Building>();
            if (possibleBuilding != null)
            {
                bool hasAuth = IsClient ? possibleBuilding.IsOwner : possibleBuilding.OwnerClientId == OwnerClientId;
                if (!hasAuth)
                {
                    return false;
                }
            }
        }

        foreach (Building build in myBuildings)
        {
            if ((positionToSpawn - build.transform.position).sqrMagnitude <= buildingRangeLimit * buildingRangeLimit)
            {
                return true;
            }
        }

        return false;
    }

    public Color GetTeamColor()
    {
        return teamColor;
    }

    public Transform GetCameraTransform()
    {
        return cameraTransform;
    }

    public bool IsPartyOwner()
    {
        return isPartyOwner.Value;
    }
    
    public string GetDisplayName()
    {
        return playerName.Value;
    }
}
