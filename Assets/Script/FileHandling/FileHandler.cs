using System;
using System.IO;
using Unity.Netcode;
using UnityEngine;

public static class FileHandler
{
    public static void LoadEntitySprite(Entity entity) {
        string path = GameManager.ENTITY_IMG_PATH;
        
        if (!File.Exists(path)) return;
        
        var bytes = File.ReadAllBytes(path);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(bytes);

        SpriteRenderer sr = entity.gameObject.GetComponent<SpriteRenderer>();

        sr.sprite = Sprite.Create(
            texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero, 200);

        entity.gameObject.GetComponent<CircleCollider2D>().radius = sr.size.x;
    }
}
