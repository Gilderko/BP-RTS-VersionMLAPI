using Unity.Netcode;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Unity.Collections;

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

    private NetworkVariable<int> resources = new NetworkVariable<int>(NetworkVariableReadPermission.OwnerOnly, 500);

    private NetworkVariable<bool> isPartyOwner = new NetworkVariable<bool>(NetworkVariableReadPermission.Everyone, false);

    private NetworkVariable<FixedString32Bytes> playerName = new NetworkVariable<FixedString32Bytes>(NetworkVariableReadPermission.Everyone, "");

    private NetworkVariable<Vector3> startPosition = new NetworkVariable<Vector3>(NetworkVariableReadPermission.Everyone, new Vector3(0, 0, 21));

    public event Action<int> ClientOnResourcesUpdated;

    public static event Action ClientOnInfoUpdated;
    public static event Action<bool> AuthorityOnPartyOwnerChanged;

    private Color teamColor = new Color();

    [SerializeField] 
    private List<Unit> myUnits = new List<Unit>();

    [SerializeField] 
    private List<Building> myBuildings = new List<Building>();

    public override void OnNetworkSpawn()
    {
        ((RTSNetworkManager)NetworkManager.Singleton).Players.Add(this);


        if (IsServer)
        {
            OnStartServer();
        }

        if (IsClient)
        {
            OnStartAuthority();
            OnStartClient();
        }

        base.OnNetworkSpawn();
    }

    public override void OnNetworkDespawn()
    {

        if (IsServer)
        {
            OnStopServer();
        }

        if (IsClient)
        {
            OnStopClient();
        }

        base.OnNetworkDespawn();
    }


#region Server

    public void OnStartServer()
    {
        Unit.ServerOnUnitSpawned += ServerHandleUnitSpawned;
        Unit.ServerOnUnitDespawned += ServerHandleUnitDespawned;
        Building.ServerOnBuildingSpawned += ServerHandleBuildingSpawned;
        Building.ServerOnBuildingDespawned += ServerHandleBuildingDespawned;        
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
        Debug.Log("Handling spawning");
        if (building.OwnerClientId != OwnerClientId) { return; }

        Debug.Log("Addding to my buildings");
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

        ((RTSNetworkManager)NetworkManager.Singleton).Players.Remove(this);
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
        Debug.Log(isPartyOwner.Value);
        Debug.Log("Server RPC Start game");
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
            Debug.Log("Server says I cant place building");
            return;
        }

        GameObject building = Instantiate(buildingToPlace.gameObject, positionToSpawn, Quaternion.identity);
        
        building.GetComponent<NetworkObject>().SpawnWithOwnership(OwnerClientId, true);

        AddResources(-buildingToPlace.GetPrice());
    }

#endregion

#region Client

    public void OnStartAuthority()
    {
        if (!IsOwner)
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
        resources.OnValueChanged += ClientHandleResourcesUpdated;
        playerName.OnValueChanged += ClientHandleDisplayNameUpdated;
        startPosition.OnValueChanged += ClientHandleStartCameraPositionUpdated;    
    }

 
    private void AuthorityHandlePartyOwnerStateUpdated(bool oldState, bool newState)
    {
        if (!IsOwner)
        {
            return;
        }

        AuthorityOnPartyOwnerChanged?.Invoke(newState);
    }

    private void ClientHandleDisplayNameUpdated(FixedString32Bytes oldVal, FixedString32Bytes newVal)
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

        ((RTSNetworkManager)NetworkManager.Singleton).Players.Remove(this);

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
            Debug.Log("Collision problem");
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
                    Debug.Log("Too close to enemy unit");
                    return false;
                }
            }

            Building possibleBuilding = hit.transform.GetComponent<Building>();
            if (possibleBuilding != null)
            {
                bool hasAuth = IsClient ? possibleBuilding.IsOwner : possibleBuilding.OwnerClientId == OwnerClientId;
                if (!hasAuth)
                {
                    Debug.Log("Too close to enemy building");
                    return false;
                }
            }
        }

        foreach (Building build in myBuildings)
        {
            if ((positionToSpawn - build.transform.position).sqrMagnitude <= buildingRangeLimit * buildingRangeLimit)
            {
                Debug.Log("Close enough to my buildings");
                return true;
            }
        }

        Debug.Log("Not close enough to my buildings");
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
        var value = playerName.Value;
        return value.ConvertToString();
    }
}
