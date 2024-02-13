
using System.IO;
using UnityEngine;

public class Entity
{
    public string Name;
    public int Hp;
    public GameObject Obj;

    public Entity(MonsterData data) {
        Name = data.Name;
        Hp = data.Hp;
    }

    public Sprite CreateSprite() {
        byte[] textureData = File.ReadAllBytes(GameManager.IMG_SAVE_PATH + Name + ".png");
        
        Texture2D texture = new Texture2D(0, 0);
        texture.LoadImage(textureData);
        
        return Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), Vector2.zero);
    }
}
        
