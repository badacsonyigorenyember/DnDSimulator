using System.Collections.Generic;
using Models;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;

namespace Utils.Data
{
    public class SceneData
    {
        public string Name { get; set; }
        public List<string> Creatures { get; set; }
        public List<string> Players { get; set; }
        public float ZoomScale { get; set; }
        public Vector2 CamPosition { get; set; }

        public SceneData(Scene scene) {
            Creatures = new List<string>();
            Players = new List<string>();
            
            Name = scene.name;
            foreach (string uuid in scene.creatures.Keys) {
                Creatures.Add(uuid);
            }

            foreach (string uuid in scene.players.Keys) {
                Players.Add(uuid);
            }
        }

        public SceneData(string name) {
            Name = name;
            Creatures = new List<string>();
            Players = new List<string>();
        }
    }
}
