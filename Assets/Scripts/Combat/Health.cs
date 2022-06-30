using System;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Basic health component where all damage dealing is handled on server.
/// </summary>
public class Health : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 100;

    private NetworkVariable<int> currentHealth = new NetworkVariable<int>(NetworkVariableReadPermission.Everyone, 1);

    public event Action ServerOnDie;

    public event Action<int, int> ClientOnHealthUpdated;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            currentHealth.Value = maxHealth;
            UnitBase.ServerOnPlayerDie += ServerHandlePlayerDie;
        }

        if (IsClient)
        {
            currentHealth.OnValueChanged += HandeHealthUpdated;
        }

        base.OnNetworkSpawn();
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            UnitBase.ServerOnPlayerDie -= ServerHandlePlayerDie;
        }

        if (IsClient)
        {
            currentHealth.OnValueChanged += HandeHealthUpdated;
        }

        base.OnNetworkDespawn();
    }

    #region Server

    private void ServerHandlePlayerDie(ulong connectionID)
    {
        if (connectionID != OwnerClientId)
        {
            return;
        }

        DealDamage(currentHealth.Value);
    }

    public void DealDamage(int damageAmount)
    {
        if (currentHealth.Value <= 0)
        {
            return;
        }

        currentHealth.Value = Mathf.Clamp(currentHealth.Value - damageAmount, 0, maxHealth);

        if (currentHealth.Value != 0)
        {
            return;
        }

        ServerOnDie?.Invoke();
    }

    #endregion

    #region Client

    private void HandeHealthUpdated(int oldHealth, int newHealth)
    {
        ClientOnHealthUpdated(newHealth, maxHealth);
    }

    #endregion
}
