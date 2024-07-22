using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        startStopButton.onClick.AddListener(StartStopGame);
        
        //TODO
        NetworkManager.CustomMessagingManager.RegisterNamedMessageHandler("GetGameStateData", ReceiveMessage);

    }

    async void StartStopGame() {
        if (currentScene == null) return;

        isPlaying = !isPlaying;
        
        TextMeshProUGUI text = startStopButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        text.text = isPlaying ? "Stop" : "Start";

        List<Task> tasks = new();

        await SceneHandler.Instance.SaveScene();

        GameStateDto gameState = new GameStateDto();
        if (isPlaying) {
            gameState = await CloudDataHandler.GetGameStateDto(currentScene.name);

            Debug.Log("Finished uploading!");
        }

        //TODO beállítani, hogy a GameStateDto-t küldje el
        var data = await File.ReadAllBytesAsync(GameManager.MAP_PATH + $"/{currentScene.name}.png");
        var asd = new FastBufferWriter(data.Length, Allocator.Temp);
        if (asd.TryBeginWrite(data.Length)) {
            asd.WriteBytes(data);
            NetworkManager.CustomMessagingManager.SendNamedMessageToAll("GetGameStateData", asd);
        }

        //StartGameClientRpc(isPlaying, currentScene.name, gameState);
    }
    
    //TODO beállítani a GameStateDto-nak megfelelően
    void ReceiveMessage(ulong senderId, FastBufferReader reader) {
        int length = reader.Length - reader.Position;

        if (reader.TryBeginRead(length)) {
            byte[] s = new byte[length];
            reader.ReadBytes(ref s, length);
            Debug.Log(s.Length);
        }
        
    }
    
    [ClientRpc]
    void StartGameClientRpc(bool isPlaying, string sceneName, GameStateDto gameState) {
        if (IsServer) return;

        SetUpClient(sceneName, isPlaying, gameState);
    }

    async void SetUpClient(string sceneName, bool isPlaying, GameStateDto gameState) {
        if (isPlaying) {
            currentScene = JsonUtility.FromJson<SceneData>(gameState.GetSceneData());

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
