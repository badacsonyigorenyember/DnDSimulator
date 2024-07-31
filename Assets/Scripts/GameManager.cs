using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FileHandling;
using FileHandling.Dto;
using Models;
using Network;
using Newtonsoft.Json;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Utils.Interfaces;

public class GameManager : NetworkBehaviour
{
    public Button startStopButton;

    public Dictionary<string, EntityBehaviour> entities = new();
    public bool isPlaying;
    public Scene currentScene;

    public static GameManager Instance;

    public GameObject waitingScreenObj;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        if (!IsServer) {
            NetworkManager.CustomMessagingManager.RegisterNamedMessageHandler("GetGameStateData", ReceiveGetGameStateDataMessage);
            
            waitingScreenObj.transform.parent.gameObject.SetActive(true);
            return;
        }

        currentScene = null;

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
            currentScene = JsonConvert.DeserializeObject<Scene>(gameState.sceneData);

            Debug.Log("Current scene set!");

            await CloudDataHandler.SaveCreatureImages(currentScene.creatures.Select(e => e.Value.Name).ToList(), gameState);

            Debug.Log("Images downloaded!");

            List<Task> loadCreatureTasks = new List<Task>();
            foreach (var creature in entities.Values) {
                loadCreatureTasks.Add(SceneHandler.Instance.LoadEntityToObject(creature, 
                    currentScene.creatures.ToList()
                        .Find(c => c.Value.Position == (Vector2)creature.transform.position).Value)
                );
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
