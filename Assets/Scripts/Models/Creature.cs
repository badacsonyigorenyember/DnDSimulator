using System;
using System.Numerics;
using Script.Models.Interfaces;
using Script.Utils.Data;

namespace Script.Models
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
