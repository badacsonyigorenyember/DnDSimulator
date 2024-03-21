using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : NetworkBehaviour
{
    private void Start() {
        DontDestroyOnLoad(this);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.A)) {
            TestClientRpc();
        }
    }

    [ClientRpc]
    void TestClientRpc() {
        if (!IsServer) {
            SceneManager.LoadScene("Game");
        }
    }
}
