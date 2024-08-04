using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FileHandling.Dto;

namespace FileHandling
{
    public static class CloudDataHandler
    {
        public static async Task SaveCreatureImages(List<string> creatureNames, GameStateDto gameState) {
            List<Task> downloadTasks = new List<Task>();

            string creatureImgPath = FileManager.Instance.creatureImgPath;
            
            Directory.CreateDirectory(creatureImgPath);

            foreach (var creature in creatureNames) {
                if (!File.Exists(creatureImgPath + $"/{creature}.png")) {
                    byte[] creaturePicture = gameState.creaturePictures[creature];
                    downloadTasks.Add(File.WriteAllBytesAsync(creatureImgPath + $"/{creature}.png", creaturePicture));
                }
            }

            await Task.WhenAll(downloadTasks);
        }

        public static async Task<GameStateDto> GetGameStateDto(string sceneName) {
            FileManager fileManager = FileManager.Instance;
            var mapPictureTask = File.ReadAllBytesAsync(fileManager.sceneImgPath + $"/{sceneName}.png");
            var sceneDataTask = File.ReadAllTextAsync(fileManager.sceneFolderPath + $"/{sceneName}.json");
            var creaturePicturesTask = GetCreaturePictures();

            await Task.WhenAll(mapPictureTask, sceneDataTask, creaturePicturesTask);
        
            byte[] mapPicture = mapPictureTask.Result;
            string sceneData = sceneDataTask.Result;
            Dictionary<string, byte[]> creaturePictures = creaturePicturesTask.Result;
            return new GameStateDto(mapPicture, creaturePictures, sceneData);
        }

        private static async Task<Dictionary<string, byte[]>> GetCreaturePictures() {
            string creatureImgPath = FileManager.Instance.creatureImgPath;
            Dictionary<string, byte[]> creaturePictures = new Dictionary<string, byte[]>();
            string[] creatureImageNames = Directory.GetFiles(creatureImgPath);
            foreach (var creatureImageName in creatureImageNames) {
                string creatureName = Path.GetFileNameWithoutExtension(creatureImageName);

                if (GameManager.Instance.entities.Any(e => e.Value.name == creatureName)) {
                    byte[] picture = await File.ReadAllBytesAsync(creatureImgPath + $"/{creatureName}.png");
                    creaturePictures.Add(creatureName, picture);
                }
            }
            return creaturePictures;
        }

        public static async Task DownloadMap(string name, GameStateDto gameState) {
            await File.WriteAllBytesAsync(FileManager.Instance.sceneImgPath + $"/{name}.png", gameState.mapPicture);
        }
    }
}
