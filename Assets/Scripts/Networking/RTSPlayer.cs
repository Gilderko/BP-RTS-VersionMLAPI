using MLAPI;
using System;
using System.Collections;
using System.Collections.Generic;
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

    private static Dictionary<ulong, RTSPlayer> ulongToPlayer = new Dictionary<ulong, RTSPlayer>();

    [SyncVar(hook = nameof(ClientHandleResourcesUpdated))] 
    private int resources = 500;

    [SyncVar(hook = nameof(AuthorityHandlePartyOwnerStateUpdated))]
    private bool isPartyOwner = false;

    [SyncVar(hook = nameof(ClientHandleDisplayNameUpdated))]
    private string playerName = "";

    [SyncVar(hook = nameof(ClientHandleStartCameraPositionUpdated))]
    private Vector3 startPosition = new Vector3(0, 0, 21);

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
        if (IsClient)
        {
            ulongToPlayer.Add(OwnerClientId, this);
        }

        base.NetworkStart();
    }

    #region Server

    [Server]
    public override void OnStartServer()
    {
        base.OnStartServer();

        Unit.ServerOnUnitSpawned += ServerHandleUnitSpawned;
        Unit.ServerOnUnitDespawned += ServerHandleUnitDespawned;
        Building.ServerOnBuildingSpawned += ServerHandleBuildingSpawned;
        Building.ServerOnBuildingDespawned += ServerHandleBuildingDespawned;

        DontDestroyOnLoad(gameObject);
    }

    [Server]
    public void SetPlayerName(string displayName)
    {
        playerName = displayName;
    }

    [Server]
    private void ServerHandleBuildingDespawned(Building building)
    {
        if (building.connectionToClient.connectionId != connectionToClient.connectionId) { return; }

        myBuildings.Remove(building);
    }

    [Server]
    private void ServerHandleBuildingSpawned(Building building)
    {
        if (building.connectionToClient.connectionId != connectionToClient.connectionId) { return; }

        myBuildings.Add(building);
    }

    [Server]
    private void ServerHandleUnitSpawned(Unit unit)
    {
        // Check if the same person who owns this player also owns this unit
        if (unit.connectionToClient.connectionId != connectionToClient.connectionId)
        {
            return;
        }

        myUnits.Add(unit);
    }

    [Server]
    public override void OnStopServer()
    {
        base.OnStopServer();

        Unit.ServerOnUnitSpawned -= ServerHandleUnitSpawned;
        Unit.ServerOnUnitDespawned -= ServerHandleUnitDespawned;
        Building.ServerOnBuildingSpawned -= ServerHandleBuildingSpawned;
        Building.ServerOnBuildingDespawned -= ServerHandleBuildingDespawned;
    }

    [Server]
    private void ServerHandleUnitDespawned(Unit unit)
    {
        if (unit.connectionToClient.connectionId != connectionToClient.connectionId)
        {
            return;
        }

        myUnits.Remove(unit);
    }

    [Server]
    public void AddResources(int resourcesToAdd)
    {
        resources += resourcesToAdd;
    }

    [Server]
    public void SetTeamColor(Color newColor)
    {
        teamColor = newColor;
    }

    [Server]
    public void SetPartyOwner(bool state)
    {
        isPartyOwner = state;
    }

    [Server] 
    public void ChangeStartingPosition(Vector3 pos)
    {
        startPosition = pos;
    }

    [Command]
    public void CmdStartGame()
    {
        if (!isPartyOwner) 
        { 
            return; 
        }

        ((RTSNetworkManager)NetworkManager.singleton).StartGame();
    }

    [Command]
    public void CmdTryPlaceBuilding(int buildingID, Vector3 positionToSpawn)
    {       
        Building buildingToPlace = buildings.First(build => build.GetID() == buildingID);

        if (buildingToPlace == null)
        {
            return;
        }

        if (resources < buildingToPlace.GetPrice())
        {
            return;
        }

        BoxCollider buildingCollider = buildingToPlace.GetComponent<BoxCollider>();        

        if (!CanPlaceBuilding(buildingCollider, positionToSpawn))
        {
            return;
        }

        GameObject building = Instantiate(buildingToPlace.gameObject, positionToSpawn, Quaternion.identity);
        NetworkServer.Spawn(building, connectionToClient);

        AddResources(-buildingToPlace.GetPrice());
    }

    #endregion

    #region Client

    [Client]
    public override void OnStartAuthority()
    {
        if (NetworkServer.active)
        {
            return;
        }

        base.OnStartClient();

        Unit.AuthorityOnUnitSpawned += AuthorityHandleUnitSpawned;
        Unit.AuthorityOnUnitDespawned += AuthorityHandleUnitDespawned;
        Building.AuthorityOnBuildingSpawned += AuthorityHandleBuildingSpawned;
        Building.AuthorityOnBuildingDespawned += AuthorityHandleBuildingDespawned;
    }

    [Client]
    public override void OnStartClient()
    {
        if (NetworkServer.active)
        {
            return;
        }

        DontDestroyOnLoad(gameObject);

        ((RTSNetworkManager)NetworkManager.singleton).ClientPlayers.Add(this);        
    }

    [Client]
    private void AuthorityHandlePartyOwnerStateUpdated(bool oldState, bool newState)
    {
        if (!hasAuthority)
        {
            return;
        }


        AuthorityOnPartyOwnerChanged?.Invoke(newState);
    }

    [Client]
    private void ClientHandleDisplayNameUpdated(string oldVal, string newVal)
    {
        ClientOnInfoUpdated?.Invoke();
    }

    [Client]
    private void AuthorityHandleBuildingDespawned(Building building)
    {
        myBuildings.Remove(building);
    }

    [Client]
    private void AuthorityHandleBuildingSpawned(Building building)
    {
        myBuildings.Add(building);
    }

    [Client]
    private void ClientHandleStartCameraPositionUpdated(Vector3 oldVec, Vector3 newVec)
    {
        if (!hasAuthority)
        {
            return;
        }

        cameraTransform.position = new Vector3(newVec.x + cameraStartOffset.x, 21, newVec.z + cameraStartOffset.y);
    }

    [Client]
    public override void OnStopClient()
    {
        ClientOnInfoUpdated?.Invoke();

        if (!isClientOnly)
        {
            return;
        }

        ((RTSNetworkManager)NetworkManager.singleton).ClientPlayers.Add(this);

        if (!hasAuthority)
        {
            return;
        }

        base.OnStartClient();

        Unit.AuthorityOnUnitSpawned -= AuthorityHandleUnitSpawned;
        Unit.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned;
        Building.AuthorityOnBuildingSpawned -= AuthorityHandleBuildingSpawned;
        Building.AuthorityOnBuildingDespawned -= AuthorityHandleBuildingDespawned;
    }

    [Client]
    private void AuthorityHandleUnitSpawned(Unit unit)
    {
        myUnits.Add(unit);
    }

    [Client]
    private void AuthorityHandleUnitDespawned(Unit unit)
    {
        myUnits.Remove(unit);
    } 

    [Client]
    private void ClientHandleResourcesUpdated(int oldValue, int newValue)
    {
        ClientOnResourcesUpdated?.Invoke(newValue);
    }
    #endregion

    public int GetResources()
    {
        return resources;
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
                bool hasAuth = isClient ? possibleUnit.hasAuthority : possibleUnit.connectionToClient.connectionId == connectionToClient.connectionId;
                if (!hasAuth)
                {
                    return false;
                }
            }

            Building possibleBuilding = hit.transform.GetComponent<Building>();
            if (possibleBuilding != null)
            {
                bool hasAuth = isClient ? possibleBuilding.hasAuthority : possibleBuilding.connectionToClient.connectionId == connectionToClient.connectionId;
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
        return isPartyOwner;
    }
    
    public string GetDisplayName()
    {
        return playerName;
    }

    public static RTSPlayer GetPlayerByID(ulong ID)
    {
        return ulongToPlayer[ID];
    }
}
