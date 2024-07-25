using UnityEngine;

namespace Script.FileHandling
{
    public class FileManager : MonoBehaviour
    {
        public string subPath;

        public static FileManager Instance;
        
        private void Awake() {
            DontDestroyOnLoad(gameObject);
            
            Instance = this;
        }
    }
}