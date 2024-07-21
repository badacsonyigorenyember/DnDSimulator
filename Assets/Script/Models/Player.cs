using Script.Models.Interfaces;

namespace Script.Models
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
