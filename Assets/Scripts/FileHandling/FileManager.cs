using UnityEngine;

namespace FileHandling
{
    public class FileManager : MonoBehaviour
    {
        public static readonly string MonsterManualPath = Application.streamingAssetsPath + "/monster_manual.json";
        public static readonly string MonsterManualImagesPath = Application.streamingAssetsPath + "/monster_manual_images/";

        public string adventurePath;
        public string creaturePath;
        public string playerPath;
        public string sceneFolderPath;
        public string creatureImgPath;
        public string playerImgPathPath;

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
            creatureImgPath = $"{adventurePath}/CreatureImages"; 
            playerImgPathPath = $"{adventurePath}/PlayerImages";
        }
    }
}
