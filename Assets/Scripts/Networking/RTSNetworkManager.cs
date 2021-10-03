using MLAPI;
using MLAPI.Connection;
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

    public List<RTSPlayer> ClientPlayers { get; } = new List<RTSPlayer>();

    private bool isGameInProgress = false;

    private void Start()
    {
        
        OnClientConnectedCallback += HandleClientConnected;
        OnClientDisconnectCallback += HandleClientDisconnected;
    }

    private void HandleClientDisconnected(ulong obj)
    {
        if (IsClient)
        {
            ClientPlayers.Clear();
            ClientOnDisconnected?.Invoke();            
        }
        else if (IsServer)
        {
            ClientPlayers.RemoveAll(player => player.OwnerClientId == obj);
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
            RTSPlayer player = ConnectedClients[obj].PlayerObject.GetComponent<RTSPlayer>();

            player.SetPlayerName($"Player {ClientPlayers.Count}");

            player.SetTeamColor(Random.ColorHSV());

            player.SetPartyOwner(ClientPlayers.Count == 1);
        }
    }

    #region Server    



    [Server]
    public override void OnServerConnect(NetworkConnection conn)
    {
        if (!isGameInProgress) { return; }

        conn.Disconnect();
    }

    [Server]
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        RTSPlayer player = conn.identity.GetComponent<RTSPlayer>();

        ClientPlayers.Remove(player);

        base.OnServerDisconnect(conn);
    }

    [Server]
    public override void OnStopServer()
    {
        ClientPlayers.Clear();

        isGameInProgress = false;

        base.OnStopServer();
    }

    [Server]
    public void StartGame()
    {
        if (ClientPlayers.Count < 2) { return; }

        isGameInProgress = true;

        ServerChangeScene("Scene_Map");
    }

    [Server]
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        RTSPlayer player = conn.identity.GetComponent<RTSPlayer>();

        ClientPlayers.Add(player);

        player.SetPlayerName($"Player {ClientPlayers.Count}");

        player.SetTeamColor(Random.ColorHSV());

        player.SetPartyOwner(ClientPlayers.Count == 1);
    }

    [Server]
    public override void OnServerSceneChanged(string sceneName)
    {
        base.OnServerSceneChanged(sceneName);
        if (SceneManager.GetActiveScene().name.StartsWith("Scene_Map"))
        {
            GameOverHandler gameOverHandlerInstance = Instantiate(gameOverHandler);

            NetworkServer.Spawn(gameOverHandlerInstance.gameObject);

            foreach(RTSPlayer player in ClientPlayers)
            {
                GameObject baseInstance = Instantiate(playerBase, GetStartPosition().position, Quaternion.identity);
                Debug.Log(baseInstance.transform.position);
                NetworkServer.Spawn(baseInstance, player.connectionToClient);

                player.ChangeStartingPosition(baseInstance.transform.position);
            }  
        }
    }

    #endregion

    #region Client

    [Client]
    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);

        ClientOnConnected?.Invoke();
    }

    [Client]
    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);

        ClientOnDisconnected?.Invoke();
    }

    [Client]
    public override void OnStopClient()
    {
        base.OnStopClient();

        ClientPlayers.Clear();
    }

    public RTSPlayer ClientGetRTSPlayer()
    {
        return ClientSidePlayer;
    }

    #endregion
}
