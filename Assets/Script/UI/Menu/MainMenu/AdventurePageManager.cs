using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Script.UI.Menu.MainMenu
{
    public class AdventurePageManager : MonoBehaviour
    {
        [SerializeField] private Transform _adventureContainer;
        [SerializeField] private GameObject _creatingPanel;
        
        [SerializeField] private GameObject _adventureListElement;

        [SerializeField] private Button _createAdventureButton;
        
        [SerializeField] private Button _createAdventureInCreatePanelButton;
        [SerializeField] private Button _cancelAdventureInCreatePanelButton;
        [SerializeField] private TMP_InputField _adventureNameInputField;
        
        private string path = Application.dataPath + "/Resources/Adventures/";

        private void Start() {
            _createAdventureButton.onClick.AddListener(OpenCreatingPanel);
            _cancelAdventureInCreatePanelButton.onClick.AddListener(CloseCreatingPanel);
            _createAdventureInCreatePanelButton.onClick.AddListener(CreateAdventure);
        }

        private void OnEnable() {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            
            ListAdventures();
        }

        void ListAdventures() {
            foreach (Transform child in _adventureContainer) {
                Destroy(child.gameObject);
            }
            
            string[] adventurePaths = Directory.GetDirectories(path);

            foreach (var adventurePath in adventurePaths) {
                GameObject obj = Instantiate(_adventureListElement, _adventureContainer);
                
                obj.GetComponent<AdventureListElement>().Init(adventurePath);
            }
        }
        
        void OpenCreatingPanel() {
            _adventureNameInputField.text = "";
            _creatingPanel.SetActive(true);
        }
        
        void CloseCreatingPanel() {
            _creatingPanel.SetActive(false);
        }

        void CreateAdventure() {
            string adventureName = _adventureNameInputField.text;
            string folderName = "";

            foreach (var sub in adventureName.Split(' ')) {
                folderName += sub[0];
            }

            string folderPath = path + folderName.ToLower();

            int counter = 0;
            string tempFolderPath = folderPath;
            
            while (Directory.Exists(tempFolderPath)) {
                tempFolderPath = folderPath + " " + ++counter;
            }

            folderPath = tempFolderPath;

            Directory.CreateDirectory(folderPath);
            File.Create(folderPath + $"/.{adventureName}");
            
            CloseCreatingPanel();
            ListAdventures();
        }
    }
}