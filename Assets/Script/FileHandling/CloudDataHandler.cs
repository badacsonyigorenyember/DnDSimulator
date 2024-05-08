using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Storage;
using Debug = UnityEngine.Debug;

public static class CloudDataHandler
{
    static FirebaseStorage storage = FirebaseStorage.DefaultInstance;
    static StorageReference storageReference = storage.GetReferenceFromUrl("gs://dnd-simulator-2024.appspot.com");

    public static async Task UploadImages() {
        List<Task> uploadTasks = new List<Task>();
        
        var entityImages = Directory.GetFiles(GameManager.ENTITY_IMG_PATH);

        foreach (var entityImage in entityImages) {
            string entityName = Path.GetFileNameWithoutExtension(entityImage);

            if (GameManager.Instance.entities.Any(e => e.entityName == entityName)) {
                uploadTasks.Add(UploadImageAsync(entityName));
            }
        }

        await Task.WhenAll(uploadTasks);
    }

    static async Task UploadImageAsync(string name) {
        StorageReference uploadReference = storageReference.Child($"Images/Entities/{name}.png");
        
        try {
            await uploadReference.GetDownloadUrlAsync();
        }
        catch {
            byte[] bytes = await File.ReadAllBytesAsync(GameManager.ENTITY_IMG_PATH + $"/{name}.png");
            
            var uploadTask = uploadReference.PutBytesAsync(bytes);
            await uploadTask;

            if (uploadTask.Exception != null) {
                Debug.LogError($"Failed to upload: {uploadTask.Exception}");
            }
        }
    }
    
    public static async Task DownloadImages(List<string> entityNames) {
        List<Task> downloadTasks = new List<Task>();

        Directory.CreateDirectory(GameManager.ENTITY_IMG_PATH);

        foreach (var entity in entityNames) {
            if (!File.Exists(GameManager.ENTITY_IMG_PATH + $"/{entity}.png")) {
                downloadTasks.Add(DownloadImageAsync(entity));
            }
        }
        
        await Task.WhenAll(downloadTasks);
    }

    static async Task DownloadImageAsync(string name) {
        StorageReference imageReference = storageReference.Child($"Images/Entities/{name}.png");

        var downloadTask = imageReference.GetBytesAsync(10_000_000);
        try {
            await downloadTask;
        }
        catch (Exception e) {
            Debug.LogError(e.Message);
            return;
        }
        
        await File.WriteAllBytesAsync(GameManager.ENTITY_IMG_PATH + $"/{name}.png", downloadTask.Result);
    }
    
    public static async Task UploadMap(string name) {
        StorageReference uploadReference = storageReference.Child($"Images/Maps/{name}.png");

        try {
            await uploadReference.GetDownloadUrlAsync();
        }
        catch {
            byte[] mapBytes = await File.ReadAllBytesAsync(GameManager.MAP_PATH + $"/{name}.png");

            var uploadTask = uploadReference.PutBytesAsync(mapBytes);
            await uploadTask;

            if (uploadTask.Exception != null) {
                Debug.LogError($"Failed to upload: {uploadTask.Exception}");
            }
        }
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

    public static async Task UploadSceneData(string name) {
        StorageReference uploadReference = storageReference.Child($"Data/Scenes/{name}.json");

        try {
            await uploadReference.GetDownloadUrlAsync();
        }
        catch {
            var uploadTask = uploadReference.PutFileAsync(GameManager.SCENE_PATH + $"/{name}.json");
            await uploadTask;

            if (uploadTask.Exception != null) {
                Debug.LogError($"Failed to upload: {uploadTask.Exception}");
            }
        }
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
}
