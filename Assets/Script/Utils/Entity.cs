using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Entity
{
    public string Url;
    public string Name;

    public Entity(string raw) {
        Url = raw.Split(": ")[1].Replace("\"", "").Replace(",", "");
        Name = Url.Split("/").Last().Replace("%20", " ");
        Name = Name.Remove(Name.Length - 4);
    }

    public string Path() {
        return "Images/Bestiary/" + Name;
    }
}
        
