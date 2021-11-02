using MLAPI;
using MLAPI.NetworkVariable;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 100;

    private NetworkVariable<int> currentHealth = new NetworkVariable<int>(
        new NetworkVariableSettings() { WritePermission = NetworkVariablePermission.ServerOnly, ReadPermission = NetworkVariablePermission.Everyone });

    public event Action ServerOnDie;

    public event Action<int, int> ClientOnHealthUpdated;

    public override void NetworkStart()
    {
#if UNITY_SERVER
        if (IsServer)
        {
            currentHealth.Value = maxHealth;
            UnitBase.ServerOnPlayerDie += ServerHandlePlayerDie;
        }
#else
        if (IsClient)
        {
            currentHealth.OnValueChanged += HandeHealthUpdated;
        }
#endif
        base.NetworkStart();
    }

    private void OnDestroy()
    {
#if UNITY_SERVER
        if (IsServer)
        {
            UnitBase.ServerOnPlayerDie -= ServerHandlePlayerDie;
        }
#else
        if (IsClient)
        {
            currentHealth.OnValueChanged += HandeHealthUpdated;
        }
#endif
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
        ClientOnHealthUpdated(newHealth,maxHealth);
    }

#endregion
}
