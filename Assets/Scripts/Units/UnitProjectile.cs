using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

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


#if UNITY_SERVER
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
#endif
 
    private void DestroySelf()
    {
        Destroy(gameObject);
    }

#endregion
}
