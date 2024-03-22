using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Script.Utils;
using Unity.IO.LowLevel.Unsafe;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : NetworkBehaviour
{
    public SceneHelper currentScene;
    string path;
    
    private void Start() {
        DontDestroyOnLoad(this);
        path = Application.dataPath + "/Saves/Scenes";
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.A)) {
            SaveScene();
        }
        if (Input.GetKeyDown(KeyCode.D)) {
            LoadScene();
        }
        if (Input.GetKeyDown(KeyCode.S)) {
            CreateNewScene("newScene");
        }
        if (Input.GetKeyDown(KeyCode.W)) {
            TestClientRpc();
        }
    }

    [ClientRpc]
    void TestClientRpc() {
        if (!IsServer) {
            SceneManager.LoadScene("Game");
        }
    }

    void SaveScene() {
        if (currentScene == null) return;
        
        Directory.CreateDirectory(path);
        File.WriteAllText(path + $"/{currentScene.sceneName}.json", JsonUtility.ToJson(currentScene));
    }

    void LoadScene() {
        if (!IsServer) return;
        
        var file = File.ReadAllText(path + "/newScene.json");

        currentScene = JsonUtility.FromJson<SceneHelper>(file);

        foreach (var entityDto in currentScene.entities) {
            var obj = Instantiate(Resources.Load<GameObject>("Prefabs/EntityPrefab"), entityDto.position, Quaternion.identity);
            NetworkObject netObj = obj.GetComponent<NetworkObject>();
            
            netObj.Spawn();
            LoadEntityClientRpc(entityDto, netObj);
        }

    }

    [ClientRpc]
    void LoadEntityClientRpc(EntityDto entityDto, NetworkObjectReference obj) {
        GameObject entityObj = obj;
        
        entityObj.name = entityDto.entityName; //Transparency

        Entity entity = entityObj.GetComponent<Entity>();
        FileHandler.EntityDtoToEntity(entity, entityDto);
        LoadEntitySprite(entity);
    }

    void LoadEntitySprite(Entity entity) {
        var bytes = File.ReadAllBytes(Application.dataPath + $"/Resources/Entities/{entity.entityName}.png");
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(bytes);

        entity.gameObject.GetComponent<SpriteRenderer>().sprite = Sprite.Create(
            texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero, 200);
    }
  

    void CreateNewScene(string sceneName) {
        if(currentScene != null)
            SaveScene();
        
        currentScene = new SceneHelper(sceneName, FindObjectsOfType<Entity>().ToList());

    }
}
