using Unity.Netcode;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject landingPagePanel = null;


   /* public void Start()
    {
        HostServerCallback();
    }*/

    
    public void HostServerCallback()
    {
        //Debug.Log("Started server");
        landingPagePanel.SetActive(false);

        NetworkManager.Singleton.StartServer();
    }
}
