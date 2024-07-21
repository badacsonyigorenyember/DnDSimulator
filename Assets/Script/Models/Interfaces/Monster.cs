using System;

namespace Script.Models.Interfaces
{
    public abstract class Monster
    {
        public string Uuid { get; set; }
        public string Name { get; set; }
        public int MaxHealth { get; set; }
        public int CurrentHealth { get; set; }

        /**
         * Alkalmazza a paraméterben kapott sebzést
         * Visszatér CurrentHealth HP-val
         */
        public int DoDamage(int damage) {
            CurrentHealth -= damage;
            if (CurrentHealth < 0) {
                CurrentHealth = 0;
            }

            return CurrentHealth;
        }

        /**
         * Alkalmazza a paraméterben kapott értéket gyógyításként
         * Visszatér CurrentHealth HP-val
         */
        public int Heal(int amount) {
            if (CurrentHealth < 0) {
                throw new Exception("Kurva nagy a baj");        //TODO: átírni
            }
            CurrentHealth += amount;
            if (CurrentHealth > MaxHealth) {
                CurrentHealth = MaxHealth;
            }

            return CurrentHealth;
        }
    }
}
