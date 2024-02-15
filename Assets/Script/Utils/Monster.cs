
using Newtonsoft.Json;

public class Monster : Instantiatable
{
    public int hp;

    [JsonConstructor]
    public Monster(string name, int hp, string adventure, string extension) {
        this.name = name;
        this.hp = hp;
        this.adventure = adventure;
        this.extension = extension;
    }

    public WebFile ConvertToWebFile() {
        return new WebFile();
    }
}
        
