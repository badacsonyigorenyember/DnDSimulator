using System;
using Unity.Netcode;
using UnityEngine;

[Serializable]
public class Entity : NetworkBehaviour
{
    public string entityName;
    public int currentHp;
    public int maxHp;
    public bool isCharacter;
    public Vector2 position;
    public int initiativeModifier;

    private void Start() {
        gameObject.AddComponent<CircleCollider2D>();
    }

    public override void OnNetworkSpawn() {
        base.OnNetworkSpawn();
        
        GameManager.Instance.entities.Add(this);
    }

    public override void OnNetworkDespawn() {
        base.OnNetworkDespawn();

        GameManager.Instance.entities.Remove(this);
    }
}
