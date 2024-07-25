using System;
using System.Numerics;
using Models.Interfaces;
using Utils.Data;

namespace Models
{
    public class Creature : Monster, IEntity
    {
        public string Uuid { get; set; }
        public Vector2 Position { get; set; }
        public bool Visible { get; set; }

        public Creature(CreatureData data, bool visible) {
            MonsterId = data.uuid;
            GetDataByUuid(MonsterId);
            Position = data.position;
            Visible = visible;
        }

        public void Serialize() {
            throw new NotImplementedException("Script.Models.Creature Serialize is not implemented yet");
        }
    }
}
