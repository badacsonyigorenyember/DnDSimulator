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
        hostButton.onClick.AddListener(() => {
           NetworkManager.Singleton.StartHost();
           
           if(SceneManager.GetActiveScene().name != "Game")
            SceneHandler.LoadSceen("Game");
        });
        joinButton.onClick.AddListener(() => {
            NetworkManager.Singleton.StartClient();
        });
    }

    private void OnConnectedToServer() {
        Debug.Log("Connected");
    }
}
