using MLAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class UnitMovement : NetworkBehaviour
{
    [SerializeField] private NavMeshAgent agent = null;
    [SerializeField] private Targeter targeter = null;
    [SerializeField] private float chaseRange = 10f;

    #region Server

    [Server]
    public override void OnStartServer()
    {
        base.OnStartServer();
        GameOverHandler.ServerOnGameOver += ServerHandleGameOver;
    }

    [Server]
    private void ServerHandleGameOver()
    {
        agent.ResetPath();
    }

    [Server]
    public override void OnStopServer()
    {
        base.OnStopServer();
        GameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
    }

    [Server]
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

    [ServerCallback]
    private void Update()
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

    [Command]
    public void CmdMove
        (Vector3 position)
    {
        ServerMove(position);
    }

    #endregion

    #region Client    

    #endregion
}
