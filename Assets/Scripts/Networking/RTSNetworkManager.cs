using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// The core component of this game. Created by deriving from the base NetworkManager.
/// 
/// Stores the players and handles logic related to new player connecting and disconnecting + starting the game.
/// </summary>
[RequireComponent(typeof(NetworkManagerAdditionalData))]
public class RTSNetworkManager : NetworkManager
{
    public static event System.Action ClientOnConnected;
    public static event System.Action ClientOnDisconnected;

    public List<RTSPlayer> Players { get; } = new List<RTSPlayer>();

    private bool isGameInProgress = false;
    private NetworkManagerAdditionalData additionalData;

    private void Awake()
    {
        additionalData = GetComponent<NetworkManagerAdditionalData>();
        OnClientConnectedCallback += HandleClientConnected;
        OnClientDisconnectCallback += HandleClientDisconnected;
        OnServerStarted += ConfigureNetworkSceneManager;
        Application.quitting += () => Shutdown();
    }

    private void ConfigureNetworkSceneManager()
    {
        SceneManager.OnLoadEventCompleted += OnServerSceneChanged;
    }

    private void HandleClientDisconnected(ulong obj)
    {
        if (IsClient)
        {
            Players.Clear();
            ClientOnDisconnected?.Invoke();
        }
        else if (IsServer)
        {
            Players.RemoveAll(player => player.OwnerClientId == obj);
        }
    }

    private void HandleClientConnected(ulong obj)
    {
        if (IsClient)
        {
            ClientOnConnected?.Invoke();
        }
        else if (IsServer)
        {
            if (isGameInProgress)
            {
                DisconnectClient(obj);
                return;
            }

            RTSPlayer player = ConnectedClients[obj].PlayerObject.GetComponent<RTSPlayer>();

            player.SetPlayerName($"Player {Players.Count}");

            player.SetTeamColor(Random.ColorHSV());

            player.SetPartyOwner(Players.Count == 1);
        }
    }

    public void OnDestroy()
    {
        if (IsServer)
        {
            Players.Clear();

            isGameInProgress = false;
        }
    }

    #region Server  

    public void StartGame()
    {
        if (Players.Count < 2) { return; }

        isGameInProgress = true;

        SceneManager.LoadScene("Scene_Map", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    private void OnServerSceneChanged(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        if (sceneName != "Scene_Map")
        {
            return;
        }

        GameOverHandler gameOverHandlerInstance = Instantiate(additionalData.GetGameOverHandlerPrefab());
        gameOverHandlerInstance.GetComponent<NetworkObject>().Spawn(true);

        Transform parentToSpawnPoints = GameObject.FindGameObjectWithTag("SpawnPoints").transform;

        HashSet<int> occupiedIndexes = new HashSet<int>();

        foreach (RTSPlayer player in Players)
        {
            int index = 0;

            while (true)
            {
                index = Random.Range(0, parentToSpawnPoints.childCount);
                if (!occupiedIndexes.Contains(index))
                {
                    occupiedIndexes.Add(index);
                    break;
                }
            }

            UnitBase baseInstance = Instantiate(additionalData.GetUnitBasePrefab(), parentToSpawnPoints.GetChild(index).position, Quaternion.identity);

            baseInstance.GetComponent<NetworkObject>().SpawnWithOwnership(player.OwnerClientId, true);

            player.ChangeStartingPosition(baseInstance.transform.position);
        }
    }

    #endregion

    public RTSPlayer GetRTSPlayerByUID(ulong UID)
    {
        return Players.Find(player => player.OwnerClientId == UID);
    }

    public int GetPlayerCount()
    {
        return Players.Count;
    }
}
