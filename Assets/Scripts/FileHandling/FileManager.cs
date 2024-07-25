using UnityEngine;

namespace Script.FileHandling
{
    public class FileManager : MonoBehaviour
    {
        public string path;

        public static FileManager Instance;
        
        private void Awake() {
            DontDestroyOnLoad(gameObject);
            
            Instance = this;
        }
    }
}