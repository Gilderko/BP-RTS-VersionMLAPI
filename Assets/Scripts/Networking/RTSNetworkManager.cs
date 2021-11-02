using Unity.Netcode;
using Unity.Netcode.Editor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RTSNetworkManager : NetworkManager
{
    public int value = 5;

    public static event System.Action ClientOnConnected;
    public static event System.Action ClientOnDisconnected;

    public List<RTSPlayer> Players { get; } = new List<RTSPlayer>();

    private bool isGameInProgress = false;

    private void Start()
    {        
        OnClientConnectedCallback += HandleClientConnected;
        OnClientDisconnectCallback += HandleClientDisconnected;
        OnServerStarted += ConfigureNetworkSceneManager;
    }

    private void ConfigureNetworkSceneManager()
    {
        SceneManager.OnSceneEvent += OnServerSceneChanged;
    }

    private void OnServerSceneChanged(SceneEvent sceneEvent)
    {
        if (IsServer)
        {
            if (sceneEvent.SceneName.StartsWith("Scene_Map"))
            {
                //GameOverHandler gameOverHandlerInstance = Instantiate(gameOverHandler);         
                //gameOverHandlerInstance.GetComponent<NetworkObject>().Spawn();

                Transform parentToSpawnPoints = GameObject.FindGameObjectWithTag("SpawnPoints").transform;

                List<int> occupiedIndexes = new List<int>();

                foreach (RTSPlayer player in Players)
                {
                    int index = 0;

                    while (true)
                    {
                        index = Random.Range(0, parentToSpawnPoints.childCount);
                        if (!occupiedIndexes.Contains(index))

                            occupiedIndexes.Add(index);
                        break;
                    }                    

                    /*GameObject baseInstance = Instantiate(playerBase, parentToSpawnPoints.GetChild(index).position, Quaternion.identity);

                    Debug.Log(player.OwnerClientId);

                    baseInstance.GetComponent<NetworkObject>().SpawnWithOwnership(player.OwnerClientId);                   

                    player.ChangeStartingPosition(baseInstance.transform.position);*/
                }
            }
        }       
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

        SceneManager.LoadScene("Scene_Map",UnityEngine.SceneManagement.LoadSceneMode.Single);
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
