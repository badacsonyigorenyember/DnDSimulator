using System.IO;
using System.Threading.Tasks;
using Models;
using Newtonsoft.Json;
using Unity.Netcode;
using UnityEngine;
using Utils;
using Utils.Data;

namespace FileHandling
{
    public class CreatureFileHandler : MonoBehaviour
    {   //TODO: átírni function-ökkel eggyütt entity-kre
        [SerializeField] private GameObject _creaturePrefab;

        [SerializeField] private Transform _creatureContainer;

        public static CreatureFileHandler Instance;

        private void Awake() {
            Instance = this;
        }

        public async Task<CreatureData> LoadCreatureDataAsync(string name) {
            string json = await File.ReadAllTextAsync(FileManager.Instance.creaturePath + $"/{name}.json");

            CreatureData data = JsonConvert.DeserializeObject<CreatureData>(json);

            return data;
        }

        public async Task<Texture2D> LoadCreatureImageAsync(string name) {
            byte[] bytes = await File.ReadAllBytesAsync(FileManager.Instance.creatureImgPath + $"/{name}.png");

            Texture2D texture = new Texture2D(1, 1);
            texture.LoadImage(bytes);

            return texture;
        }

        public async Task SpawnCreature(string creatureName, Vector3 position) {
            Task<CreatureData> creatureDataTask = LoadCreatureDataAsync(creatureName);
            Task<Texture2D> creatureImageTask = LoadCreatureImageAsync(creatureName);

            await Task.WhenAll(creatureDataTask, creatureImageTask);

            GameObject creatureObj = Instantiate(_creaturePrefab, position, Quaternion.identity);
            creatureObj.name = creatureName;

            CreatureBehaviour creatureBehaviour = creatureObj.GetComponent<CreatureBehaviour>();

            Creature creature = new Creature(creatureDataTask.Result);
            
            creatureBehaviour.Init(creature);
            creatureBehaviour.SetImage(creatureImageTask.Result);

            creatureObj.GetComponent<NetworkObject>().Spawn();

            if (!_creatureContainer.GetComponent<NetworkObject>().IsSpawned) {
                _creatureContainer.GetComponent<NetworkObject>().Spawn();
            }

            creatureObj.GetComponent<NetworkObject>().TrySetParent(_creatureContainer);
        }
    }
}
