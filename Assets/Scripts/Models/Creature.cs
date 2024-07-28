using System;
using Models.Interfaces;
using Script.Structs;
using UnityEngine;
using Utils.Data;

namespace Models
{
    public class Creature : Monster, IEntity
    {
        public string Uuid { get; set; }
        public Vector2 Position { get; set; }
        public bool Visible { get; set; }
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
        
        public Creature(CreatureData data) {
            Uuid = data.uuid;
            Visible = data.visible;
            Position = data.position;
            CurrentHealth = data.currentHealth;
            
            GetMonsterDataById(data.monsterId);
            
            if (data.name is not null) {
                Name = data.name;
            }
            if (data.maxHealth is not null) {
                MaxHealth = (int) data.maxHealth;
            }
            if (data.initiativeModifier is not null) {
                InitiativeModifier = (int) data.initiativeModifier;
            }
            if (data.armorClass is not null) {
                ArmorClass = (int) data.armorClass;
            }
            if (data.abilities is not null) {
                Abilities = (Abilities) data.abilities;
            }
        }
    }
}
