using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Component that handles current enemy target that the GameObject should attack.
/// </summary>
public class Targeter : NetworkBehaviour
{
    [SerializeField] private Targetable target;

    #region Server

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            GameOverHandler.ServerOnGameOver += ServerHandleGameOver;
        }

        base.OnNetworkSpawn();
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            GameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
        }
        base.OnNetworkDespawn();
    }


    private void ServerHandleGameOver()
    {
        ClearTarget();
    }

    [ServerRpc]
    public void CmdSetTargetServerRpc(ulong playerID, ulong instanceID)
    {
        var targetGameObject = NetworkManager.ConnectedClients[playerID].OwnedObjects.Find(obj => obj.NetworkObjectId == instanceID);

        if (targetGameObject == null)
        {
            return;
        }

        Targetable newTarget;

        if (!targetGameObject.TryGetComponent<Targetable>(out newTarget))
        {
            return;
        }

        target = newTarget;
    }

    public void ClearTarget()
    {
        target = null;
    }

    #endregion

    public Targetable GetTarget()
    {
        return target;
    }
}
