using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Storage;
using UnityEngine.Networking;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public static class CloudDataHDownloader
{
    static FirebaseStorage storage = FirebaseStorage.DefaultInstance;
    static StorageReference storageReference = storage.GetReferenceFromUrl("gs://dnd-simulator-2024.appspot.com");

    public static async Task DownloadImages(GameManager.EntityName[] entityNames) {
        List<Task> downloadTasks = new List<Task>();

        Directory.CreateDirectory(GameManager.ENTITY_IMG_PATH);
        
        var files =
            Directory.GetFiles(GameManager.ENTITY_IMG_PATH)
            .Select(Path.GetFileNameWithoutExtension)
            .ToList();

        foreach (var entity in entityNames) {
            string name = entity.name;
            
            if (!files.Contains(name)) {
                downloadTasks.Add(DownloadImageAsync(name));
            }
        }
        
        await Task.WhenAll(downloadTasks);
    }

    static async Task DownloadImageAsync(string name) {
        StorageReference imageReference = storageReference.Child($"Images/{name}.png");

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
        try {
            await storageReference.Child($"Images/{name}.png").GetDownloadUrlAsync();
        }
        catch {
            byte[] bytes = await File.ReadAllBytesAsync(GameManager.ENTITY_IMG_PATH + $"/{name}.png");
            
            StorageReference uploadReference = storageReference.Child($"Images/{name}.png");

            var uploadTask = uploadReference.PutBytesAsync(bytes);
            await uploadTask;

            if (uploadTask.Exception != null) {
                Debug.LogError($"Failed to upload: {uploadTask.Exception}");
            }
        }
    }
}
