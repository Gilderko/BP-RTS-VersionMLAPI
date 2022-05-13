using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Takes care of setting the path of the NavMeshAgent on the server bases on the target or the command to move to a certain place.
/// </summary>
public class UnitMovement : NetworkBehaviour
{
    [SerializeField] private NavMeshAgent agent = null;
    [SerializeField] private Targeter targeter = null;
    [SerializeField] private float chaseRange = 10f;

    #region Server

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            GameOverHandler.ServerOnGameOver += ServerHandleGameOverClientRpc;
        }
        base.OnNetworkSpawn();
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            GameOverHandler.ServerOnGameOver -= ServerHandleGameOverClientRpc;
        }
        base.OnNetworkDespawn();
    }

    [ServerRpc]
    public void CmdMoveServerRpc(Vector3 position)
    {
        ServerMove(position);
    }

    public void ServerMove(Vector3 position)
    {
        targeter.ClearTarget();

        NavMeshHit hit;
        if (!NavMesh.SamplePosition(position, out hit, 1f, NavMesh.AllAreas))
        {
            return;
        }

        agent.SetDestination(position);
    }

    #endregion

    [ClientRpc]
    private void ServerHandleGameOverClientRpc()
    {
        agent.ResetPath();
    }

#if UNITY_SERVER

    private void Update()
    {
        if (IsServer)
        {
            Targetable target = targeter.GetTarget();

            if (target != null)
            {
                if ((target.transform.position - transform.position).sqrMagnitude > chaseRange * chaseRange)
                {
                    agent.SetDestination(targeter.GetTarget().transform.position);
                }
                else if (agent.hasPath)
                {
                    agent.ResetPath();
                }
            }
            else
            {
                if (!agent.hasPath || agent.remainingDistance >= agent.stoppingDistance)
                {
                    return;
                }

                agent.ResetPath();
            }
        }        
    }

#endif

}
