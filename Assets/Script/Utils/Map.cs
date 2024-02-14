using UnityEngine;

public class Map : DataHandler.Instantiatable
{
    public GameObject obj;

    public Map(string name, string adventure) {
        this.name = name;
        this.adventure = adventure;
    }
}