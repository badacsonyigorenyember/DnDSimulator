
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class Monster : Instantiatable
{
    public int hp;

    public Monster(string name, int hp, string adventure, string extension) {
        this.name = name;
        this.hp = hp;
        this.adventure = adventure;
        this.extension = extension;
    }

    public WebFile ConvertToWebFile() {
        return new WebFile();
    }
}
        
