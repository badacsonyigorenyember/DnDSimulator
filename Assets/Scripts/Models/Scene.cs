using System.Collections.Generic;
using System.IO;
using FileHandling;
using Newtonsoft.Json;
using UnityEngine;
using Utils.Data;

namespace Models
{
    public class Scene
    {
        public string name;
        public List<Creature> creatures;
        public List<Player> players;
        public float zoomScale;
        public Vector2 camPosition;

        public Scene(SceneData data) {
            name = data.Name;
            creatures = new List<Creature>();
            players = new List<Player>();
            zoomScale = data.ZoomScale;
            camPosition = data.CamPosition;

            FileManager fileManager = FileManager.Instance;
            string creaturePath = fileManager.creaturePath;
            string playerPath = fileManager.playerPath;

            if (Directory.Exists(creaturePath)) {
                creatures = JsonConvert.DeserializeObject<List<Creature>>(File.ReadAllText(creaturePath))
                    .FindAll(creature => data.Creatures.Contains(creature.Uuid));
            }

            if (Directory.Exists(playerPath)) {
                players = JsonConvert.DeserializeObject<List<Player>>(File.ReadAllText(playerPath))
                    .FindAll(player => data.Players.Contains(player.Uuid));
            }
        }
    }
}
