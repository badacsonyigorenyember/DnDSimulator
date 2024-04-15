using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class WaitingScreenHandler : NetworkBehaviour
{
    public Image image;
    public TextMeshProUGUI waitingText;
    public float seconds;
    
    private List<Texture2D> _textures = new();
    private int currentIndex = -1;

    private void Start() {
        if (IsServer) {
            gameObject.SetActive(false);
            return;
        }
        
        LoadImagesFromDisk();
        StartCoroutine(DisplayTextures());
    }

    IEnumerator DisplayTextures() {
        while (true) {
            if (currentIndex != -1) {
                yield return FadeOutImage(image, 1);
            }
            else {
                currentIndex = 0;
            }
            
            Texture2D currentTexture = _textures[currentIndex];
            image.sprite = Sprite.Create(currentTexture, new Rect(0, 0, currentTexture.width, currentTexture.height), Vector2.zero);

            currentIndex = (currentIndex + 1) % _textures.Count;

            yield return FadeInImage(image, 1);

            yield return new WaitForSeconds(seconds);
        }
    }
    
    IEnumerator FadeOutImage(Image image, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            
            Color color = Color.Lerp(Color.white, Color.black, elapsedTime / duration);
            
            image.color = color;
            waitingText.color = color;
            yield return null;
        }

        image.color = Color.black;
    }

    IEnumerator FadeInImage(Image image, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            
            Color color = Color.Lerp(Color.black, Color.white, elapsedTime / duration);
            
            image.color = color;
            waitingText.color = color;
            yield return null;
        }

        image.color = Color.white;
    }

    void LoadImagesFromDisk() {
        Texture2D tex = new Texture2D(1, 1);
        var files = Directory.GetFiles(Application.dataPath + "/Resources/Images/LoadingImages/", "*.jpeg");

        foreach (var file in files) {
            byte[] bytes = File.ReadAllBytes(file);

            Texture2D texture = new Texture2D(1, 1);
            texture.LoadImage(bytes);
            
            _textures.Add(texture);
        }

    }
    
}
