using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Models;
using Models.Interfaces;
using FileHandling;
using Newtonsoft.Json;
using Unity.Netcode;
using UnityEngine;
using Utils;
using Utils.Data;
using Utils.Interfaces;

namespace Network
{
    public class SceneHandler : MonoBehaviour
    {
        [SerializeField] private Transform _creatureContainer;

        [SerializeField] private GameObject _creaturePrefab;
        [SerializeField] private GameObject sceneObject;

        public float autosaveInSeconds;
        
        private float _timer;
        public static SceneHandler Instance;

        private void Awake() {
            Instance = this;
        }

        private void Update() {
            _timer += Time.deltaTime;

            if (_timer >= autosaveInSeconds || Input.GetKeyDown(KeyCode.Period)) {
                _timer = 0;
                Debug.Log("Saved!");
                SaveScene();
            }
        }

        public async void LoadScene(string sceneName) {
            if (GameManager.Instance.currentScene != null && GameManager.Instance.currentScene.name == sceneName) return;

            await SaveScene();
            ClearScene();

            string path = FileManager.Instance.sceneFolderPath + $"{sceneName}.json";
            
            if (File.Exists(path)) {
                SceneData sceneData = JsonConvert.DeserializeObject<SceneData>(await File.ReadAllTextAsync(path));
                Scene scene = new Scene(sceneData);
                GameManager.Instance.currentScene = scene;

                Camera mainCam = Camera.main;
                mainCam.transform.position = new Vector3(scene.camPosition.x, scene.camPosition.y, -10);
                mainCam.orthographicSize = scene.zoomScale;

                LoadMap(sceneName); //TODO: Map-hoz még hozzá se nyúltunk??

                List<Task> loadTasks = new List<Task>();

                List<IEntity> entities = scene.creatures.Values.Cast<IEntity>().Concat(scene.players.Values).ToList();

                foreach (var entity in entities) {
                    loadTasks.Add(SpawnCreature(entity));
                }

                await Task.WhenAll(loadTasks);
                Debug.Log("Scene loaded!");
            }
            else {
                Debug.LogError($"No such scene at {path}!");
            }
        }

        public void LoadMap(string sceneName) {
            string imgPath = FileManager.Instance.sceneImgPath + $"/{sceneName}.png";

            if (File.Exists(imgPath)) {
                byte[] imgBytes = File.ReadAllBytes(imgPath);
                Texture2D texture = new Texture2D(1, 1);
                texture.LoadImage(imgBytes);

                sceneObject.GetComponent<SpriteRenderer>().sprite
                    = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f, 100f);
            }
            else {
                Debug.LogError($"No such image at {imgPath}");
            }
        }

        public async Task SpawnCreature(IEntity creature) {
            SpawnEntityObject(creature);
            await LoadEntityToObject(GameManager.Instance.entities[creature.Uuid]);
        }

        void SpawnEntityObject<T>(T entity) where T : IEntity {
            Debug.Log(entity.Uuid);
            GameObject obj = Instantiate(_creaturePrefab);
            
            //obj.GetComponent<EntityBehaviour>().Uuid = entity.Uuid;
            
            switch (entity) {
                case Creature creature:
                    CreatureBehaviour creatureBehaviour = obj.AddComponent<CreatureBehaviour>();
                    creatureBehaviour.Init(creature);
                    break;
                case Player player: 
                    PlayerBehaviour playerBehaviour = obj.AddComponent<PlayerBehaviour>();
                    playerBehaviour.Init(player);
                    break;
                default:
                    throw new Exception("Nem megfelelő típus!");
            }
            
            obj.transform.position = entity.Position.GetPosition();
            
            obj.GetComponent<NetworkObject>().Spawn();

            if (!_creatureContainer.GetComponent<NetworkObject>().IsSpawned) {
                _creatureContainer.GetComponent<NetworkObject>().Spawn();
            }

            obj.GetComponent<NetworkObject>().TrySetParent(_creatureContainer);
        }

        public async Task LoadEntityToObject(EntityBehaviour entity) {
            string imagePath = FileManager.MonsterManualImagesPath + $"{entity.Entity.Name}.png"; //TODO: lekezelni a custom képeket! 
            
            byte[] bytes = await File.ReadAllBytesAsync(imagePath);
            Texture2D texture = new Texture2D(1, 1);
            texture.LoadImage(bytes);

            entity.SetImage(texture);
        }

        public async Task SaveScene() {
            if (GameManager.Instance.currentScene == null) {
                Debug.Log("NULL scene at save!");
                return;
            }

            SceneData sceneData = new SceneData(GameManager.Instance.currentScene);
            sceneData.Creatures.Clear();

            Dictionary<string, Creature> creatures = new Dictionary<string, Creature>();
            Dictionary<string, Player> players = new Dictionary<string, Player>();

            foreach ((string uuid, EntityBehaviour entityBehaviour) in GameManager.Instance.entities) {
                switch (entityBehaviour.Entity) {
                    case Creature creature: {
                        sceneData.Creatures.Add(uuid);
                        creatures.Add(uuid, creature);
                        break;
                    }
                    case Player player: {
                        sceneData.Players.Add(uuid);
                        players.Add(uuid, player);
                        break;
                    }
                    default:
                        throw new Exception("Nem megfelelő az entity típusa!");
                }
            }
            
            FileManager fileManager = FileManager.Instance;

            Dictionary<string, Creature> creaturesInAdventure = JsonConvert.DeserializeObject<Dictionary<string, Creature>>(await File.ReadAllTextAsync(fileManager.creaturePath));

            var getAllCreatures = GetAllEntities(creaturesInAdventure, creatures); 
            string creaturesJson = JsonConvert.SerializeObject(getAllCreatures);
            Task creatureWriteTask = File.WriteAllTextAsync(fileManager.creaturePath, creaturesJson);
            
            Dictionary<string, Player> playersInAdventure = JsonConvert.DeserializeObject<Dictionary<string, Player>>(await File.ReadAllTextAsync(fileManager.playerPath));
            
            var getAllPlayers = GetAllEntities(playersInAdventure, players);
            string playersJson = JsonConvert.SerializeObject(getAllPlayers);
            Task playerWriteTask = File.WriteAllTextAsync(fileManager.playerPath, playersJson);
            
            sceneData.CamPosition = Camera.main.transform.position;
            sceneData.ZoomScale = Camera.main.orthographicSize;

            string sceneJson = JsonConvert.SerializeObject(sceneData, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            string sceneFolderPath = FileManager.Instance.sceneFolderPath;
            Task sceneWriteTask = File.WriteAllTextAsync(sceneFolderPath + $"/{sceneData.Name}.json", sceneJson);

            Task.WaitAll(creatureWriteTask, playerWriteTask, sceneWriteTask);

            Debug.Log("Saved at: " + sceneFolderPath + $"/{sceneData.Name}.json");
        }

        static Dictionary<string, T> GetAllEntities<T>(Dictionary<string, T> entitiesInAdventure, Dictionary<string, T> sceneEntities)
            where T : IEntity {
            if (entitiesInAdventure == null) {
                return sceneEntities;
            }
            
            foreach ((string uuid, T entity) in sceneEntities) {
                entitiesInAdventure.Remove(uuid);
                entitiesInAdventure.Add(uuid, entity);
            }

            return entitiesInAdventure;
        }

        void ClearScene() {
            sceneObject.GetComponent<SpriteRenderer>().sprite = null;

            foreach ((_, EntityBehaviour entity) in GameManager.Instance.entities) {
                entity.GetComponent<NetworkObject>().Despawn();
            }

            GameManager.Instance.currentScene = null;
        }
    }
}
