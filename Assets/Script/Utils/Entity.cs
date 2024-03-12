using System;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class Entity : NetworkBehaviour
{
    public new string name;
    public int currentHp;
    public int maxHp;
    public bool isCharacter;
    public Vector2 position;
    public int initiativeModifier;

    public override void OnDestroy() {
        base.OnDestroy();
        
        foreach (Transform child in transform) {
            if (child.TryGetComponent(out NetworkObject netObj)) {
                netObj.Despawn();
            }
        }
        
        InitiativeHandler.Instance.RemoveEntityFromList(this);
    }
}
