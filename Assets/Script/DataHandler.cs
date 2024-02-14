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
        Debug.Log(GameManager.IMG_URL + searchPath);
        Debug.Log(searchType);


        if (!File.Exists(GameManager.DATA_SAVE_PATH + "/list/" + currentFolder + ".json")) {
            using (UnityWebRequest request = UnityWebRequest.Get(GameManager.IMG_URL + searchPath)) {
                yield return request.SendWebRequest();
            
                
                if (!HandleWebRequestResult(request, "File list"))
                    yield break;

                files = JsonConvert.DeserializeObject<List<WebFile>>(request.downloadHandler.text)!
                    .Where(f =>
                        f.Name.Length > 0 &&
                        (f.Name[0] >= 'A' && f.Name[0] <= 'Z' || f.Name.Contains("jpeg")))
                    .Select(f => {
                        f.AdventureName = searchPath == "" ? "" : searchPath.Remove(0, 1);
                        if (searchPath != "") {
                            f.AdventureName =  searchPath.Split("/").Last();
                        }
                        else {
                            f.AdventureName = "";
                        }
                        f.fileType = f.GetFullName().Contains('.') ? FileType.File : FileType.Folder;
                        return f;
                    }).ToList();
            
                Debug.Log("Successful FileList access!");
                
                Directory.CreateDirectory(GameManager.DATA_SAVE_PATH + "/list" + (searchType == EntityType.Map ? "/adventure" : ""));
                File.WriteAllText(GameManager.DATA_SAVE_PATH + "/list/" + currentFolder + ".json", JsonConvert.SerializeObject(files));
            }
        }
        else {
            string json = File.ReadAllText(GameManager.DATA_SAVE_PATH + "/list/" + currentFolder + ".json");

            files = JsonConvert.DeserializeObject<List<WebFile>>(json);

            yield return null;
            
            Debug.Log("Successful FileList read!");
        }
        
        
        OnDataAccess.Invoke();
    }

    public static IEnumerator DownloadData(WebFile file, Action<ScrollButtonState> action) {
        if (searchType != EntityType.Map) {
            using (UnityWebRequest request = UnityWebRequest.Get(GameManager.DOWNLOAD_URL + file.GetDownloadPath(DownloadType.Data))) {
                yield return request.SendWebRequest();
            
                if (!HandleWebRequestResult(request, "Data download"))
                    yield break;

                BestiaryDownloadData data = JsonConvert.DeserializeObject<Root>(request.downloadHandler.text)!.entities
                    .Find(e => e.name == file.Name);

                Debug.Log("Successful data download");

                Directory.CreateDirectory(GameManager.DATA_SAVE_PATH + "/data/bestiary");
                File.WriteAllText(GameManager.DATA_SAVE_PATH + "/data/bestiary/" + file.Name + ".json", JsonConvert.SerializeObject(data.CreateEntity(file.AdventureName)));
            }
        }
        else {
            Map map = new Map(file.Name, file.AdventureName);
                        
            Directory.CreateDirectory(GameManager.DATA_SAVE_PATH + "/data/map");
            File.WriteAllText(GameManager.DATA_SAVE_PATH + "/data/map/" + file.Name, JsonConvert.SerializeObject(map));
        }
        
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(GameManager.DOWNLOAD_URL + file.GetDownloadPath(DownloadType.Image))) {
            yield return request.SendWebRequest();
            
            if (!HandleWebRequestResult(request, "Image download"))
                yield break;

            Texture2D img = ((DownloadHandlerTexture) request.downloadHandler).texture;

            Debug.Log("Successful image download");

            if (searchType != EntityType.Map) {
                Directory.CreateDirectory(GameManager.DATA_SAVE_PATH + "/img/bestiary");
                File.WriteAllBytes(GameManager.DATA_SAVE_PATH + "/img/bestiary/" + file.GetFullName(), img.EncodeToPNG());
            }
            else {
                Directory.CreateDirectory(GameManager.DATA_SAVE_PATH + "/img/map");
                File.WriteAllBytes(GameManager.DATA_SAVE_PATH + "/img/map/" + file.GetFullName(), img.EncodeToPNG());
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
        List<Instantiatable> files = new List<Instantiatable>();

        foreach (var file in dir.GetFiles()) {
            if (file.Name.Contains("json") && !file.Name.Contains("meta")) {
                if (searchType == EntityType.Map) {
                    files.Add(JsonConvert.DeserializeObject<Map>(File.ReadAllText(path + file.Name)));
                    continue;
                }

                if (searchType == EntityType.Monster) {
                    files.Add(JsonConvert.DeserializeObject<Monster>(File.ReadAllText(path + file.Name)));
                    continue;
                }
                
            }
        }
        
        
        return files;
    }

    public static Instantiatable CreateEntity(Instantiatable entity) {
        string path = GameManager.DATA_SAVE_PATH + "img/";
        switch (searchType) {
            case EntityType.Map:
                path += "map/";
                break;
            case EntityType.Monster:
                path += "bestiary/";
                break;
        }

        GameObject obj = new GameObject(entity.name);

        return null;
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

        public Monster CreateEntity(string adventure) {
            Monster e = new Monster(name, hp, adventure);

            return e;
        }
    }

    protected class Root
    {
        [JsonProperty("monster")]public List<BestiaryDownloadData> entities;
    }



    public abstract class Instantiatable
    {
        public string name;
        public string adventure;
    }
    


    private static bool HandleWebRequestResult(UnityWebRequest request, string operation) {
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Something went wrong during {operation}: {request.error}");
            return false;
        }
        return true;
    }


    /*public static List<WebFile> files = new List<WebFile>();
    
    public static Action OnDataDownloaded;

    public static string currentPath = "";
    public static EntityType searchType = EntityType.Monster;

    public static IEnumerator GetImagesList() {
        if (!File.Exists(GameManager.JSON_SAVE_PATH + (currentPath == "" ? "data" : currentPath) + ".json")) {
            using (UnityWebRequest request = UnityWebRequest.Get(GameManager.IMG_URL + currentPath)) {
                yield return request.SendWebRequest();
                
                Debug.Log(GameManager.IMG_URL + currentPath);
                
                if (!HandleWebRequestResult(request, "File list"))
                    yield break;

                files = JsonConvert.DeserializeObject<List<WebFile>>(request.downloadHandler.text)!
                    .Where(f => (f.Name.Length > 0 && f.Name[0] >= 'A' && f.Name[0] <= 'Z') || f.Name.Contains(".jpg")).ToList();;

                foreach (var file in files) {
                    file.entityType = searchType;
                    Debug.Log(searchType + " " + file.entityType);
                }
                
                Debug.Log("Successful file list access!");

                Directory.CreateDirectory(GameManager.JSON_SAVE_PATH + currentPath);
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
            
            File.WriteAllBytes(GameManager.IMG_SAVE_PATH + data.Name + "." + data.extension, bytes);
        }
        
Debug.Log(GameManager.DOWNLOAD_URL + data.GetDownloadPath(WebFile.DownloadType.Data));
        
        if (!File.Exists(GameManager.DATA_SAVE_PATH + data.Path)) {
            using (UnityWebRequest request = UnityWebRequest.Get(GameManager.DOWNLOAD_URL + data.GetDownloadPath(WebFile.DownloadType.Data))) {
                yield return request.SendWebRequest();
            
                if (!HandleWebRequestResult(request, "Data download"))
                    yield break;
            
                string json = request.downloadHandler.text;

                Entity entity = JsonConvert.DeserializeObject<Root>(json)!.list.Find(m => m.Name == data.Name).Convert();
                
                Directory.CreateDirectory(GameManager.DATA_SAVE_PATH + data.GetDir());
                //File.WriteAllText(GameManager.DATA_SAVE_PATH + data.GetDownloadPath(Dat), JsonConvert.SerializeObject(entity));
            
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
*/
}