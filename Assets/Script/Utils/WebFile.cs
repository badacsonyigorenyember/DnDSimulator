using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;

public class WebFile
{
    public string adventureName;
    public string name;
    public string extension;
    public FileType fileType;
    
    public void Init() {
        adventureName = DataHandler.searchPath.Split("/").Last();
        
        if (name.Contains('.')) {
            fileType = FileType.File;
            var temp = name.Split('.');
            extension = temp.Last();
            name = temp.First();
        }
        else {
            fileType = FileType.Folder;
            extension = "";
        }
    }

    public string GetDownloadPath(DataHandler.DownloadType donwloadType) {
        if (donwloadType == DataHandler.DownloadType.Data) {
            return $"data/bestiary/bestiary-{adventureName.ToLower()}.json";
        }

        string subString = $"{adventureName}/{name.Replace(" ", "%20")}.{extension}";
            
        if (DataHandler.searchType == EntityType.Map) {
            return $"img/adventure/{subString}";
        }
        
        return $"img/{subString}";
    }

    public string GetNameWithExtension() {
        return name + "." + extension;
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

