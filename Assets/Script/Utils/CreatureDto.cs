using System;
using UnityEngine;

[Serializable]
public class CreatureDto
{
    public string creatureName;
    public int currentHp;
    public int maxHp;
    public bool isCharacter;
    public Vector2 position;
    public int initiativeModifier;
    public bool visible;
    public int armorClass;

    public CreatureDto(Creature entity) {
        this.creatureName = entity.creatureName;
        this.currentHp = entity.currentHp;
        this.maxHp = entity.maxHp;
        this.isCharacter = entity.isPlayer;
        this.position = entity.transform.position;
        this.initiativeModifier = entity.initiativeModifier;
        this.visible = entity.visible;
        this.armorClass = entity.armorClass;
    }
}
