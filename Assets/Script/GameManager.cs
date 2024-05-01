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
        isPlaying = !isPlaying;
        
        TextMeshProUGUI text = startStopButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        text.text = isPlaying ? "Stop" : "Start";

        if (isPlaying) {
            await CloudDataHandler.UploadImages();
            await CloudDataHandler.UploadMap(currentScene.name);
            await CloudDataHandler.UploadSceneData(currentScene.name);
            
            Debug.Log("Finished uploading!");
        }
        
        StartGameClientRpc(isPlaying, GetEntityNameList());
        
    }

    [ClientRpc]
    void StartGameClientRpc(bool value, EntityName[] entityNames) {
        waitingScreenObj.SetActive(!value);
        isPlaying = value;

        if (!IsServer) {
            DownloadData(entityNames);
        
        }
    }

    async void DownloadData(EntityName[] entityNames) {
        await CloudDataHandler.DownloadImages(entityNames);
        
        Debug.Log("Finished Download");
    }

    void OnClientConnected(ulong clientId) {
        StartGameClientRpc(isPlaying, GetEntityNameList());
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

    EntityName[] GetEntityNameList() {
        return entities
            .Select(e => e.entityName)
            .Distinct()
            .Select(n => new EntityName {name = n})
            .ToArray();
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


