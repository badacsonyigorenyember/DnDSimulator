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

    public List<Creature> creatures = new();
    public GameObject map;

    public bool isPlaying;
    public bool isSceneLoaded;

    public SceneObject currentScene;

    public static GameManager Instance;

    public GameObject waitingScreenObj;
    public static string CREATURE_IMG_PATH;
    public static string CREATURE_DATA_PATH;
    public static string MAP_PATH;
    public static string SCENE_PATH;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        CREATURE_IMG_PATH = Application.dataPath + "/Resources/Images/Creatures";
        CREATURE_DATA_PATH = Application.dataPath + "/Resources/Data/Creatures";
        MAP_PATH = Application.dataPath + "/Resources/Images/Maps";
        SCENE_PATH = Application.dataPath + "/Resources/Data/Scenes";

        Directory.CreateDirectory(CREATURE_IMG_PATH);
        Directory.CreateDirectory(CREATURE_DATA_PATH);
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
        if (value) {
            string json = await CloudDataHandler.DownloadSceneData(sceneName);
            currentScene = JsonUtility.FromJson<SceneObject>(json);
        
            Debug.Log("Current scene set!");

            await CloudDataHandler.DownloadImages(currentScene.creatures.Select(e => e.creatureName).ToList());
        
            Debug.Log("Images downloaded!");

            SceneHandler.LoadCreatures();
        
            Debug.Log("Entities loaded!");

            await CloudDataHandler.DownloadMap(currentScene.name);
            
            Debug.Log("Map downloaded!");
            
            SceneHandler.LoadMap();
            
            Debug.Log("Map loaded!");
        }

        waitingScreenObj.SetActive(!value);
        isPlaying = value;
    }

    void OnClientConnected(ulong clientId) {
        if (isPlaying) {
            SetUpClient(currentScene.name, isPlaying);
        }
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
}


