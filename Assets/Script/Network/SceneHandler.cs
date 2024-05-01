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

        List<EntityDto> entities = new();

        foreach (var entity in FindObjectsOfType<Entity>().ToList()) {
            entity.position = entity.transform.position;
            entities.Add(EntityWriterHelper.EntityToEntityDto(entity));

            if (!GameManager.Instance.entities.Contains(entity)) {
                GameManager.Instance.entities.Add(entity);
            }
        }

        GameManager.Instance.currentScene.entities = entities;

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
        
        foreach (var entityDto in GameManager.Instance.currentScene.entities) {
            var obj = Instantiate(Resources.Load<GameObject>($"Prefabs/EntityPrefab"),
                entityDto.position, Quaternion.identity);
            obj.name = entityDto.entityName;

            Entity entity = obj.GetComponent<Entity>();
            EntityWriterHelper.EntityDtoToEntity(entity, entityDto);

            GameManager.Instance.entities.Add(entity);

            NetworkObject netObj = obj.GetComponent<NetworkObject>();

            netObj.Spawn();
            netObj.transform.SetParent(sceneContainer.transform.GetChild(0));
            
        }

        var mapObj = Instantiate(Resources.Load<GameObject>($"Prefabs/Maps/{GameManager.Instance.currentScene.name}"));

        NetworkObject mapNetObj = mapObj.GetComponent<NetworkObject>();
        
        mapNetObj.Spawn();
        mapNetObj.transform.SetParent(sceneContainer.transform.GetChild(1));
    }



    void ClearScene() {
        for (int i = GameManager.Instance.entities.Count - 1; i >= 0; i--) {
            Destroy(GameManager.Instance.entities[i].gameObject);
        }
        GameManager.Instance.entities.Clear();

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

}
