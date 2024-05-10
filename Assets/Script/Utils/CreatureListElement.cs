using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CreatureListElement : MonoBehaviour
{
    [SerializeField] private Image _image;

    public void SetCreature(string n, byte[] img) {
        name = n ;

        Texture2D texture = new Texture2D(1, 1);
        texture.LoadImage(img);
        
        _image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
    }

    
    
}
