using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Models;
using Models.Interfaces;
using FileHandling;
using JetBrains.Annotations;
using Models;
using Models.Interfaces;
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
        
        private float timer;
        public static SceneHandler Instance;

        private void Awake() {
            Instance = this;
        }

        private void Update() {
            timer += Time.deltaTime;

            if (timer >= autosaveInSeconds || Input.GetKeyDown(KeyCode.Period)) {
                timer = 0;
                SaveScene();
            }
        }

        public async void LoadScene(string sceneName) {
            if (GameManager.Instance.currentScene != null && GameManager.Instance.currentScene.name == sceneName) return;

            await SaveScene();
            ClearScene();

            string path = GameManager.SCENE_PATH + $"/{sceneName}.json";
            if (File.Exists(path)) {
                SceneData sceneData = JsonConvert.DeserializeObject<SceneData>(path);
                Scene scene = new Scene(sceneData);
                GameManager.Instance.currentScene = scene;
                Camera.main.transform.position = new Vector3(scene.camPosition.x, scene.camPosition.y, -10);
                Camera.main.orthographicSize = scene.zoomScale;

                LoadMap(sceneName);

                List<Task> tasks = new List<Task>();

                foreach ((string uuid, Creature creature) in scene.creatures) {   //TODO: player-eket is létre kell hozni
                    CreateCreature();
                    tasks.Add(LoadEntity(GameManager.Instance.entities[uuid], creature)); 
                }

                await Task.WhenAll(tasks);
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

        void CreateCreature() {
            GameObject obj = Instantiate(_creaturePrefab);

            obj.GetComponent<NetworkObject>().Spawn();

            if (!_creatureContainer.GetComponent<NetworkObject>().IsSpawned) {
                _creatureContainer.GetComponent<NetworkObject>().Spawn();
            }

            obj.GetComponent<NetworkObject>().TrySetParent(_creatureContainer);
        }

        public async Task LoadEntity<T>(EntityBehaviour entity, T entityModel) where T: IEntity {
            bool isCreature = entityModel.GetType() == typeof(Creature);
            entity = isCreature ? new CreatureBehaviour(entityModel) : new PlayerBehaviour(entityModel);    //TODO: ez itt kurvaszar, meg faszság

            Vector2 pos = entityModel.Position;
            
            entity.transform.position = new Vector3(pos.x, pos.y);

            string path = isCreature
                ? GameManager.CREATURE_IMG_PATH
                : GameManager.PLAYER_IMG_PATH;

            byte[] bytes = await File.ReadAllBytesAsync(path + $"/{entityModel.Uuid}.png");
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

            for (int i = GameManager.Instance.entities.Count - 1; i >= 0; i--) {
                GameManager.Instance.entities[i].GetComponent<NetworkObject>().Despawn();
            }

            GameManager.Instance.currentScene = null;
        }
    }
}
