using UnityEngine;
using System;

[Serializable]
public class EntityDto
{
    public string entityName;
    public int currentHp;
    public int maxHp;
    public bool isCharacter;
    public Vector2 position;
    public int initiativeModifier;
    
    public EntityDto(string entityName, int currentHp, int maxHp, bool isCharacter, Vector2 position, int initiativeModifier){
        this.entityName = entityName;
        this.currentHp = currentHp;
        this.maxHp = maxHp;
        this.isCharacter = isCharacter;
        this.position = position;
        this.initiativeModifier = initiativeModifier;

    }
 
}
