using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class BuildingButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Building representedBuilding = null;
    [SerializeField] private Image iconMage = null;
    [SerializeField] private TextMeshProUGUI priceText = null;
    [SerializeField] private LayerMask floorMask = new LayerMask();
    [SerializeField] private TextMeshProUGUI buildingName = null;

    [SerializeField] private Color canPlaceColor = new Color();
    [SerializeField] private Color canNotPlaceColor = new Color();

    private Camera mainCamera;
    private BoxCollider buildingCollider;
    private RTSPlayer player;
    private GameObject buildingPreviewInstance;
    private Renderer buildingRendererInstance;


    private void Start()
    {
        mainCamera = Camera.main;
        iconMage.sprite = representedBuilding.GetIcon();
        priceText.text = representedBuilding.GetPrice().ToString();
        buildingName.text = representedBuilding.GetName();
        buildingCollider = representedBuilding.GetComponent<BoxCollider>();

        if (NetworkManager.Singleton.IsClient)
        {
            player = (NetworkManager.Singleton as RTSNetworkManager).GetRTSPlayerByUID(NetworkManager.Singleton.LocalClientId);
        }        
    }

    private void Update()
    {
        if (buildingPreviewInstance == null)
        {
            return;
        }

        UpdateBuildingPreview();
    }


    private void UpdateBuildingPreview()
    {
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        RaycastHit hit;
        bool hasHit = Physics.Raycast(ray, out hit, Mathf.Infinity, floorMask);

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (hasHit)
            {
                player.CmdTryPlaceBuildingServerRpc(representedBuilding.GetID(), hit.point);
            }

            Destroy(buildingPreviewInstance);
            buildingRendererInstance = null;           
        }
        else if (hasHit)
        {
            buildingPreviewInstance.transform.position = hit.point;

            if (!buildingPreviewInstance.activeSelf)
            {
                buildingPreviewInstance.SetActive(true);
            }

            Color color = player.CanPlaceBuilding(buildingCollider, hit.point) ? canPlaceColor : canNotPlaceColor;

            foreach (Material material in buildingRendererInstance.materials)
            {
                material.SetColor("_BaseColor", color
                    );
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
        {
            return;
        }

        if (player.GetResources() < representedBuilding.GetPrice())
        {
            return;
        }

        //Debug.Log("Creating building");

        buildingPreviewInstance = Instantiate(representedBuilding.GetBuildingPreview());
        buildingRendererInstance = buildingPreviewInstance.GetComponentInChildren<Renderer>();

        buildingPreviewInstance.SetActive(false);
    }
}
