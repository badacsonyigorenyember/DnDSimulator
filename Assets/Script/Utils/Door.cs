using Unity.Netcode;
using UnityEngine;

public class Door : NetworkBehaviour
{
    private bool _closed;
    
    public override void OnNetworkSpawn() {
        base.OnNetworkSpawn();
        if (!IsServer)
            transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
        
        _closed = true;
    }

    public void ChangeDoorHitBox(bool value) {
        transform.GetChild(0).GetComponent<BoxCollider2D>().enabled = value;
    }

    [ClientRpc]
    public void OpenCloseClientRpc() {
        _closed = !_closed;
        transform.GetChild(0).GetChild(0).GetComponent<EdgeCollider2D>().enabled = _closed;
    }
}
