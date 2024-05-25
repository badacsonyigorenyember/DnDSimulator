using UnityEngine;
using System;

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

    public CreatureDto(string creatureName, int currentHp, int maxHp, bool isCharacter, Vector2 position, int initiativeModifier, bool visible){
        this.creatureName = creatureName;
        this.currentHp = currentHp;
        this.maxHp = maxHp;
        this.isCharacter = isCharacter;
        this.position = position;
        this.initiativeModifier = initiativeModifier;
        this.visible = visible;

    }
 
}
