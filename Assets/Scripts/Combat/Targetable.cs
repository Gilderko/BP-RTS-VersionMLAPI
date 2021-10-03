using MLAPI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Targetable : NetworkBehaviour
{
    [SerializeField] private Transform aimAtPoint = null;

    public Transform GetAimAtPoint()
    {
        return aimAtPoint;
    }
}
