using System;
using System.Collections.Generic;

public class SceneObject
{
    public string name;
    public List<CreatureDto> creatures;

    public SceneObject(string name) {
        this.name = name;
        creatures = new List<CreatureDto>();
    }
}
