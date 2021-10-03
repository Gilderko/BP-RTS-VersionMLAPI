using MLAPI;
using MLAPI.Connection;
using MLAPI.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RTSNetworkManager : NetworkManager
{
    [SerializeField] private GameObject playerBase = null;
    [SerializeField] private GameOverHandler gameOverHandler = null;

    public static event System.Action ClientOnConnected;
    public static event System.Action ClientOnDisconnected;

    public List<RTSPlayer> Players { get; } = new List<RTSPlayer>();

    private bool isGameInProgress = false;


    private void Start()
    {        
        OnClientConnectedCallback += HandleClientConnected;
        OnClientDisconnectCallback += HandleClientDisconnected;
        NetworkSceneManager.OnSceneSwitched += OnServerSceneChanged;
    }

    private void OnServerSceneChanged()
    {
        if (IsServer)
        {
            if (SceneManager.GetActiveScene().name.StartsWith("Scene_Map"))
            {
                GameOverHandler gameOverHandlerInstance = Instantiate(gameOverHandler);         
                gameOverHandlerInstance.GetComponent<NetworkObject>().Spawn();

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

                    GameObject baseInstance = Instantiate(playerBase, parentToSpawnPoints.GetChild(index).position, Quaternion.identity);

                    baseInstance.GetComponent<NetworkObject>().ChangeOwnership(LocalClientId);
                    baseInstance.GetComponent<NetworkObject>().Spawn();

                    player.ChangeStartingPosition(baseInstance.transform.position);
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

            Players.Add(player);

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

        NetworkSceneManager.SwitchScene("Scene_map");

    }

    

    #endregion

    public RTSPlayer ClientGetRTSPlayerByUID(ulong UID)
    {
        return Players.Find(player => player.OwnerClientId == UID);
    }


}
