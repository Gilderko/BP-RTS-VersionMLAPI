using MLAPI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject landingPagePanel = null;

    public void HostServerCallback()
    {
        landingPagePanel.SetActive(false);

        NetworkManager.Singleton.StartServer();
    }
}
