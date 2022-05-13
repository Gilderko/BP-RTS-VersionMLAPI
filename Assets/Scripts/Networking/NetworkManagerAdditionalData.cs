using UnityEngine;

/// <summary>
/// Additional data for the NetworkManager that the NetworkManager uses.
/// </summary>
public class NetworkManagerAdditionalData : MonoBehaviour
{
    [SerializeField] private GameOverHandler gameOverHandler;
    [SerializeField] private UnitBase unitBase;

    public UnitBase GetUnitBasePrefab()
    {
        return unitBase;
    }

    public GameOverHandler GetGameOverHandlerPrefab()
    {
        return gameOverHandler;
    }
}
