using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class Get5etoolsInfos : MonoBehaviour
{
    private static string imgUrl = "https://api.github.com/repos/5etools-mirror-1/5etools-img/contents/img/MM";
    private static string dataUrl = "https://api.github.com/repos/5etools-mirror-1/5etools-mirror-1.github.io/contents/data/bestiary/bestiary-mm.json";
    
    public static List<MonsterObj> monsters = new List<MonsterObj>();

    public static Action OnDataDownloaded;

    private void Start() {
        StartCoroutine(GetBeastiaryData());
    }

    public static IEnumerator GetBeastiaryData()
    {
        string downloadUrl;

        using (UnityWebRequest request = UnityWebRequest.Get(dataUrl))
        {
            yield return request.SendWebRequest();

            if (!HandleWebRequestResult(request, "Data access"))
                yield break;

            downloadUrl = JsonUtility.FromJson<WebFile>(request.downloadHandler.text).download_url;
            Debug.Log("Successful data access!");
        }

        using (UnityWebRequest request = UnityWebRequest.Get(downloadUrl))
        {
            yield return request.SendWebRequest();

            if (!HandleWebRequestResult(request, "Data download"))
                yield break;

            string json = request.downloadHandler.text;
            json = json.Substring(13, json.Length - 15);

            monsters = JsonConvert.DeserializeObject<List<MonsterObj>>(json);
            File.WriteAllText("Assets/Resources/Bestiary/data.json", json);

            Debug.Log("Successful data download");
        }

        using (UnityWebRequest request = UnityWebRequest.Get(imgUrl))
        {
            yield return request.SendWebRequest();

            if (!HandleWebRequestResult(request, "Image URL access"))
                yield break;

            List<WebFile> webFiles = JsonConvert.DeserializeObject<List<WebFile>>(request.downloadHandler.text);

            foreach (var monster in monsters)
            {
                monster.download_url = webFiles.Find(w => w.Name == monster.Name)?.download_url;
            }

            Debug.Log("Successful image URL access!");
        }
    }

// Helper method to handle UnityWebRequest results
    private static bool HandleWebRequestResult(UnityWebRequest request, string operation)
    {
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Something went wrong during {operation}: {request.error}");
            return false;
        }
        return true;
    }

    
    public static IEnumerator DownloadImage(MonsterObj data, Action action) {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(data.download_url)) {
            yield return request.SendWebRequest();
            
            switch (request.result) {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(string.Format("Something went wrong: " + request.error));
                    break;
            
                case UnityWebRequest.Result.Success:
                    Texture2D texture = ((DownloadHandlerTexture) request.downloadHandler).texture;

                    byte[] bytes = texture.EncodeToPNG();
                    File.WriteAllBytes("Assets/Resources/Images/Bestiary/" + data.Name + ".png", bytes);
                    break;
            }
        }
        
        action.Invoke();
    }

    


    
    
}
