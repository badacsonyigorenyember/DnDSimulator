using System;
using Models.Interfaces;
using UnityEngine;
using Utils.Data;

namespace Models
{
    public class Creature : Monster, IEntity
    {
        public string Uuid { get; set; }
        public Vector2 Position { get; set; }
        public bool Visible { get; set; }

        public Creature(CreatureData data) {
            Uuid = data.uuid;
            Visible = data.visible;
            Position = data.position;
            
            GetMonsterDataById(data.monsterId);
            
            if (Convert.ToBoolean(data.name)) {
                Name = data.name;
            }
            if (Convert.ToBoolean(data.maxHealth)) {
                MaxHealth = data.maxHealth;
            }
            if (Convert.ToBoolean(data.currentHealth)) {
                CurrentHealth = data.currentHealth;
            }
            if (Convert.ToBoolean(data.initiativeModifier)) {
                InitiativeModifier = data.initiativeModifier;
            }
            if (Convert.ToBoolean(data.armorClass)) {
                ArmorClass = data.armorClass;
            }
            if (Convert.ToBoolean(data.abilities)) {
                Abilities = data.abilities;
            }
        }
    }
}
