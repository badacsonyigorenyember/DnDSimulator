using System;
using Models.Interfaces;
using Structs;

namespace Models
{
    public class Creature : Monster, IEntity
    {
        public string Uuid { get; set; }
        public Position Position { get; set; }
        public bool Visible { get; set; }
        public int CurrentHealth { get; set; }

        public Creature() { } 

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

        public Creature(string monsterId) {
            Uuid = Guid.NewGuid().ToString();
            
            GetMonsterDataById(monsterId);

            Visible = true;
        }
    }
}
