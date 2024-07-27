using UnityEngine;

namespace FileHandling
{
    public class FileManager : MonoBehaviour
    {
        public static readonly string MonsterManualPath = Application.streamingAssetsPath + "/monster_manual.json";
        
        public string path;
        public string creaturePath;
        public string playerPath;

        public static FileManager Instance;
        
        private void Awake() {
            DontDestroyOnLoad(gameObject);
            
            Instance = this;
        }

        public void SetPaths(string adventurePath) {
            path = adventurePath;
            creaturePath = $"{path}/creatures.json";
            playerPath = $"{path}/players.json";
        }
    }
}
