
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class Entity
{
    public string Name;
    public int Hp;
    [JsonIgnore] public GameObject Obj;

    public Entity(string name, int hp) {
        Name = name;
        Hp = hp;
    }

    public Sprite CreateSprite(byte[] bytes) {
        Texture2D texture = new Texture2D(0, 0);
        texture.LoadImage(bytes);
        
        return Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), Vector2.zero);
    }
}
        
