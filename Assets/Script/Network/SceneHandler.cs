using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class SceneHandler : NetworkBehaviour
{
    [SerializeField] private Button _saveSceneButton;
    [SerializeField] private Button _createSceneButton;
    [SerializeField] private TMP_InputField _sceneNameInputField;
    [SerializeField] private TextMeshProUGUI _sceneNameTextField;

    public GameObject sceneListElementPrefab;
    public GameObject sceneListContainer;
    public GameObject sceneContainer;
    
    private List<GameObject> sceneListElemets = new();

    private void Start() {
        _saveSceneButton.onClick.AddListener(SaveScene);
        _createSceneButton.onClick.AddListener(CreateNewScene);
        _sceneNameTextField.text = "";
        GameManager.Instance.currentScene = null;
        
        LoadSavedScenes();
    }

    void CreateNewScene() {
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
    

    void LoadScene(string name) {
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

    public static void LoadMap() {
        GameObject map = GameManager.Instance.map;

        string imgPath = GameManager.MAP_PATH + $"/{GameManager.Instance.currentScene.name}.png";
        
        Debug.Log(imgPath);
        if (File.Exists(imgPath)) {
            byte[] imgBytes = File.ReadAllBytes(imgPath);
            Texture2D texture = new Texture2D(1, 1);
            texture.LoadImage(imgBytes);

            map.GetComponent<SpriteRenderer>().sprite 
                = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f, 100f);
        }
        else {
            Debug.LogError("No Image!");
        }
        
        
    }
    
    
    

}
