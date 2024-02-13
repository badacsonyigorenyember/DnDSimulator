using System.Collections.Generic;
using UnityEngine;

public static class GameManager
{
    public const string IMG_URL = "https://api.github.com/repos/5etools-mirror-1/5etools-img/contents/img/MM";
    public const string DATA_URL = "https://api.github.com/repos/5etools-mirror-1/5etools-mirror-1.github.io/contents/data/bestiary/bestiary-mm.json";
    public static readonly string DATA_SAVE_PATH = Application.dataPath + "/Resources/Bestiary/Data/";
    public static readonly string IMG_SAVE_PATH = Application.dataPath + "/Resources/Bestiary/Images/";

    public static List<Entity> Entities;
}
