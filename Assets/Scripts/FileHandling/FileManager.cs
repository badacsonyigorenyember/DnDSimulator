using System.IO;
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
        public string sceneImgPath;

        public static FileManager Instance;
        
        private void Awake() {
            DontDestroyOnLoad(gameObject);
            
            Instance = this;
        }

        public void SetPaths(string adventurePath) {
            this.adventurePath = adventurePath;
            creaturePath = $"{adventurePath}/creatures.json";
            playerPath = $"{adventurePath}/players.json";
            sceneFolderPath = $"{adventurePath}/Scenes";
            creatureImgPath = $"{adventurePath}/CreatureImages"; 
            playerImgPathPath = $"{adventurePath}/PlayerImages";
            sceneImgPath = $"{adventurePath}/SceneImages";
            
            if (!Directory.Exists(sceneFolderPath)) {
                Directory.CreateDirectory(sceneFolderPath);
            }
            if (!Directory.Exists(creatureImgPath)) {
                Directory.CreateDirectory(creatureImgPath);
            }
            if (!Directory.Exists(playerImgPathPath)) {
                Directory.CreateDirectory(playerImgPathPath);
            }
            if (!Directory.Exists(sceneImgPath)) {
                Directory.CreateDirectory(sceneImgPath);
            }
            if (!File.Exists(creaturePath)) {
                File.Create(creaturePath);    
            }
            if (!File.Exists(playerPath)) {
                File.Create(playerPath);    
            }
        }
    }
}
