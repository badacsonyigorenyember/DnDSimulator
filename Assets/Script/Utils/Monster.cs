
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class Monster : DataHandler.Instantiatable
{
    public int hp;
    [JsonIgnore] public GameObject obj;

    public Monster(string name, int hp, string adventure) {
        this.name = name;
        this.hp = hp;
        this.adventure = adventure;
    }

    public Sprite CreateSprite(byte[] bytes) {
        Texture2D texture = new Texture2D(0, 0);
        texture.LoadImage(bytes);
        
        return Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), Vector2.zero);
    }

    public WebFile ConvertToWebFile() {
        return new WebFile();
    }
}
        
