using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour
{
    public Button startStopButton;

    public List<Entity> entities = new();

    public bool isPlaying;
    public bool isSceneLoaded;

    public SceneObject currentScene;

    public static GameManager Instance;

    public GameObject waitingScreenObj;
    public static string ENTITY_IMG_PATH;
    public static string ENTITY_DATA_PATH;
    public static string MAP_PATH;
    public static string SCENE_PATH;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        ENTITY_IMG_PATH = Application.dataPath + "/Resources/Images/Entities";
        ENTITY_DATA_PATH = Application.dataPath + "/Resources/Data/Entities";
        MAP_PATH = Application.dataPath + "/Resources/Images/Maps";
        SCENE_PATH = Application.dataPath + "/Resources/Data/Scenes";

        Directory.CreateDirectory(ENTITY_IMG_PATH);
        Directory.CreateDirectory(ENTITY_DATA_PATH);
        Directory.CreateDirectory(MAP_PATH);
        Directory.CreateDirectory(SCENE_PATH);

        if (!IsServer) {
            waitingScreenObj.transform.parent.gameObject.SetActive(true);
            return;
        }

        startStopButton.onClick.AddListener(StartStopGame);
    }

    async void StartStopGame() {
        if (currentScene == null) return;
        
        isPlaying = !isPlaying;
        
        TextMeshProUGUI text = startStopButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        text.text = isPlaying ? "Stop" : "Start";

        List<Task> tasks = new();

        if (isPlaying) {
             tasks.Add(CloudDataHandler.UploadImages());
             tasks.Add(CloudDataHandler.UploadMap(currentScene.name));
             tasks.Add(CloudDataHandler.UploadSceneData(currentScene.name));
             
             await Task.WhenAll(tasks);
             
             Debug.Log("Finished uploading!");
        }
        
        StartGameClientRpc(isPlaying, currentScene.name);
        
    }

    [ClientRpc]
    void StartGameClientRpc(bool value, string sceneName) {
        if (IsServer) return;

        SetUpClient(sceneName, value);
    }

    async void SetUpClient(string sceneName, bool value) {
        string json = await CloudDataHandler.DownloadSceneData(sceneName);
        currentScene = JsonUtility.FromJson<SceneObject>(json);
        
        Debug.Log("Current scene set!");

        await CloudDataHandler.DownloadImages(currentScene.entities.Select(e => e.entityName).ToList());
        
        Debug.Log("Images downloaded!");

        SceneHandler.LoadEntities();
        
        Debug.Log("Finished Download");
        
        waitingScreenObj.SetActive(!value);
        isPlaying = value;
    }

    void OnClientConnected(ulong clientId) {
        
    }

    public override void OnNetworkDespawn() {
        base.OnNetworkDespawn();

        if (IsServer) {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        }
    }

    public override void OnNetworkSpawn() {
        if (IsServer) {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }
    }

    public class EntityName : INetworkSerializable
    {
        public string name;
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
            if (serializer.IsWriter)
            {
                serializer.GetFastBufferWriter().WriteValueSafe(name);
            }
            else
            {
                serializer.GetFastBufferReader().ReadValueSafe(out name);
            }
        }
    }
}


