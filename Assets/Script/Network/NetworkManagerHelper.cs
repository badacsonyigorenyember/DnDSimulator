using System;
using System.Net;
using System.Net.Sockets;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NetworkManagerHelper : MonoBehaviour
{
    public Button hostButton;
    public Button joinButton;
    private static NetworkManager manager;

    [SerializeField] private TMP_InputField _ipInput;

    void Awake() {
        hostButton.onClick.AddListener(() => ConnectToGame(true));
        joinButton.onClick.AddListener(() => ConnectToGame(false));
    }

    void ConnectToGame(bool isServer) {
        if (isServer) {
            StartHost();
            NetworkManager.Singleton.SceneManager.LoadScene("Game", LoadSceneMode.Single);
        }
        else {
            StartClient();
        }
    }

    void StartHost() {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList) {
            if (ip.AddressFamily == AddressFamily.InterNetwork) {
                Debug.Log(ip);
                NetworkManager.Singleton.StartHost();
                return;
            }
        }

        throw new Exception("No network adapters with an IPv4 address in the system!");
    }

    void StartClient() {
        UnityTransport transport = GetComponent<UnityTransport>();
        transport.ConnectionData.Address = _ipInput.text;
        NetworkManager.Singleton.StartClient();
    }
}
