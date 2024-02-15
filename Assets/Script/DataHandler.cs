using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

public class DataHandler : MonoBehaviour
{
    public static List<WebFile> files = new List<WebFile>();
    public static Action OnDataAccess;

    public static string searchPath = "";
    public static EntityType searchType = EntityType.Monster;

    public enum DownloadType
    {
        Image,
        Data
    }
    
    public static IEnumerator GetFileList() {
        string currentFolder = searchPath == "" ? "main" : searchPath;
        string filePath = GameManager.DATA_SAVE_PATH + "/list/" + currentFolder + ".json";

        if (!File.Exists(filePath)) {
            using (UnityWebRequest request = UnityWebRequest.Get(GameManager.IMG_URL + searchPath)) {
                yield return request.SendWebRequest();

                if (!HandleWebRequestResult(request, "File list"))
                    yield break;

                files = JsonConvert.DeserializeObject<List<WebFile>>(request.downloadHandler.text)!
                    .Where(f =>
                        f.name.Length > 0 &&
                        (f.name[0] >= 'A' && f.name[0] <= 'Z' || f.name.Contains("jpeg")))
                    .Select(f => {
                        f.Init();
                        return f;
                    }).ToList();

                Debug.Log("Successful FileList access!");
                
                Directory.CreateDirectory(GameManager.DATA_SAVE_PATH + "/list" + (searchType == EntityType.Map ? "/adventure" : ""));
                File.WriteAllText(filePath, JsonConvert.SerializeObject(files));
            }
        }
        else {
            string json = File.ReadAllText(filePath);
            files = JsonConvert.DeserializeObject<List<WebFile>>(json);

            yield return null;
            
            Debug.Log("Successful FileList read!");
        }
        
        OnDataAccess.Invoke();
    }

    public static IEnumerator DownloadData(WebFile file, Action<ScrollButtonState> action) {
        string filePath = GameManager.DATA_SAVE_PATH + "/data/";
        
        if (searchType != EntityType.Map) {
            using (UnityWebRequest request = UnityWebRequest.Get(GameManager.DOWNLOAD_URL + file.GetDownloadPath(DownloadType.Data))) {
                yield return request.SendWebRequest();
            
                if (!HandleWebRequestResult(request, "Data download"))
                    yield break;

                BestiaryDownloadData data = JsonConvert.DeserializeObject<Root>(request.downloadHandler.text)!.entities
                    .Find(e => e.name == file.name);

                Debug.Log("Successful data download");

                Directory.CreateDirectory(GameManager.DATA_SAVE_PATH + "/data/bestiary");
                File.WriteAllText(filePath + "bestiary/" + file.name + ".json", JsonConvert.SerializeObject(data.CreateEntity(file)));
            }
        }
        else {
            Map map = new Map(file.name, file.adventureName, file.extension, 1f);
                        
            Directory.CreateDirectory(GameManager.DATA_SAVE_PATH + "/data/map");
            File.WriteAllText( filePath + "map/" + file.name + ".json", JsonConvert.SerializeObject(map));
        }

        filePath = GameManager.DATA_SAVE_PATH + "/img/";
        
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(GameManager.DOWNLOAD_URL + file.GetDownloadPath(DownloadType.Image))) {
            yield return request.SendWebRequest();
            
            if (!HandleWebRequestResult(request, "Image download"))
                yield break;

            Texture2D img = ((DownloadHandlerTexture) request.downloadHandler).texture;

            Debug.Log("Successful image download");

            if (searchType != EntityType.Map) {
                Directory.CreateDirectory(GameManager.DATA_SAVE_PATH + "/img/bestiary");
                File.WriteAllBytes(filePath + "bestiary/" + file.GetNameWithExtension(), img.EncodeToPNG());
            }
            else {
                Directory.CreateDirectory(GameManager.DATA_SAVE_PATH + "/img/map");
                File.WriteAllBytes(filePath + "map/" + file.GetNameWithExtension(), img.EncodeToPNG());
            }
        }
        
        action.Invoke(ScrollButtonState.Instantiate);
    }

    public static void GoBackOneFolder() {
        if (searchPath == "" || (searchType == EntityType.Map && searchPath == "/adventure")) return;

        searchPath = searchPath.Remove(searchPath.LastIndexOf('/'));
    }

    public static List<Instantiatable> ListDownloadedFiles() {
        string path = GameManager.DATA_SAVE_PATH + "data/";
        switch (searchType) {
            case EntityType.Map:
                path += "map/";
                break;
            case EntityType.Monster:
                path += "bestiary/";
                break;
        }

        DirectoryInfo dir = new DirectoryInfo(path);
        List<Instantiatable> fileList = new List<Instantiatable>();
        
        foreach (var file in dir.GetFiles()) {
            if (file.Name.Contains("json") && !file.Name.Contains("meta")) {
                if (searchType == EntityType.Map) {
                    fileList.Add(JsonConvert.DeserializeObject<Map>(File.ReadAllText(path + file.Name)));
                    continue;
                }

                if (searchType == EntityType.Monster) {
                    fileList.Add(JsonConvert.DeserializeObject<Monster>(File.ReadAllText(path + file.Name)));
                    continue;
                }
                
            }
        }

        return fileList;
    }

    public static bool EntityIsOnDisk(string name) {
        string path = GameManager.DATA_SAVE_PATH + "data/";
        switch (searchType) {
            case EntityType.Map:
                path += "map/";
                break;
            case EntityType.Monster:
                path += "bestiary/";
                break;
        }
        
        return File.Exists(path + name + ".json");
    }

    public static Instantiatable CreateEntity(string name) {
        string imgPath = GameManager.DATA_SAVE_PATH + "img/";
        string dataPath = GameManager.DATA_SAVE_PATH + "data/";

        Instantiatable entity = null;
        
        switch (searchType) {
            case EntityType.Map:
                imgPath += "map/";
                dataPath += "map/" + name + ".json";
                entity = JsonConvert.DeserializeObject<Map>(File.ReadAllText(dataPath));
                break;
            case EntityType.Monster:
                imgPath += "bestiary/";
                dataPath += "bestiary/" + name + ".json";
                entity = JsonConvert.DeserializeObject<Monster>(File.ReadAllText(dataPath));
                break;
        }

        GameObject obj = new GameObject(name);

        byte[] bytes = File.ReadAllBytes(imgPath + name + "." + entity!.extension);
        obj.AddComponent<SpriteRenderer>().sprite = entity.CreateSprite(bytes);
        obj.AddComponent<CircleCollider2D>();

        return entity;
    }

    protected class BestiaryDownloadData
    {
        [JsonProperty("name")] public string name;
        [JsonIgnore] public int hp;

        [JsonProperty("hp")]
        public JObject hpObj {
            set {
                if (value != null && value["average"] != null)
                    hp = value["average"].Value<int>();
                else
                    hp = 1;
            }
        }

        public Monster CreateEntity(WebFile file) {
            return new Monster(name, hp, file.adventureName, file.extension);
        }
    }

    protected class Root
    {
        [JsonProperty("monster")]public List<BestiaryDownloadData> entities;
    }



    private static bool HandleWebRequestResult(UnityWebRequest request, string operation) {
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Something went wrong during {operation}: {request.error}");
            return false;
        }
        return true;
    }
}