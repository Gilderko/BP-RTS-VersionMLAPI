using TMPro;
using UnityEngine;

public class PlayerLobbyUIInstance : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerName;

    public void SetName(string name)
    {
        playerName.text = name;
    }
}
