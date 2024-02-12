using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class MonsterObj
{
    [JsonProperty("name")] public string Name;
    [JsonProperty("hp")] public JObject hp;
    
    public string download_url;

    public int GetHp => hp["average"]!.Value<int>();
}
