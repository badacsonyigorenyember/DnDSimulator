using System;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour
{
    public Button startStopButton;

    public List<Entity> entities = new();

    public bool isPlaying;

    public static GameManager Instance;

    public GameObject waitingScreenObj;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        if(!IsServer)
            waitingScreenObj.transform.parent.gameObject.SetActive(true);

        startStopButton.onClick.AddListener(StartStopGame);
    }

    void StartStopGame() {
        isPlaying = !isPlaying;
        
        TextMeshProUGUI text = startStopButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        text.text = isPlaying ? "Stop" : "Start";
        
        StartStopClientRpc(isPlaying);
    }

    [ClientRpc]
    void StartStopClientRpc(bool value) {
        waitingScreenObj.SetActive(!value);
        isPlaying = value;
    }

    /*void OnClientConnected(ulong clientId) {
        SendGameStateClientRpc(isPlaying);
    }

    public override void OnNetworkDespawn() {
        base.OnNetworkDespawn();

        if (IsServer) {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        }
    }

    public override void OnNetworkSpawn() {
        if (IsServer) {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }
    }*/
}
