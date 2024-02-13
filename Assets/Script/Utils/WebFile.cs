using Newtonsoft.Json;

public class WebFile
{
    private string name;
    
    [JsonProperty("name")]public string Name {
        get;
        set;
    }

    public string download_url;
    public string type;
}
