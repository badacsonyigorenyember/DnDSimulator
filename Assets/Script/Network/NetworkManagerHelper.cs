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

    public static NetworkManagerHelper Instance;

    private void Awake() {
        Instance = this;
    }
    
    public void StartHost() {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList) {
            if (ip.AddressFamily == AddressFamily.InterNetwork) {
                Debug.Log(ip);
                NetworkManager.Singleton.StartHost();
                NetworkManager.Singleton.SceneManager.LoadScene("Game", LoadSceneMode.Single);
                
                return;
            }
        }

        throw new Exception("No network adapters with an IPv4 address in the system!");
    }

    public void StartClient(string ip) {
        UnityTransport transport = GetComponent<UnityTransport>();
        transport.ConnectionData.Address = ip;
        NetworkManager.Singleton.StartClient();
    }
}
