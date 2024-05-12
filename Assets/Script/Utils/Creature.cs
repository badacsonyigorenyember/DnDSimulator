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
    
    public void Init(CreatureData data) {
        this.creatureName = data.creatureName;
        this.currentHp = data.maxHp;
        this.maxHp = data.maxHp;
        this.isPlayer = data.isPlayer;
        this.position = transform.position;
        this.initiativeModifier = data.initiativeModifier;
    }

    public void SetImage(Texture2D texture) {
        GetComponent<SpriteRenderer>().sprite = Sprite.Create(
            texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f, 200f);
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
