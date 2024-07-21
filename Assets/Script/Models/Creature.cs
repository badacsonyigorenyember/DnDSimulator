using System;
using System.Numerics;
using Script.Models.Interfaces;

namespace Script.Models
{
    public class Creature : Monster
    {
        public Vector2 Position { get; set; }

        public Creature(string uuid, Vector2 position) {
            Uuid = uuid;
            GetDataByUuid();

            Position = position;
        }

        public void Serialize() {
            throw new NotImplementedException("Script.Models.Creature Serialize is not implemented yet");
        }
    }
}
