using Unity.Netcode;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitSelectionHandler : MonoBehaviour
{
    [SerializeField] private RectTransform unitSelectionArea = null;
    
    [SerializeField] private LayerMask layerMask = new LayerMask();

    private Vector2 startPosition;

    private RTSPlayer player;
    private Camera mainCamera;

    [SerializeField] private List<Unit> selectedUnits = new List<Unit>();

    private void Start()
    {
        mainCamera = Camera.main;

        if (NetworkManager.Singleton.IsConnectedClient)
        {
            Debug.Log($"Selection handler looking for {NetworkManager.Singleton.LocalClientId}");
            player = (NetworkManager.Singleton as RTSNetworkManager).GetRTSPlayerByUID(NetworkManager.Singleton.LocalClientId);
        }

        Unit.AuthorityOnUnitDespawned += AuthorityHandleUnitDespawned;
        GameOverHandler.ClientOnGameOver += ClientHandleGameOver;       
    }

    private void ClientHandleGameOver(string winnerName)
    {
        enabled = false;
    }

    private void OnDestroy()
    {
        Unit.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned;
    }

    private void AuthorityHandleUnitDespawned(Unit unit)
    {
        selectedUnits.Remove(unit);
    }

#if (UNITY_SERVER == false)
    private void Update()
    {
        if (!NetworkManager.Singleton.IsConnectedClient)
        {
            return;
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            StartSelectionArea();
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            ClearSelectionArea();
        }
        else if (Mouse.current.leftButton.isPressed)
        {
            Debug.Log("Is pressed");    
            UpdateSelectionArea();
        }
    }
#endif

    private void StartSelectionArea()
    {
        if (!Keyboard.current.leftShiftKey.isPressed)
        {
            foreach (Unit selectedUnit in selectedUnits)
            {
                selectedUnit.Deselect();
            }

            selectedUnits.Clear();
        }        

        unitSelectionArea.gameObject.SetActive(true);

        startPosition = Mouse.current.position.ReadValue();

        UpdateSelectionArea();
    }

    private void UpdateSelectionArea()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
     
        float areaWidth = mousePosition.x - startPosition.x;
        float areaHeight = mousePosition.y - startPosition.y;
        
        unitSelectionArea.sizeDelta = new Vector2(Mathf.Abs(areaWidth), Mathf.Abs(areaHeight));
        unitSelectionArea.anchoredPosition = startPosition +
            new Vector2(areaWidth / 2, areaHeight / 2);
    }

    private void ClearSelectionArea()
    {
        unitSelectionArea.gameObject.SetActive(false);

        if (unitSelectionArea.sizeDelta.magnitude == 0)
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;
            if (!Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                return;
            }

            Unit unit;
            if (!hit.collider.TryGetComponent<Unit>(out unit))
            {
                return;
            }

            if (!unit.IsOwner)
            {
                return;
            }

            selectedUnits.Add(unit);

            foreach (Unit selectedUnit in selectedUnits)
            {
                selectedUnit.Select();
            }
        }
        else
        {
            Vector2 min = unitSelectionArea.anchoredPosition - unitSelectionArea.sizeDelta / 2;
            Vector2 max = unitSelectionArea.anchoredPosition + unitSelectionArea.sizeDelta / 2;

            Debug.Log($"You have amount of units {player.GetMyUnits().Count()}");

            foreach(Unit unit in player.GetMyUnits())
            {
                if (selectedUnits.Contains(unit))
                {
                    continue;
                }

                Vector3 screenPosition = mainCamera.WorldToScreenPoint(unit.transform.position);
                if (screenPosition.x > min.x && screenPosition.x < max.x
                    && screenPosition.y > min.y && screenPosition.y < max.y)
                {
                    selectedUnits.Add(unit);
                    unit.Select();
                }
            }
        }        
    }

    public IEnumerable<Unit> GetSelectedUnits()
    {
        return selectedUnits;
    }
}
