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

        private string adventurePath;

        public void Init(string path) {
            _playButton.onClick.AddListener(StartGame);
            _deleteButton.onClick.AddListener(DeleteAdventure);
            
            adventurePath = path;

            _adventureNameText.text = new DirectoryInfo(path).Name;
        }

        void StartGame() {
            FileManager.Instance.path = adventurePath;
            NetworkManagerHelper.Instance.StartHost();
        }

        void DeleteAdventure() {
            Directory.Delete(adventurePath, true);
            Destroy(gameObject);
        }
    }
}