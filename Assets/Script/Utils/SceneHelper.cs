using System.Collections.Generic;
using System.Linq;
using Script.Utils;

public class SceneHelper
{
    public string sceneName;
    public List<EntityDto> entities;

    public SceneHelper(string name, List<Entity> entity) {
        sceneName = name;
        entities = entity.Select(FileHandler.EntityToEntityDto).ToList();
    }
}
