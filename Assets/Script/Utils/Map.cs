using Unity.Netcode;

public class Map : NetworkBehaviour
{
    public override void OnNetworkSpawn() {
        base.OnNetworkSpawn();
        GameManager.Instance.map = gameObject;
    }

    public override void OnNetworkDespawn() {
        GameManager.Instance.map = null;
        base.OnNetworkDespawn();

    }
}
