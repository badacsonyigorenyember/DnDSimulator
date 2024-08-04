using Script.Structs;
using UnityEngine;
using Utils.Interfaces;

namespace Utils.Data
{
    public class CustomCreatureData : IEntityData
    {
        public string uuid;
        public string name;
        public int maxHealth;
        public int currentHealth;
        public int initiativeModifier;
        public int armorClass;
        public bool visible;
        public Vector2 position;
        public Abilities abilities;
    }
}
