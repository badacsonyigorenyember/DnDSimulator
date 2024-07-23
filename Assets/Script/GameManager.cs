using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour
{
    public Button startStopButton;

    public List<Creature> creatures = new();
    public bool isPlaying;
    public SceneData currentScene;

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
        NetworkManager.Singleton.StartHost();
        
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

        currentScene = null;

        if (!IsServer) {
            NetworkManager.CustomMessagingManager.RegisterNamedMessageHandler("GetGameStateData", ReceiveGetGameStateDataMessage);
        }

        startStopButton.onClick.AddListener(StartStopGame);
    }

    async void StartStopGame() {
        if (currentScene == null) return;

        isPlaying = !isPlaying;

        TextMeshProUGUI text = startStopButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        text.text = isPlaying ? "Stop" : "Start";

        await SceneHandler.Instance.SaveScene();

        GameStateDto gameState = new GameStateDto();
        if (isPlaying) {
            gameState = await CloudDataHandler.GetGameStateDto(currentScene.name);

            Debug.Log("Game state object created!");
        }

        SendGameStateDataToClients(gameState);
    }

    private void SendGameStateDataToClients(GameStateDto gameState) {
        byte isPlayingFlag = (byte)(isPlaying ? 1 : 0);
        string gameStateJson = JsonConvert.SerializeObject(gameState);
        int bufferSize = FastBufferWriter.GetWriteSize(gameStateJson, true) + sizeof(byte);
        
        using (FastBufferWriter fastBufferWriter = new FastBufferWriter(bufferSize, Allocator.Temp)) {
            if (fastBufferWriter.TryBeginWrite(bufferSize)) {
                fastBufferWriter.WriteByteSafe(isPlayingFlag);
                fastBufferWriter.WriteValueSafe(gameStateJson, true);
                NetworkManager.CustomMessagingManager.SendNamedMessageToAll("GetGameStateData", fastBufferWriter);
                Debug.Log("Game state object sent! (Bytes: " + bufferSize + ")");
            } else {
                throw new Exception("Could not write \"GetGameStateData\" message!");
            }
        }
    }
    
    void ReceiveGetGameStateDataMessage(ulong senderId, FastBufferReader reader) {
        int length = reader.Length - reader.Position;
        
        if (reader.TryBeginRead(length)) {
            reader.ReadByteSafe(out byte isPlayingByte);
            reader.ReadValueSafe(out string gameStateJson, true);
            Debug.Log("Game state object received! (Bytes: " + length + ")");
            SetUpClient(Convert.ToBoolean(isPlayingByte), JsonConvert.DeserializeObject<GameStateDto>(gameStateJson));
        } else {
            throw new Exception("Could not read \"GetGameStateData\" message!");
        }
    }

    async void SetUpClient(bool isPlaying, GameStateDto gameState) {
        if (isPlaying) {
            currentScene = JsonConvert.DeserializeObject<SceneData>(gameState.sceneData);

            Debug.Log("Current scene set!");

            await CloudDataHandler.SaveCreatureImages(currentScene.creatures.Select(e => e.creatureName).ToList(), gameState);

            Debug.Log("Images downloaded!");

            List<Task> loadCreatureTasks = new List<Task>();
            foreach (var creature in creatures) {
                loadCreatureTasks.Add(SceneHandler.Instance.LoadCreature(creature, 
                    currentScene.creatures.Find(c => c.position == (Vector2)creature.transform.position)));
            }
            await Task.WhenAll(loadCreatureTasks);

            Debug.Log("Creatures loaded!");

            await CloudDataHandler.DownloadMap(currentScene.name, gameState);

            Debug.Log("Map downloaded!");

            SceneHandler.Instance.LoadMap(currentScene.name);

            Debug.Log("Map loaded!");
        }

        waitingScreenObj.SetActive(!isPlaying);
        this.isPlaying = isPlaying;
    }

    void OnClientConnected(ulong clientId) {
        if (isPlaying) {
            //SetUpClient(currentScene.name, isPlaying);
        }
    }

    public override void OnNetworkDespawn() {
        base.OnNetworkDespawn();

        if (IsServer) {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        }
    }
}
