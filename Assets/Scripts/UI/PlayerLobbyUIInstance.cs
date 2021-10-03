using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerLobbyUIInstance : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerName;

    public void SetName(string name)
    {
        playerName.text = name;
    }
}
