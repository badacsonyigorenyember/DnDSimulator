using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

public class DataHandler : MonoBehaviour
{
    public static List<WebFile> files = new List<WebFile>();
    
    public static Action OnDataDownloaded;

    public static string currentPath = "";

    public static IEnumerator GetImagesList() {
        if (!File.Exists(GameManager.JSON_SAVE_PATH + (currentPath == "" ? "data" : currentPath) + ".json")) {
            using (UnityWebRequest request = UnityWebRequest.Get(GameManager.IMG_URL + currentPath)) {
                yield return request.SendWebRequest();

                if (!HandleWebRequestResult(request, "File list"))
                    yield break;
                
                files = JsonConvert.DeserializeObject<List<WebFile>>(request.downloadHandler.text)!.
                    Where(f => f.Name[0] >= 'A' && f.Name[0] <= 'Z').ToList();
            
                Debug.Log("Successful file list access!");

                Directory.CreateDirectory(GameManager.JSON_SAVE_PATH);
                File.WriteAllText(GameManager.JSON_SAVE_PATH + (currentPath == "" ? "data" : currentPath) + ".json",
                    JsonConvert.SerializeObject(files));
            }
        }
        else {
            string json = 
                File.ReadAllText(GameManager.JSON_SAVE_PATH + (currentPath == "" ? "data" : currentPath) + ".json");

            yield return null;
            
            files = JsonConvert.DeserializeObject<List<WebFile>>(json);
            
            Debug.Log("Successful file list read!");
        }
        
        OnDataDownloaded.Invoke();
    }

    private static bool HandleWebRequestResult(UnityWebRequest request, string operation) {
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Something went wrong during {operation}: {request.error}");
            return false;
        }
        return true;
    }

    
    public static IEnumerator DownloadImage(WebFile data, Action<ScrollButtonState> action) {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(data.download_url)) {
            yield return request.SendWebRequest();
        
            if (!HandleWebRequestResult(request, "Image download"))
                yield break;
        
            Texture2D texture = ((DownloadHandlerTexture) request.downloadHandler).texture;

            byte[] bytes = texture.EncodeToPNG();

            if (!Directory.Exists(GameManager.IMG_SAVE_PATH)) Directory.CreateDirectory(GameManager.IMG_SAVE_PATH);
            
            File.WriteAllBytes(GameManager.IMG_SAVE_PATH + data.Name + ".png", bytes);
        }
        
        string downloadUrl;

        using (UnityWebRequest request = UnityWebRequest.Get(GameManager.DATA_URL + data.Source.ToLower() + ".json")) {
            yield return request.SendWebRequest();
            
            if (!HandleWebRequestResult(request, "Image data download"))
                yield break;
        
            downloadUrl = JsonConvert.DeserializeObject<WebFile>(request.downloadHandler.text)!.download_url;
            
            Debug.Log("Successful image download URL access!");
        }
        

        if (!File.Exists(GameManager.DATA_SAVE_PATH + data.Source + ".json")) {
            using (UnityWebRequest request = UnityWebRequest.Get(downloadUrl)) {
                yield return request.SendWebRequest();
            
                if (!HandleWebRequestResult(request, "Data download"))
                    yield break;
            
                string json = request.downloadHandler.text;

                Entity entity = JsonConvert.DeserializeObject<Root>(json)!.list.Find(m => m.Name == data.Name).Convert();
                
                Directory.CreateDirectory(GameManager.DATA_SAVE_PATH);
                File.WriteAllText(GameManager.DATA_SAVE_PATH + data.Name + ".json", JsonConvert.SerializeObject(entity));
            
                Debug.Log("Successful data download!");
            }
        }
        
        action.Invoke(ScrollButtonState.Instantiate);
    }

    public static bool MonsterIsOnDisk(string name) {
        return File.Exists(GameManager.IMG_SAVE_PATH + name + ".png");
    }

    public static List<Entity> GetDownloadedEntities() {
        var info = new DirectoryInfo(GameManager.DATA_SAVE_PATH);
        var fileInfo = info.GetFiles();

        List<Entity> entities = new List<Entity>();

        foreach (var f in fileInfo) {
            if (f.Name.Contains(".json") && !f.Name.Contains(".meta")) {
                entities.Add(JsonConvert.DeserializeObject<Entity>(File.ReadAllText(GameManager.DATA_SAVE_PATH + f.Name)));
            }
        }

        return entities;
    }

    public static void GoBackOneFolder() {
        if (currentPath == "") return;

        currentPath = currentPath.Remove(currentPath.LastIndexOf('/'));
    }

    public static Entity CreateEntity(string name) {
        byte[] bytes = File.ReadAllBytes(GameManager.IMG_SAVE_PATH + name + ".png");
        string json = File.ReadAllText(GameManager.DATA_SAVE_PATH + name + ".json");

        Entity e = JsonConvert.DeserializeObject<Entity>(json);
        
        GameObject a = new GameObject
        {
            name = name
        };

        a.AddComponent<SpriteRenderer>().sprite = e.CreateSprite(bytes);
        a.AddComponent<CircleCollider2D>();
        a.layer = LayerMask.NameToLayer("Entity");
        
        e.Obj = a;
        return e;
    }
    

    protected class MonsterDataDownloaded {
        [JsonProperty("name")] public string Name;
        [JsonProperty("hp")] public JObject hp;

        private int GetHp {
            get {
                if (hp == null) return 1;
                
                return hp["average"]!.Value<int>();
            }
        }

        public Entity Convert() {
            return new Entity(Name, GetHp);
        }
    }

    protected class Root
    {
        [JsonProperty("monster")] public List<MonsterDataDownloaded> list;
    }

}