using Newtonsoft.Json;
using UnityEngine;

public abstract class Instantiatable
{
    public string name;
    public string adventure;
    public string extension;
    [JsonIgnore] public GameObject obj;

        
    public Sprite CreateSprite(byte[] bytes) {
        Texture2D texture = new Texture2D(0, 0);
        texture.LoadImage(bytes);
        
        return Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), Vector2.zero);
    }
}