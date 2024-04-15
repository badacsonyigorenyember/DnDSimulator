using System;
using System.IO;
using Unity.Netcode;
using UnityEngine;

public class FileHandler : NetworkBehaviour
{
    public static FileHandler Instance;
    public string path;

    private void Awake() {
        Instance = this;
    }

    

    public void LoadEntitySprite(Entity entity) {
        var bytes = File.ReadAllBytes(Application.dataPath + $"/Resources/Entities/{entity.entityName}.png");
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(bytes);

        SpriteRenderer renderer = entity.gameObject.GetComponent<SpriteRenderer>();

        renderer.sprite = Sprite.Create(
            texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero, 200);

        entity.gameObject.GetComponent<CircleCollider2D>().radius = renderer.size.x;
    }
}
