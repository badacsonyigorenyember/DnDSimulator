using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class WebFile
{
    private string name;
    private string source;
    
    [JsonProperty("name")]public string Name {
        get => name.Contains(".png") ? name[..^4] : name;
        set => name = value;
    }

    [JsonProperty("path")]public string Source {
        get => source;
        set {
            source = value.Remove(0, value.IndexOf('/') + 1);
            if (source.Contains('/')) {
                source = source.Remove(source.IndexOf('/'));
            }
        }
    }

    public string download_url;
    public string type;

    public WebFile() {
        
    }
}


