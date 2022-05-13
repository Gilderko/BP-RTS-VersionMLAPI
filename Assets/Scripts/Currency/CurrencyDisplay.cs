using TMPro;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Displays the ammount of resources we have locally in UI.
/// </summary>
public class CurrencyDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI resourcesText = null;

    private RTSPlayer player;

#if !UNITY_SERVER

    private void Start()
    {
        if (NetworkManager.Singleton.IsClient)
        {
            player = (NetworkManager.Singleton as RTSNetworkManager).GetRTSPlayerByUID(NetworkManager.Singleton.LocalClientId);
            ClientHandleResourcesUpdated(player.GetResources());
            player.ClientOnResourcesUpdated += ClientHandleResourcesUpdated;
        }
    }

    private void OnDestroy()
    {
        player.ClientOnResourcesUpdated -= ClientHandleResourcesUpdated;
    }

#endif

    private void ClientHandleResourcesUpdated(int obj)
    {
        resourcesText.text = $"Gold {obj.ToString()}";
    }
}
