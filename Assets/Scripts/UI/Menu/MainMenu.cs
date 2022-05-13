using Unity.Netcode;
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
        landingPagePanel.SetActive(false);

        NetworkManager.Singleton.StartServer();
    }
}
