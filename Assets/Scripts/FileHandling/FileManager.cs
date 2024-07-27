using UnityEngine;

namespace FileHandling
{
    public class FileManager : MonoBehaviour
    {
        public static readonly string MonsterManualPath = Application.streamingAssetsPath + "/monster_manual.json";
        
        public string adventurePath;
        public string creaturePath;
        public string playerPath;
        public string sceneFolderPath;

        public static FileManager Instance;
        
        private void Awake() {
            DontDestroyOnLoad(gameObject);
            
            Instance = this;
        }

        public void SetPaths(string adventurePath) {
            this.adventurePath = adventurePath;
            creaturePath = $"{adventurePath}/creatures.json";
            playerPath = $"{adventurePath}/players.json";
            sceneFolderPath = $"{adventurePath}/Scenes/";
        }
    }
}
