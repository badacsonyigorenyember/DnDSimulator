using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class ClientHandler : NetworkBehaviour
{
    public List<GameObject> notVisibleForClients = new();
    
    private void Start() {
        if (!IsServer) {
            foreach (var obj in notVisibleForClients) {
                obj.gameObject.SetActive(false);
            }
        }
    }
}
