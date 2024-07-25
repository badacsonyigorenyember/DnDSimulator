using System.Collections.Generic;
using UnityEngine;

namespace Models
{
    public class Scene
    {
        public string name;
        public List<Creature> creatures;
        public List<Player> players;
        public float zoomScale;
        public Vector2 camPosition;

        public Scene(string name) {
            this.name = name;
            creatures = new List<Creature>();
            players = new List<Player>();
        }
    }
}
