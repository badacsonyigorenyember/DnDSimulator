using System.IO;
using System.Linq;
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

        public void Init(string path) {
            _playButton.onClick.AddListener(StartGame);
            _deleteButton.onClick.AddListener(DeleteAdventure);
            
            adventureSubPath = path;

            _adventureNameText.text = new DirectoryInfo(path).Name;
        }

        void StartGame() {
            FileManager.Instance.path = adventureSubPath;
            NetworkManagerHelper.Instance.StartHost();
        }

        void DeleteAdventure() {
            Directory.Delete(adventureSubPath, true);
            Destroy(gameObject);
        }
    }
}