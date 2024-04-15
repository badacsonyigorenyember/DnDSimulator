using System;
using System.Collections.Generic;

[Serializable]
public class SceneObject
{
    public string name;
    public List<EntityDto> entities;

    public SceneObject(string name) {
        this.name = name;
        entities = new List<EntityDto>();
    }
}
