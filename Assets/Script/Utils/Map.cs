using UnityEngine;

public class Map : Instantiatable
{

    public Map(string name, string adventure, string extension) {
        this.name = name;
        this.adventure = adventure;
        this.extension = extension;
    }
}