using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerHelper : MonoBehaviour
{
    public Button hostButton;
    public Button joinButton;
    private static NetworkManager manager;
    
    void Awake() {
       hostButton.onClick.AddListener(() => NetworkManager.Singleton.StartHost());
       joinButton.onClick.AddListener(() => NetworkManager.Singleton.StartClient());
    }
}
