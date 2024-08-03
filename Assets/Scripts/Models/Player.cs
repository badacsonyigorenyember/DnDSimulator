using Models.Interfaces;
using Script.Structs;
using Structs;
using Utils.Data;
using Vector2 = UnityEngine.Vector2;

namespace Models
{
    public class Player : IEntity
    {
        public string Uuid { get; set; }
        public string Name { get; set; }
        public int MaxHealth { get; set; }
        public int CurrentHealth { get; set; }
        public int InitiativeModifier { get; set; }
        public int ArmorClass { get; set; }
        public bool Visible { get; set; }
        public Position Position { get; set; }

        public Abilities Abilities { get; set; }
        
        public Player(PlayerData data) {
            Uuid = data.uuid;
            Name = data.name;
            MaxHealth = data.maxHealth;
            CurrentHealth = data.currentHealth;
            InitiativeModifier = data.initiativeModifier;
            ArmorClass = data.armorClass;
            Visible = data.visible;
            Position = data.position;
            Abilities = data.abilities;
        }

        public int DoDamage(int damage) {
            CurrentHealth -= damage;
            if (CurrentHealth < 0) {
                CurrentHealth = 0;
            }

            return CurrentHealth;
        }

        public int Heal(int amount) {
            CurrentHealth += amount;
            if (CurrentHealth > MaxHealth) {
                CurrentHealth = MaxHealth;
            }

            return CurrentHealth;
        }
    }
}
