using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class MonsterData
{
    public string Name;
    public int Hp;
    public string DownloadUrl;

    public MonsterData(string name, int hp) {
        Name = name;
        Hp = hp;
    }

    public void SaveToDisk() {
        string json = JsonConvert.SerializeObject(this);
        
        File.WriteAllText(GameManager.DATA_SAVE_PATH + Name + ".json", json);
    }
    
}
