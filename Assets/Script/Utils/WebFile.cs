using Newtonsoft.Json;

public class WebFile
{
    private string name;
    
    [JsonProperty("name")]public string Name {
        get => name;
        set => name = value.Remove(value.Length - 4);
    }

    public string download_url;
}
