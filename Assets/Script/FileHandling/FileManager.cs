using System;
using System.Net;
using System.Net.Sockets;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

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