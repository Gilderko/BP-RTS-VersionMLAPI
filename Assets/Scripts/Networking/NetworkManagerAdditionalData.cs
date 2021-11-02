using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManagerAdditionalData : MonoBehaviour
{
    [SerializeField] GameOverHandler gameOverHandler;
    [SerializeField] UnitBase unitBase;

    public UnitBase GetUnitBasePrefab()
    {
        return unitBase;
    }

    public GameOverHandler GetGameOverHandlerPrefab()
    {
        return gameOverHandler;
    }
}
