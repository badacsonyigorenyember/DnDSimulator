using UnityEngine;

namespace FileHandling
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