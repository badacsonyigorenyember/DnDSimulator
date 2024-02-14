using System.Collections.Generic;
using UnityEngine;

public static class GameManager
{
    public const string IMG_URL = "https://api.github.com/repos/5etools-mirror-1/5etools-mirror-1.github.io/contents/img";
    public const string DOWNLOAD_URL = "https://raw.githubusercontent.com/5etools-mirror-1/5etools-mirror-1.github.io/master/";
    public static readonly string DATA_SAVE_PATH = Application.dataPath + "/resources/bestiary/";

    public static List<Monster> Entities = new List<Monster>();
    private static List<Monster> DownloadedEntities = new List<Monster>();

}
