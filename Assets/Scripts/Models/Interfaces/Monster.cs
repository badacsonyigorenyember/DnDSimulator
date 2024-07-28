using System.Collections.Generic;
using FileHandling;
using Newtonsoft.Json;
using Script.Structs;

namespace Models.Interfaces
{
    public class Monster
    {
        public string MonsterId { get; set; }
        public string Name { get; set; }
        public int MaxHealth { get; set; }
        public int InitiativeModifier { get; set; }
        public int ArmorClass { get; set; }

        public Abilities Abilities { get; set; }

        /**
         * Kiolvassa a kapott monster adatait a json-ből
         */
        protected void GetMonsterDataById(string monsterId) {
            Monster monster = JsonConvert.DeserializeObject<Dictionary<string, Monster>>(FileManager.MonsterManualPath)[monsterId];
            
            MonsterId = monster.MonsterId;
            Name = monster.Name;
            MaxHealth = monster.MaxHealth;
            InitiativeModifier = monster.InitiativeModifier;
            ArmorClass = monster.ArmorClass;
            Abilities = monster.Abilities;
        }
    }
}
