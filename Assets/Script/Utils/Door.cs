using Unity.Netcode;
using UnityEngine;

public class Door : NetworkBehaviour
{
    public bool _isClosed;
    
    public override void OnNetworkSpawn() {
        base.OnNetworkSpawn();
        if (!IsServer)
            transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
        
        _isClosed = true;
    }
    
    public void ChangeDoorHitBox(bool value) {
        transform.GetChild(0).GetComponent<BoxCollider2D>().enabled = value;
    }

    [ClientRpc]
    public void OpenCloseClientRpc() {
        _isClosed = !_isClosed;
        transform.GetChild(0).GetChild(0).GetComponent<EdgeCollider2D>().enabled = _isClosed;
    }
}
