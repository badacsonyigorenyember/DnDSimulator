using UnityEngine;

public class Map : Instantiatable
{
    public float scale;

    public Map(string name, string adventure, string extension, float scale) {
        this.name = name;
        this.adventure = adventure;
        this.extension = extension;
        this.scale = scale;
    }
}