using System;
using System.IO;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CreatureFileHandler : MonoBehaviour
{
    [SerializeField] private GameObject _creaturePrefab;

    public static CreatureFileHandler Instance;

    private void Awake() {
        Instance = this;
    }

    public async Task<CreatureData> LoadCreatureDataAsync(string name) {
        string json = await File.ReadAllTextAsync(GameManager.CREATURE_DATA_PATH + $"/{name}.json");

        CreatureData data = JsonUtility.FromJson<CreatureData>(json);

        return data;
    }

    public async Task<Texture2D> LoadCreatureImageAsync(string name) {
        byte[] bytes = await File.ReadAllBytesAsync(GameManager.CREATURE_IMG_PATH + $"/{name}.png");

        Texture2D texture = new Texture2D(1, 1);
        texture.LoadImage(bytes);

        return texture;
    }

    public async Task SpawnCreature(string creatureName, Vector3 position) {
        Task<CreatureData> creatureDataTask = LoadCreatureDataAsync(creatureName);
        Task<Texture2D> creatureImageTask = LoadCreatureImageAsync(creatureName);
        
        await Task.WhenAll(creatureDataTask, creatureImageTask);
        
        GameObject creatureObj = Instantiate(_creaturePrefab, position, Quaternion.identity);
        creatureObj.name = creatureName;

        Creature creature = creatureObj.GetComponent<Creature>();

        creature.Init(creatureDataTask.Result);
        creature.SetImage(creatureImageTask.Result);

        creatureObj.GetComponent<NetworkObject>().Spawn();
    }
}
