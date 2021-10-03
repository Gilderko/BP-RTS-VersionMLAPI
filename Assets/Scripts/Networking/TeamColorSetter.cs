using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.NetworkVariable;

public class TeamColorSetter : NetworkBehaviour
{
    [SerializeField] private Renderer[] colorRenderers = new Renderer[0];

    private NetworkVariable<Color> teamColor = new NetworkVariable<Color>(
        new NetworkVariableSettings() {WritePermission = NetworkVariablePermission.ServerOnly, ReadPermission = NetworkVariablePermission.Everyone });

    #region Server

    public override void NetworkStart()
    {
        if (IsClient)
        {
            teamColor.OnValueChanged += HandleTeamColorUpdated;
        }
        else if (IsServer)
        {
            Debug.Log((NetworkManager.Singleton as RTSNetworkManager).GetPlayerCount());
            Debug.Log($"Base owner is {OwnerClientId}");
            RTSPlayer player = (NetworkManager.Singleton as RTSNetworkManager).GetRTSPlayerByUID(OwnerClientId);

            teamColor.Value = player.GetTeamColor();
        }

        base.NetworkStart();
    }

    #endregion

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
