using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Takes care of instantiating projectiles when the enemy is close enough.
/// </summary>
public class UnitFiring : NetworkBehaviour
{
    [SerializeField] private Targeter targeter = null;
    [SerializeField] private GameObject projectilePrefab = null;
    [SerializeField] private Transform projectileSpawnPoint = null;
    [SerializeField] private float fireRange = 5f;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private float rotationSpeed = 20f;

    private float lastFireTime;

    #region Server

#if UNITY_SERVER

    private void Update()
    {
        if (IsServer)
        {
            Targetable target = targeter.GetTarget();

            if (target == null)
            {
                return;
            }

            if (!CanFireAtTarget())
            {
                return;
            }

            Quaternion targetRotation = Quaternion.LookRotation(target.transform.position - transform.position);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            if (Time.time > (1 / fireRate) + lastFireTime)
            {
                Quaternion projectRotation = Quaternion.LookRotation(target.GetAimAtPoint().position - projectileSpawnPoint.position);

                GameObject projectileInstance = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectRotation);                
                
                projectileInstance.GetComponent<NetworkObject>().SpawnWithOwnership(OwnerClientId, true);

                lastFireTime = Time.time;
            }
        }        
    }

#endif

    private bool CanFireAtTarget()
    {
        return (targeter.GetTarget().transform.position - transform.position).sqrMagnitude <= fireRange * fireRange;
    }

    #endregion
}
