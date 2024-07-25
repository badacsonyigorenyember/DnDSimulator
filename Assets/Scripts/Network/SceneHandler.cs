using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Script.Utils;
using Unity.Netcode;
using UnityEngine;

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
            string json = await File.ReadAllTextAsync(path);
            SceneData scene = JsonUtility.FromJson<SceneData>(json);
            GameManager.Instance.currentScene = scene;
            Camera.main.transform.position = new Vector3(scene.camPosition.x, scene.camPosition.y, -10);
            Camera.main.orthographicSize = scene.zoomScale;

            LoadMap(sceneName);

            List<Task> tasks = new List<Task>();

            for (int i = 0; i < scene.creatures.Count; i++) {
                CreateCreature();
                tasks.Add(LoadCreature(GameManager.Instance.creatures[i], scene.creatures[i]));
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

    public async Task LoadCreature(CreatureBehaviour creatureBehaviour, CreatureDto creatureDto) {
        CreatureDtoHandler.CreatureDtoToCreature(creatureBehaviour, creatureDto);

        creatureBehaviour.transform.position = creatureDto.position;

        byte[] bytes = await File.ReadAllBytesAsync(GameManager.CREATURE_IMG_PATH + $"/{creatureDto.creatureName}.png");
        Texture2D texture = new Texture2D(1, 1);
        texture.LoadImage(bytes);

        creatureBehaviour.SetImage(texture);
    }

    public async Task SaveScene() {
        if (GameManager.Instance.currentScene == null) {
            Debug.Log("NULL scene at save!");
            return;
        }

        SceneData scene = GameManager.Instance.currentScene;
        scene.creatures.Clear();

        foreach (var creature in GameManager.Instance.creatures) {
            scene.creatures.Add(new CreatureDto(creature));
        }

        scene.camPosition = Camera.main.transform.position;
        scene.zoomScale = Camera.main.orthographicSize;

        string json = JsonUtility.ToJson(scene);
        await File.WriteAllTextAsync(GameManager.SCENE_PATH + $"/{scene.name}.json", json);

        Debug.Log("Saved at: " + GameManager.SCENE_PATH + $"/{scene.name}.json");
    }

    void ClearScene() {
        sceneObject.GetComponent<SpriteRenderer>().sprite = null;

        for (int i = GameManager.Instance.creatures.Count - 1; i >= 0; i--) {
            GameManager.Instance.creatures[i].GetComponent<NetworkObject>().Despawn();
        }

        GameManager.Instance.currentScene = null;
    }
}
