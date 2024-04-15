using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using TMPro;
using Unity.IO.LowLevel.Unsafe;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneHandler : NetworkBehaviour
{
    [SerializeField] private Button _saveSceneButton;
    [SerializeField] private Button _createSceneButton;
    [SerializeField] private TMP_InputField _sceneNameInputField;
    [SerializeField] private TextMeshProUGUI _sceneNameTextField;
    
    public SceneObject currentScene;
    string path;

    public GameObject sceneListElementPrefab;
    public GameObject sceneListContainer;
    public GameObject sceneContainer;


    private List<GameObject> sceneListElemets = new();

    private void Start() {
        path = Application.dataPath + "/Saves/Scenes";
        
        _saveSceneButton.onClick.AddListener(SaveScene);
        _createSceneButton.onClick.AddListener(CreateNewScene);
        _sceneNameTextField.text = "";
        currentScene = null;
        
        LoadSavedScenes();
    }

    void CreateNewScene() {
        ClearScene();
        
        if(currentScene != null)
            SaveScene();

        if (_sceneNameInputField.text == string.Empty || _sceneNameInputField.text.Contains('.')) {
            Debug.Log("Not valid name!");
            return;
        }

        string sceneName = _sceneNameInputField.text;

        _sceneNameTextField.text = sceneName;
        
        currentScene = new SceneObject(sceneName);
    }

    void SaveScene() {
        if (currentScene == null) return;

        List<EntityDto> entities = new();

        foreach (var entity in FindObjectsOfType<Entity>().ToList()) {
            entity.position = entity.transform.position;
            entities.Add(EntityWriterHelper.EntityToEntityDto(entity));

            if (!GameManager.Instance.entities.Contains(entity)) {
                GameManager.Instance.entities.Add(entity);
            }
        }

        currentScene.entities = entities;

        Directory.CreateDirectory(path);
        File.WriteAllText(path + $"/{currentScene.name}.json", JsonUtility.ToJson(currentScene));
        
        LoadSavedScenes();
    }
    

    void LoadScene(string name) {
        if(!IsServer)
            return;
        
        SaveScene();
        ClearScene();

        var file = File.ReadAllText(path + $"/{name}.json");
        
        currentScene = JsonUtility.FromJson<SceneObject>(file);
        
        _sceneNameTextField.text = currentScene.name;
        
        Debug.Log(currentScene.name);

        foreach (var entityDto in currentScene.entities) {
            var obj = Instantiate(Resources.Load<GameObject>($"Prefabs/Entitites/{entityDto.entityName}"),
                entityDto.position, Quaternion.identity);

            Entity entity = obj.GetComponent<Entity>();

            GameManager.Instance.entities.Add(entity);

            NetworkObject netObj = obj.GetComponent<NetworkObject>();

            netObj.Spawn();
            netObj.transform.SetParent(sceneContainer.transform.GetChild(0));
            
        }

        var mapObj = Instantiate(Resources.Load<GameObject>($"Prefabs/Maps/{currentScene.name}"));

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
        
        if (Directory.Exists(path)) {
            string[] files = Directory.GetFiles(path);

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
