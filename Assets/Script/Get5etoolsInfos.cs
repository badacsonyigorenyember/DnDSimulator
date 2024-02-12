using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class Get5etoolsInfos : MonoBehaviour
{
    private static string uri = "https://api.github.com/repos/5etools-mirror-1/5etools-img/contents/img/MM";
    public static List<Entity> entites = new List<Entity>();

    public static Action OnDataDownloaded;

    private void Start() {
        StartCoroutine(GetBeastiaryData());
    }

    public static  IEnumerator GetBeastiaryData() {
        string[] list = Array.Empty<string>();

        using (UnityWebRequest request = UnityWebRequest.Get(uri)) {
            yield return request.SendWebRequest();

            switch (request.result) {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(string.Format("Something went wrong: " + request.error));
                    break;

                case UnityWebRequest.Result.Success:
                    list = request.downloadHandler.text.Split("\n");
                    break;

            }
        }
        
        for(int i = 9; i < list.Length; i+=16) {
            entites.Add(new Entity(list[i]));
        }
        
        Debug.Log("Finished downloading the data!");

        OnDataDownloaded.Invoke();
    }

    public static IEnumerator DownloadImage(Entity data, Action action) {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(data.Url)) {
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
