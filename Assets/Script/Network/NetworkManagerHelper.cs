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
           SceneManager.LoadScene("Game");
        });
        joinButton.onClick.AddListener(() => {
            NetworkManager.Singleton.StartClient();
            SceneManager.LoadScene("WaitLobby");
        });
    }

    private void OnConnectedToServer() {
        Debug.Log("Connected");
    }
}
