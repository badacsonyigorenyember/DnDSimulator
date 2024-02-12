using UnityEngine;

public static class ObjectGenerator
{
    public static void GenerateSprite(Texture2D texture, Vector2 position) {
        GameObject obj = new GameObject
        {
            transform =
            {
                position = position
            }
        };

        obj.AddComponent<SpriteRenderer>().sprite =
            Sprite.Create(texture, new Rect(new Vector2(0, 0), Vector2.one * texture.height), Vector2.zero);
    }
}
