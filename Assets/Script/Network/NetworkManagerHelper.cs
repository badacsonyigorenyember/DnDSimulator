using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NetworkManagerHelper : MonoBehaviour
{
    public Button hostButton;
    public Button joinButton;
    private static NetworkManager manager;
    
    void Awake() {
        hostButton.onClick.AddListener(() => ConnectToGame(true));
        joinButton.onClick.AddListener(() => ConnectToGame(false));
    }

    void ConnectToGame(bool isServer) {
        if (isServer) {
            NetworkManager.Singleton.StartHost();
            NetworkManager.Singleton.SceneManager.LoadScene("Game", LoadSceneMode.Single);
        }
        else {
            NetworkManager.Singleton.StartClient();
        }
        
    }
    
    
}
