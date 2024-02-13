using System.Collections.Generic;
using UnityEngine;

public static class GameManager
{
    public const string IMG_URL = "https://api.github.com/repos/5etools-mirror-1/5etools-img/contents/img";
    public const string MAP_URL = "https://api.github.com/repos/5etools-mirror-1/5etools-img/contents/img/adventure/";
    public const string DATA_URL = "https://api.github.com/repos/5etools-mirror-1/5etools-mirror-1.github.io/contents/data/bestiary/bestiary-";
    public static readonly string DATA_SAVE_PATH = Application.dataPath + "/Resources/Bestiary/Data/";
    public static readonly string IMG_SAVE_PATH = Application.dataPath + "/Resources/Bestiary/Images/";
    public static readonly string JSON_SAVE_PATH = Application.dataPath + "/Resources/Bestiary/Data/Data/";

    public static List<Entity> Entities = new List<Entity>();
}
