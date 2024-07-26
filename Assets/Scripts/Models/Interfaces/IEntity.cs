using UnityEngine;

namespace Models.Interfaces
{
    public interface IEntity
    {
        public string Uuid { get; set; }
        public string Name { get; set; }
        public int MaxHealth { get; set; }
        public int CurrentHealth { get; set; }
        public int InitiativeModifier { get; set; }
        public int ArmorClass { get; set; }
        public bool Visible { get; set; }
        public Vector2 Position { get; set; }

        public int DoDamage(int damage);
        public int Heal(int amount);
    }
}