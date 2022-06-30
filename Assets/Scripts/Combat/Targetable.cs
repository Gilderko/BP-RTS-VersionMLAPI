using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Simple script that tells where should the enemies aim at and also works as a tag for enemies that we can attack.
/// </summary>
[System.Serializable]
public class Targetable : NetworkBehaviour
{
    [SerializeField] private Transform aimAtPoint = null;

    public Transform GetAimAtPoint()
    {
        return aimAtPoint;
    }
}
