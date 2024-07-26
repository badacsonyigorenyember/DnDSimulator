using System.Collections.Generic;
using Models;
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
            foreach (Creature creature in scene.creatures) {
                Creatures.Add(creature.Uuid);
            }

            foreach (Player player in scene.players) {
                Players.Add(player.Uuid);
            }
        }
    }
}
