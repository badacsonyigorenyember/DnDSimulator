using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

public class Get5etoolsInfos : MonoBehaviour
{
    private string uri = "https://api.github.com/repos/5etools-mirror-1/5etools-img/contents/img/MM";
    private List<Image> images = new List<Image>();

    private void Start() {
        StartCoroutine(GetRequest());
    }

    IEnumerator GetRequest() {
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
        
        Debug.Log("Finished downloading the data!");
        
        for(int i = 9; i < list.Length; i+=16) {
            images.Add(new Image(list[i]));
        }

        foreach (var image in images) {
            using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(image.Url)) {
                yield return request.SendWebRequest();
            
                switch (request.result) {
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.DataProcessingError:
                        Debug.LogError(string.Format("Something went wrong: " + request.error));
                        break;
            
                    case UnityWebRequest.Result.Success:
                        Texture2D texture = ((DownloadHandlerTexture) request.downloadHandler).texture;

                        byte[] bytes = texture.EncodeToPNG();
                        File.WriteAllBytes("Assets/Resources/Images/" + image.Name + ".png", bytes);
                        break;
                
                }
            
            
            }
            
            
        }
        Debug.Log("Finished downloading the images!");
        
    }
    
}
