using System.IO;
using Script.FileHandling;
using TMPro;
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
        }

        void StartGame() {
            FileManager.Instance.subPath = adventureSubPath;
            NetworkManagerHelper.Instance.StartHost();
        }

        void DeleteAdventure() {
            Directory.Delete(adventureSubPath, true);
            Destroy(gameObject);
        }
    }
}