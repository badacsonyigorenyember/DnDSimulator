using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
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

    private SceneObject asd;

    private void Awake() {
        Instance = this;
    }

    private void Update() {
        timer += Time.deltaTime;

        if (timer >= autosaveInSeconds || Input.GetKeyDown(KeyCode.S)) {
            timer = 0;
            SaveScene();
        }
    }

    public async void LoadScene(string sceneName) {
        if (GameManager.Instance.currentScene != null && GameManager.Instance.currentScene.name == sceneName) return;
        
        SaveScene();
        ClearScene();
        
        string path = GameManager.SCENE_PATH + $"/{sceneName}.json";
        if (File.Exists(path)) {
            string json = await File.ReadAllTextAsync(path);
            SceneObject scene = JsonUtility.FromJson<SceneObject>(json);
            GameManager.Instance.currentScene = scene;

            LoadMap(sceneName);

            List<Task> tasks = new List<Task>();

            for(int i = 0; i < scene.creatures.Count; i++) {
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
        GameObject obj = Instantiate(_creaturePrefab, _creatureContainer);
        
        obj.GetComponent<NetworkObject>().Spawn();
    }

    public async Task LoadCreature(Creature creature,CreatureDto creatureDto) {
        CreatureDtoHandler.CreatureDtoToCreature(creature, creatureDto);

        creature.transform.position = creatureDto.position;

        byte[] bytes = await File.ReadAllBytesAsync(GameManager.CREATURE_IMG_PATH + $"/{creatureDto.creatureName}.png");
        Texture2D texture = new Texture2D(1, 1);
        texture.LoadImage(bytes);

        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), 
            Vector2.one * 0.5f, 200f);
        creature.GetComponent<SpriteRenderer>().sprite = sprite;
    }

    public async void SaveScene() {
        if (GameManager.Instance.currentScene == null) {
            Debug.LogError("NULL scene at save!");
            return;
        }
        
        SceneObject scene = GameManager.Instance.currentScene;

        foreach (var creature in GameManager.Instance.creatures) {
            scene.creatures.Add(CreatureDtoHandler.CreatureToCreatureDto(creature));    
        }

        string json = JsonUtility.ToJson(scene);
        await File.WriteAllTextAsync(GameManager.SCENE_PATH + $"/{scene.name}.json", json);
        
        Debug.Log("Saved!");
    }

    void ClearScene() {
        sceneObject.GetComponent<SpriteRenderer>().sprite = null;

        foreach (Transform child in _creatureContainer) {
            Destroy(child.gameObject);
        }

        GameManager.Instance.currentScene = null;
    }
    
    

    /*void CreateNewScene() {
        ClearScene();
        
        if(GameManager.Instance.currentScene != null)
            SaveScene();

        if (_sceneNameInputField.text == string.Empty || _sceneNameInputField.text.Contains('.')) {
            Debug.Log("Not valid name!");
            return;
        }

        string sceneName = _sceneNameInputField.text;

        _sceneNameTextField.text = sceneName;
        
        GameManager.Instance.currentScene = new SceneObject(sceneName);
    }

    void SaveScene() {
        if (GameManager.Instance.currentScene == null) return;

        List<CreatureDto> creatures = new();

        foreach (var creature in FindObjectsOfType<Creature>().ToList()) {
            creature.position = creature.transform.position;
            creatures.Add(CreatureDtoHandler.CreatureToCreatureDto(creature));

            if (!GameManager.Instance.creatures.Contains(creature)) {
                GameManager.Instance.creatures.Add(creature);
            }
        }

        GameManager.Instance.currentScene.creatures = creatures;

        File.WriteAllText(GameManager.SCENE_PATH + $"/{GameManager.Instance.currentScene.name}.json", JsonUtility.ToJson(GameManager.Instance.currentScene));
        
        LoadSavedScenes();
    }
    

    void LasdoadScene(string name) {
        if(!IsServer)
            return;
        
        ClearScene();

        var mapJson = File.ReadAllText(GameManager.SCENE_PATH + $"/{name}.json");
        GameManager.Instance.currentScene = JsonUtility.FromJson<SceneObject>(mapJson);
        
        _sceneNameTextField.text = GameManager.Instance.currentScene.name;
        
        foreach (var creatureDto in GameManager.Instance.currentScene.creatures) {
            var obj = Instantiate(Resources.Load<GameObject>($"Prefabs/CreaturePrefab"));
            obj.name = creatureDto.creatureName;

            NetworkObject netObj = obj.GetComponent<NetworkObject>();

            netObj.Spawn();
            netObj.transform.SetParent(sceneContainer.transform.GetChild(0));
        }

        var mapObj = Instantiate(Resources.Load<GameObject>($"Prefabs/MapPrefab"));

        NetworkObject mapNetObj = mapObj.GetComponent<NetworkObject>();
        
        mapNetObj.Spawn();
        mapNetObj.transform.SetParent(sceneContainer.transform.GetChild(1));
        
        GameManager.Instance.map = mapObj;

        LoadCreatures();
        LoadMap();
    }

    void ClearScene() {
        for (int i = GameManager.Instance.creatures.Count - 1; i >= 0; i--) {
            Destroy(GameManager.Instance.creatures[i].gameObject);
        }
        GameManager.Instance.creatures.Clear();

        if (sceneContainer.transform.GetChild(1).childCount > 0) {
            Destroy(sceneContainer.transform.GetChild(1).GetChild(0).gameObject);
        }

    }

    void LoadSavedScenes() {
        foreach (var element in sceneListElemets) {
            Destroy(element);
        }
        
        if (Directory.Exists(GameManager.SCENE_PATH)) {
            string[] files = Directory.GetFiles(GameManager.SCENE_PATH);

            foreach (var file in files) {
                string fileName = Path.GetFileName(file).Split(".")[0];

                if(file.Contains(".meta") || fileName == "")
                    continue;
                
                GameObject element = Instantiate(sceneListElementPrefab, sceneListContainer.transform);
                
                element.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = fileName;
                element.GetComponent<Button>().onClick.AddListener(delegate { LoadScene(fileName); });
                
                sceneListElemets.Add(element);
            }
        }
    }

    public static void LoadCreatures() {
        for (int i = 0; i < GameManager.Instance.creatures.Count; i++) {
            Creature creature = GameManager.Instance.creatures[i];
            
            CreatureDtoHandler.CreatureDtoToCreature(creature, GameManager.Instance.currentScene.creatures[i]);

            creature.transform.position = creature.position;

            string imgPath = GameManager.CREATURE_IMG_PATH + $"/{creature.creatureName}.png";

            if (File.Exists(imgPath)) {
                byte[] imgBytes = File.ReadAllBytes(imgPath);

                Texture2D texture = new Texture2D(1, 1);
                texture.LoadImage(imgBytes);

                creature.gameObject.GetComponent<SpriteRenderer>().sprite
                    = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f, 200f);

            }
            else {
                Debug.LogError("No Image!");
            }
        }
    }
    */

    
    
    
    

}
