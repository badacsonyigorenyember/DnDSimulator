using System;
using System.IO;
using System.Linq;
using Script.FileHandling;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Script.UI.Menu.MainMenu
{
    public class AdventureListElement : MonoBehaviour
    {
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _deleteButton;
        [SerializeField] private TextMeshProUGUI _adventureNameText;

        private string adventureSubPath;

        public void Init(string subPath) {
            _playButton.onClick.AddListener(StartGame);
            _deleteButton.onClick.AddListener(DeleteAdventure);
            
            adventureSubPath = subPath;
            
            Debug.Log(subPath);
            
            string fileNamePath = Directory.GetFiles(subPath).First(file => Path.GetFileName(file).First() == '.');
            _adventureNameText.text = Path.GetFileName(fileNamePath).Remove(0, 1);
        }

        void StartGame() {
            FileManager.Instance.subPath = $"/{adventureSubPath}";
            NetworkManagerHelper.Instance.StartHost();
        }

        void DeleteAdventure() {
            Directory.Delete(adventureSubPath, true);
            Destroy(gameObject);
        }
    }
}