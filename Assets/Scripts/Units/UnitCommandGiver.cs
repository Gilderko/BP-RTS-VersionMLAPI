using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitCommandGiver : MonoBehaviour
{
    [SerializeField] private UnitSelectionHandler unitSelectionHandler = null;
    [SerializeField] private LayerMask layerMask = new LayerMask();

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;

        GameOverHandler.ClientOnGameOver += ClientHandleGameOver;
    }

    private void OnDestroy()
    {
        GameOverHandler.ClientOnGameOver -= ClientHandleGameOver;
    }

    private void ClientHandleGameOver(string obj)
    {
        enabled = false;
    }

    void Update()
    {
        if (!Mouse.current.rightButton.wasPressedThisFrame)
        {
            return;
        }

        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        RaycastHit hit;
        if (!Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            return;
        }

        if (hit.collider.TryGetComponent<Targetable>(out Targetable target))
        {
            if (target.IsOwner)
            {
                TryMove(hit.point);
                return;
            }

            TryTarget(target);
        }
        else
        {
            TryMove(hit.point);
        }        
    }

    private void TryTarget(Targetable target)
    {
        foreach (Unit unit in unitSelectionHandler.GetSelectedUnits())
        {
            unit.GetTargeter().CmdSetTargetServerRpc(target.gameObject);
        }
    }

    private void TryMove(Vector3 point)
    {
        foreach(Unit unit in unitSelectionHandler.GetSelectedUnits())
        {
            unit.GetUnitMovement().CmdMove(point);
        }
    }
}
