using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public static class CloudDataHandler
{

    public static async Task SaveCreatureImages(List<string> creatureNames, GameStateDto gameState) {
        List<Task> downloadTasks = new List<Task>();
        
        Directory.CreateDirectory(GameManager.CREATURE_IMG_PATH);

        foreach (var creature in creatureNames) {
            if (!File.Exists(GameManager.CREATURE_IMG_PATH + $"/{creature}.png")) {
                byte[] creaturePicture = gameState.GetCreaturePictures()[creature];
                downloadTasks.Add(File.WriteAllBytesAsync(GameManager.CREATURE_IMG_PATH + $"/{creature}.png", creaturePicture));
            }
        }

        await Task.WhenAll(downloadTasks);
    }

    public static async Task<GameStateDto> GetGameStateDto(string sceneName) {
        var mapPictureTask = File.ReadAllBytesAsync(GameManager.MAP_PATH + $"/{sceneName}.png");
        var sceneDataTask = File.ReadAllTextAsync(GameManager.SCENE_PATH + $"/{sceneName}.json");
        var creaturePicturesTask = GetCreaturePictures();

        await Task.WhenAll(mapPictureTask, sceneDataTask, creaturePicturesTask);
        
        byte[] mapPicture = mapPictureTask.Result;
        string sceneData = sceneDataTask.Result;
        Dictionary<string, byte[]> creaturePictures = creaturePicturesTask.Result;
        return new GameStateDto(mapPicture, creaturePictures, sceneData);
    }

    private static async Task<Dictionary<string, byte[]>> GetCreaturePictures() {
        Dictionary<string, byte[]> creaturePictures = new Dictionary<string, byte[]>();
        string[] creatureImageNames = Directory.GetFiles(GameManager.CREATURE_IMG_PATH);
        foreach (var creatureImageName in creatureImageNames) {
            string creatureName = Path.GetFileNameWithoutExtension(creatureImageName);

            if (GameManager.Instance.creatures.Any(e => e.creatureName == creatureName)) {
                byte[] picture = await File.ReadAllBytesAsync(GameManager.CREATURE_IMG_PATH + $"/{creatureName}.png");
                creaturePictures.Add(creatureName, picture);
            }
        }
        return creaturePictures;
    }

    public static async Task DownloadMap(string name, GameStateDto gameState) {
        await File.WriteAllBytesAsync(GameManager.MAP_PATH + $"/{name}.png", gameState.GetMapPicture());
    }
    
}
