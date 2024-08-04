using System.Collections.Generic;
using Models;
using Structs;

namespace Utils.Data
{
    public class SceneData
    {
        public string Name { get; set; }
        public List<string> Creatures { get; set; }
        public List<string> Players { get; set; }
        public float ZoomScale { get; set; }
        public Position CamPosition { get; set; }

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

        public SceneData() {}
    }
}
