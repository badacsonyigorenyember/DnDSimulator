using System;
using System.Numerics;
using Script.Models.Interfaces;

namespace Script.Models
{
    public class Creature : Monster
    {
        public Vector2 Position { get; set; }
        public bool Visible { get; set; }

        public Creature(string uuid, Vector2 position, bool visible) {
            Uuid = uuid;
            GetDataByUuid();
            Position = position;
            Visible = visible;
        }

        public void Serialize() {
            throw new NotImplementedException("Script.Models.Creature Serialize is not implemented yet");
        }
    }
}
