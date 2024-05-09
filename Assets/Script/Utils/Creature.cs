using System;
using Unity.Netcode;
using UnityEngine;

[Serializable]
public class Creature : NetworkBehaviour
{
    public string creatureName;
    public int currentHp;
    public int maxHp;
    public bool isPlayer;
    public Vector2 position;
    public int initiativeModifier;

    private void Start() {
        gameObject.AddComponent<CircleCollider2D>();
    }

    public override void OnNetworkSpawn() {
        base.OnNetworkSpawn();
        
        GameManager.Instance.creatures.Add(this);
    }

    public override void OnNetworkDespawn() {
        base.OnNetworkDespawn();

        GameManager.Instance.creatures.Remove(this);
    }
}
