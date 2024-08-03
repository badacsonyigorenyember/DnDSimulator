using Script.Structs;
using Structs;
using UnityEngine;
using Utils.Interfaces;

namespace Utils.Data
{
    public class CreatureData : IEntityData
    {
        public string uuid;
        public string name;
        public int? maxHealth;
        public int currentHealth;
        public int? initiativeModifier;
        public int? armorClass;
        public bool visible;
        public Position position;
        public Abilities? abilities;
        public string monsterId;    //TODO: customCreature ha itt lesz, akkor ez is nullable
    }
}
