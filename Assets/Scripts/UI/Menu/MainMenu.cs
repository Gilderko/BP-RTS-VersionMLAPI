using Unity.Netcode;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject landingPagePanel = null;

#if UNITY_SERVER
    public void Start()
    {
        HostServerCallback();
    }
#endif
    
    public void HostServerCallback()
    {
        Debug.Log("Started server");
        landingPagePanel.SetActive(false);

        NetworkManager.Singleton.StartServer();
    }
}
