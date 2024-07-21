using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Debug = UnityEngine.Debug;

public static class CloudDataHandler
{

    public static async Task SaveCreatureImages(List<string> creatureNames, GameStateDto gameState) {
        List<Task> downloadTasks = new List<Task>();
        
        Directory.CreateDirectory(GameManager.CREATURE_IMG_PATH);

        foreach (var creature in creatureNames) {
            if (!File.Exists(GameManager.CREATURE_IMG_PATH + $"/{creature}.png")) {
                downloadTasks.Add(DownloadImageAsync(creature));
                byte[] creaturePicture = gameState.GetCreaturePictures().Find(it => )
                await File.WriteAllBytesAsync(GameManager.CREATURE_IMG_PATH + $"/{creature}.png", downloadTask.Result);
            }
        }

        await Task.WhenAll(downloadTasks);
    }

    static async Task DownloadImageAsync(string name) {
        StorageReference imageReference = storageReference.Child($"Images/Creatures/{name}.png");

        var downloadTask = imageReference.GetBytesAsync(10_000_000);
        try {
            await downloadTask;
        }
        catch (Exception e) {
            Debug.LogError(e.Message);
            return;
        }
        
        await File.WriteAllBytesAsync(GameManager.CREATURE_IMG_PATH + $"/{name}.png", downloadTask.Result);
    }
    
    public static async Task DownloadMap(string name) {
        StorageReference mapReference = storageReference.Child($"Images/Maps/{name}.png");

        var downloadTask = mapReference.GetBytesAsync(25_000_000);
        try {
            await downloadTask;
        }
        catch (Exception e) {
            Debug.LogError(e.Message);
            return;
        }

        await File.WriteAllBytesAsync(GameManager.MAP_PATH + $"/{name}.png", downloadTask.Result);
    }

    public static async Task<string> DownloadSceneData(string name) {
        StorageReference sceneReference = storageReference.Child($"Data/Scenes/{name}.json");
        string path = GameManager.SCENE_PATH + $"/{name}.json";

        var downloadTask = sceneReference.GetFileAsync(path);
        
        try {
            await downloadTask;
            return await File.ReadAllTextAsync(path);
        }
        catch (Exception e) {
            Debug.LogError(e.Message);
            return null;
        }
    }

    public static async Task<GameStateDto> GetGameStateDto(string sceneName) {
        var mapPictureTask = File.ReadAllBytesAsync(GameManager.MAP_PATH + $"/{sceneName}.png");
        var sceneDataTask = File.ReadAllTextAsync(GameManager.SCENE_PATH + $"/{sceneName}.json");
        var creaturePicturesTask = GetCreaturePictures();

        await Task.WhenAll(mapPictureTask, sceneDataTask, creaturePicturesTask);
        
        byte[] mapPicture = mapPictureTask.Result;
        string sceneData = sceneDataTask.Result;
        List<byte[]> creaturePictures = creaturePicturesTask.Result;
        return new GameStateDto(mapPicture, creaturePictures, sceneData);
    }

    private static async Task<List<byte[]>> GetCreaturePictures() {
        List<byte[]> creaturePictures = new List<byte[]>();
        string[] creatureImageNames = Directory.GetFiles(GameManager.CREATURE_IMG_PATH);
        foreach (var creatureImageName in creatureImageNames) {
            string creatureName = Path.GetFileNameWithoutExtension(creatureImageName);

            if (GameManager.Instance.creatures.Any(e => e.creatureName == creatureName)) {
                byte[] picture = await File.ReadAllBytesAsync(GameManager.CREATURE_IMG_PATH + $"/{creatureName}.png");
                creaturePictures.Add(picture);
            }
        }
        return creaturePictures;
    }
}
