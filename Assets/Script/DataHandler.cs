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

    private void Start() {
        //StartCoroutine(GetBeastiaryData());
    }

    public static IEnumerator GetImagesList() {
        Debug.LogError(GameManager.IMG_URL + currentPath);
        using (UnityWebRequest request = UnityWebRequest.Get(GameManager.IMG_URL + currentPath)) {
            yield return request.SendWebRequest();

            if (!HandleWebRequestResult(request, "Image list"))
                yield break;

            files = JsonConvert.DeserializeObject<List<WebFile>>(request.downloadHandler.text)!.
                Where(f => f.Name[0] >= 'A' && f.Name[0] <= 'Z').ToList();

            

            Debug.Log("Successful data access!");
        }

        OnDataDownloaded.Invoke();
    }
    
    

    /*private static IEnumerator GetBeastiaryData() {
        string downloadUrl;

        using (UnityWebRequest request = UnityWebRequest.Get(GameManager.DATA_URL)) {
            yield return request.SendWebRequest();

            if (!HandleWebRequestResult(request, "Data access"))
                yield break;

            downloadUrl = JsonUtility.FromJson<WebFile>(request.downloadHandler.text).download_url;
            
            Debug.Log("Successful data access!");
        }
        
        using (UnityWebRequest request = UnityWebRequest.Get(downloadUrl)) {
            yield return request.SendWebRequest();
            
            if (!HandleWebRequestResult(request, "Data download"))
                yield break;
            
            string json = request.downloadHandler.text;
            json = json.Substring(13, json.Length - 15);
            
            List<MonsterDataDownloaded> monsterDataDownloaded = JsonConvert.DeserializeObject<List<MonsterDataDownloaded>>(json);
            
            foreach (var m in monsterDataDownloaded!) {
                monsters.Add(m.Convert());
            }
            
            Directory.CreateDirectory(GameManager.DATA_SAVE_PATH);
            //File.WriteAllText(GameManager.DATA_SAVE_PATH + "data.json", json); ha offline kéne majd csinálni
            
            Debug.Log("Successful data download!");
        }
        
        using (UnityWebRequest request = UnityWebRequest.Get(GameManager.IMG_URL)) {
            yield return request.SendWebRequest();

            if (!HandleWebRequestResult(request, "Image URL access"))
                yield break;

            List<WebFile> webFiles = JsonConvert.DeserializeObject<List<WebFile>>(request.downloadHandler.text);

            foreach (var monster in monsters)
            {
                monster.DownloadUrl = webFiles!.Find(w => w.Name == monster.Name).download_url;
            }

            Debug.Log("Successful image URL access!");
        }
        
        OnDataDownloaded.Invoke();

    }*/

    private static bool HandleWebRequestResult(UnityWebRequest request, string operation) {
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Something went wrong during {operation}: {request.error}");
            return false;
        }
        return true;
    }

    
    public static IEnumerator DownloadImage(MonsterData data, Action action) {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(data.DownloadUrl)) {
            yield return request.SendWebRequest();
            
            if (!HandleWebRequestResult(request, "Image download"))
                yield break;
            
            Texture2D texture = ((DownloadHandlerTexture) request.downloadHandler).texture;

            byte[] bytes = texture.EncodeToPNG();

            if (!Directory.Exists(GameManager.IMG_SAVE_PATH)) Directory.CreateDirectory(GameManager.IMG_SAVE_PATH);

            if (!File.Exists(GameManager.IMG_SAVE_PATH + data.Name + ".png"))
                File.WriteAllBytes(GameManager.IMG_SAVE_PATH + data.Name + ".png", bytes);
            
            Debug.Log(File.Exists(GameManager.IMG_SAVE_PATH + data.Name + ".png"));
        }
        
        action.Invoke();
    }

    public static bool MonsterIsOnDisk(string name) {
        return File.Exists(GameManager.IMG_SAVE_PATH + name + ".png");
    }

    public static void GoBackOneFolder() {
        if (currentPath == "") return;

        currentPath = currentPath.Remove(currentPath.LastIndexOf('/'));
    }
    

    protected class MonsterDataDownloaded {
        [JsonProperty("name")] public string Name;
        [JsonProperty("hp")] public JObject hp;

        public int GetHp => hp["average"]!.Value<int>();

        public MonsterData Convert() {
            return new MonsterData(Name, GetHp);
        }
    }

}
