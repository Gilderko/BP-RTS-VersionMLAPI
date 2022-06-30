using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// Component that stores which building it is supposed to represent. Takes care of updating the preview and asking the server to spawn the building.
/// </summary>
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

#if !UNITY_SERVER

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

#endif

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

        buildingPreviewInstance = Instantiate(representedBuilding.GetBuildingPreview());
        buildingRendererInstance = buildingPreviewInstance.GetComponentInChildren<Renderer>();

        buildingPreviewInstance.SetActive(false);
    }
}
