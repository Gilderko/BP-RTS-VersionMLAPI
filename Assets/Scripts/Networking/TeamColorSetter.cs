using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Needs to be added to every unit and building to distinguish their color. Color can be changed during the match and will be updated automatically.
/// </summary>
public class TeamColorSetter : NetworkBehaviour
{
    [SerializeField] private Renderer[] colorRenderers = new Renderer[0];

    private NetworkVariable<Color> teamColor = new NetworkVariable<Color>(NetworkVariableReadPermission.Everyone, new Color());

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            teamColor.OnValueChanged += HandleTeamColorUpdated;
        }
        else if (IsServer)
        {
            RTSPlayer player = (NetworkManager.Singleton as RTSNetworkManager).GetRTSPlayerByUID(OwnerClientId);

            teamColor.Value = player.GetTeamColor();
        }

        base.OnNetworkSpawn();
    }

    #region Client

    private void HandleTeamColorUpdated(Color oldColor, Color newColor)
    {
        foreach (Renderer render in colorRenderers)
        {
            foreach (Material material in render.materials)
            {
                material.SetColor("_BaseColor", newColor);
            }
        }
    }

    #endregion
}
