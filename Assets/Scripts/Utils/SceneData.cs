using System.Collections.Generic;
using Models;
using UnityEngine;

namespace Utils
{
    public class SceneData
    {
        public string name;
        public List<Creature> creatures;
        public List<Player> players;
        public float zoomScale;
        public Vector2 camPosition;

        public SceneData(string name) {
            this.name = name;
            creatures = new List<Creature>();
            players = new List<Player>();
        }
    }
}
