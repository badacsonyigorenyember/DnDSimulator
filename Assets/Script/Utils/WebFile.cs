using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;

public class WebFile
{
    private string name;
    private string adventureName;
    
    public FileType fileType;

    public string AdventureName {
        get => adventureName.Contains("/") ? adventureName.Split("/")[1] : adventureName;
        set => adventureName = value;
    }

    [JsonProperty("name")] public string Name {
        get { 
            if(name.Contains('.')) {
                return name.Remove(name.IndexOf('.'));
            }
            else {
                return name;
            }
        }
        set {
            name = value;
        }
    }

    public string GetDownloadPath(DataHandler.DownloadType donwloadType) {
        if (donwloadType == DataHandler.DownloadType.Data) {
            return $"data/bestiary/bestiary-{adventureName.ToLower()}.json";
        }
        else {
            if (DataHandler.searchType == EntityType.Map) {
                return $"img/adventure/{adventureName}/{name.Replace(" ", "%20")}";

            }
            return $"img/{adventureName}/{name.Replace(" ", "%20")}";
        }
    }

    public string GetFullName() {
        return name;
    }

}

public enum FileType
{
    Folder,
    File
}

public enum EntityType
{
    Map,
    Monster
}

