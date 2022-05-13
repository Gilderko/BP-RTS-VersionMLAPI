using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Basic projectile instantiated that has its speed set on start on both the client and the server. 
/// 
/// Triggers are solved on server.
/// </summary>
public class UnitProjectile : NetworkBehaviour
{
    [SerializeField] private Rigidbody rb = null;
    [SerializeField] private int damageToDeal = 20;
    [SerializeField] private float destroyAfterSeconds = 5f;
    [SerializeField] private float launchForce = 10f;

    public override void OnNetworkSpawn()
    {
        rb.velocity = transform.forward * launchForce;

        if (IsServer)
        {
            Invoke(nameof(DestroySelf), destroyAfterSeconds);
        }

        base.OnNetworkSpawn();
    }

    #region Server


    private void OnTriggerEnter(Collider other)
    {
        if (IsServer)
        {
            NetworkObject otherObjectIHit;
            if (!other.TryGetComponent<NetworkObject>(out otherObjectIHit))
            {
                return;
            }

            if (otherObjectIHit.OwnerClientId == OwnerClientId)
            {
                return;
            }

            Health health;
            if (!other.TryGetComponent<Health>(out health))
            {
                return;
            }

            health.DealDamage(damageToDeal);

            DestroySelf();
        }
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }

    #endregion
}
