using System.Collections.Generic;
using System.IO;
using System.Linq;
using FileHandling;
using Newtonsoft.Json;
using UnityEngine;
using Utils.Data;

namespace Models
{
    public class Scene
    {
        public string name;
        public Dictionary<string, Creature> creatures;
        public Dictionary<string, Player> players;
        public float zoomScale;
        public Vector2 camPosition;

        public Scene(SceneData data) {
            name = data.Name;
            creatures = new Dictionary<string, Creature>();
            players = new Dictionary<string, Player>();
            zoomScale = data.ZoomScale;
            camPosition = data.CamPosition;

            FileManager fileManager = FileManager.Instance;
            string creaturePath = fileManager.creaturePath;
            string playerPath = fileManager.playerPath;

            if (Directory.Exists(creaturePath)) {
                Dictionary<string, Creature> creatureJson = JsonConvert.DeserializeObject<Dictionary<string, Creature>>(File.ReadAllText(creaturePath));
                foreach (string uuid in data.Creatures) {
                    creatures[uuid] = creatureJson[uuid];
                }
            }

            if (Directory.Exists(playerPath)) {
                Dictionary<string, Player> playerJson = JsonConvert.DeserializeObject<Dictionary<string, Player>>(File.ReadAllText(playerPath));
                foreach (string uuid in data.Players) {
                    players[uuid] = playerJson[uuid];
                }
            }
        }
    }
}
