using UnityEngine;
using UnityEngine.UI;

public class CreatureListElement : MonoBehaviour
{
    public void SetCreature(string n, byte[] img) {
        name = n;

        Texture2D texture = new Texture2D(1, 1);
        texture.LoadImage(img);

        GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);

        GetComponent<TooltipTrigger>().header = name;
    }
}
