using System;
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
                SaveScene();
            }
        }

        public async void LoadScene(string sceneName) {
            if (GameManager.Instance.currentScene != null && GameManager.Instance.currentScene.name == sceneName) return;

            await SaveScene();
            ClearScene();

            string path = FileManager.Instance.sceneFolderPath + $"{sceneName}.json";
            
            if (File.Exists(path)) {
                SceneData sceneData = JsonConvert.DeserializeObject<SceneData>(path);
                Scene scene = new Scene(sceneData);
                GameManager.Instance.currentScene = scene;

                Camera mainCam = Camera.main;
                mainCam.transform.position = new Vector3(scene.camPosition.x, scene.camPosition.y, -10);
                mainCam.orthographicSize = scene.zoomScale;

                LoadMap(sceneName); //TODO: Map-hoz még hozzá se nyúltunk??

                List<Task> loadTasks = new List<Task>();

                foreach ((string uuid, Creature creature) in scene.creatures) {
                    SpawnEntityObject();
                    loadTasks.Add(LoadEntityToObject(GameManager.Instance.entities[uuid], creature)); 
                }
                
                foreach ((string uuid, Player player) in scene.players) {
                    SpawnEntityObject();
                    loadTasks.Add(LoadEntityToObject(GameManager.Instance.entities[uuid], player)); 
                }

                await Task.WhenAll(loadTasks);
                Debug.Log("Scene loaded!");
            }
            else {
                Debug.LogError($"No such scene at {path}!");
            }
        }

        public void LoadMap(string sceneName) {
            string imgPath = GameManager.MAP_PATH + $"/{sceneName}.png";

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

        void SpawnEntityObject() {
            GameObject obj = Instantiate(_creaturePrefab);

            obj.GetComponent<NetworkObject>().Spawn();

            if (!_creatureContainer.GetComponent<NetworkObject>().IsSpawned) {
                _creatureContainer.GetComponent<NetworkObject>().Spawn();
            }

            obj.GetComponent<NetworkObject>().TrySetParent(_creatureContainer);
        }

        public async Task LoadEntityToObject<T>(EntityBehaviour entity, T entityModel) where T: IEntity {
            switch (entityModel) {
                case Creature creature:
                    CreatureBehaviour creatureBehaviour = entity.gameObject.AddComponent<CreatureBehaviour>();
                    creatureBehaviour.Init(creature);
                    break;
                case Player player: 
                    PlayerBehaviour playerBehaviour = entity.gameObject.AddComponent<PlayerBehaviour>();
                    playerBehaviour.Init(player);
                    break;
                default:
                    throw new Exception("Nem megfelelő típus!");
            }
            
            entity.transform.position = entityModel.Position;

            string imagePath = FileManager.MonsterManualImagesPath + $"{entityModel.Name}.png"; //TODO: lekezelni a custom képeket! 
            
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

            List<Creature> creatures = new List<Creature>();
            List<Player> players = new List<Player>();

            foreach ((string uuid, EntityBehaviour entityBehaviour) in GameManager.Instance.entities) {
                switch (entityBehaviour.Entity) {
                    case Creature creature: {
                        sceneData.Creatures.Add(uuid);
                        creatures.Add(creature);
                        break;
                    }
                    case Player player: {
                        sceneData.Players.Add(uuid);
                        players.Add(player);
                        break;
                    }
                    default:
                        throw new Exception("Nem megfelelő az entity típusa!");
                }
            }
            
            FileManager fileManager = FileManager.Instance;

            List<Creature> creaturesInAdventure = JsonConvert.DeserializeObject<List<Creature>>(fileManager.creaturePath);
            
            string creaturesJson = JsonConvert.SerializeObject(GetAllEntities(creaturesInAdventure, creatures));
            Task creatureWriteTask = File.WriteAllTextAsync(fileManager.creaturePath, creaturesJson);
            creatureWriteTask.Start();
            
            List<Player> playersInAdventure = JsonConvert.DeserializeObject<List<Player>>(fileManager.playerPath);
            
            string playersJson = JsonConvert.SerializeObject(GetAllEntities(playersInAdventure, players));
            Task playerWriteTask = File.WriteAllTextAsync(fileManager.playerPath, playersJson);
            playerWriteTask.Start();
            
            sceneData.CamPosition = Camera.main.transform.position;
            sceneData.ZoomScale = Camera.main.orthographicSize;

            string sceneJson = JsonConvert.SerializeObject(sceneData);
            Task sceneWriteTask = File.WriteAllTextAsync(GameManager.SCENE_PATH + $"/{sceneData.Name}.json", sceneJson);
            sceneWriteTask.Start();

            Task.WaitAll(creatureWriteTask, playerWriteTask, sceneWriteTask);

            Debug.Log("Saved at: " + GameManager.SCENE_PATH + $"/{sceneData.Name}.json");
        }

        public static List<T> GetAllEntities<T>(List<T> entitiesInAdventure, List<T> sceneEntities) where T : IEntity{
            if (entitiesInAdventure != null) {
                foreach (T entity in sceneEntities) {
                    T matchingEntity = entitiesInAdventure.FirstOrDefault(e => e.Uuid == entity.Uuid);

                    if (matchingEntity != null) {
                        entitiesInAdventure.Remove(matchingEntity);
                        entitiesInAdventure.Add(entity);
                    } else {
                        entitiesInAdventure.Add(entity);
                    }
                }
            } else {
                entitiesInAdventure = sceneEntities;
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
