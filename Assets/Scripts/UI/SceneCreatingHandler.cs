using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FileHandling;
using Models;
using Network;
using Newtonsoft.Json;
using SimpleFileBrowser;
using Structs;
using TMPro;
using UI.MainPanel;
using UnityEngine;
using UnityEngine.UI;
using Utils.Data;

namespace UI
{
    public class SceneCreatingHandler : MonoBehaviour
    {
        public int maxImageSize;

        [SerializeField] private Button _openCreatingButton;

        [SerializeField] private TMP_InputField _sceneNameInputField;
        [SerializeField] private Image _loadedSceneImage;

        [SerializeField] private Button _selectImageButton;
        [SerializeField] private Button _cancelCreatureButton;
        [SerializeField] private Button _createSceneButton;

        [SerializeField] private TextMeshProUGUI _console;
        [SerializeField] private GameObject _overWritePanel;

        private byte[] _image;

        void Start() {
            FileBrowser.SetFilters(true, new FileBrowser.Filter("Images", ".png"));
            FileBrowser.SetDefaultFilter(".png");
            FileBrowser.SetExcludedExtensions(".lnk", ".tmp", ".zip", ".rar", ".exe");
            FileBrowser.AddQuickLink("Users", "C:\\Users");

            _createSceneButton.onClick.AddListener(CreateScene);
            _cancelCreatureButton.onClick.AddListener(ClosePanel);
            _selectImageButton.onClick.AddListener(SelectImage);
        }

        public void Init() {
            transform.GetChild(0).gameObject.SetActive(true);

            ClearPanel();
        }

        void SelectImage() {
            _console.text = "";
            _loadedSceneImage.rectTransform.localScale = Vector3.one;

            FileBrowser.ShowLoadDialog(
                (path) => {
                    _image = File.ReadAllBytes(path.First());

                    if (_image.Length > maxImageSize * 1_048_576) {
                        _console.text = $"Too large image! Image must be under 25 MB in size!";
                        _loadedSceneImage.sprite = null;
                        _loadedSceneImage.rectTransform.localScale *= 0;
                        return;
                    }

                    Texture2D texture = new Texture2D(1, 1);
                    texture.LoadImage(_image);

                    _loadedSceneImage.rectTransform.localScale = Vector3.one;
                    _loadedSceneImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                        Vector2.one / 2f);

                    float modifier;

                    if (texture.width / texture.height > 16 / 9) {
                        modifier = _loadedSceneImage.rectTransform.sizeDelta.x / texture.width;
                    }
                    else {
                        modifier = _loadedSceneImage.rectTransform.sizeDelta.y / texture.height;
                    }

                    _loadedSceneImage.rectTransform.sizeDelta = new Vector2(texture.width * modifier, texture.height * modifier);

                },
                () => {
                    Debug.Log("Canceled");
                }, 
                FileBrowser.PickMode.Files, false, null,
                null, "Select image for creature!");
        }

        async void CreateScene() {
            if (string.IsNullOrWhiteSpace(_sceneNameInputField.text) || _sceneNameInputField.text.Length < 3) {
                _console.text = "Scene name is incorrect! It must be at least 3 character long!";
                _sceneNameInputField.text = "";
                return;
            }

            if (_image == null) {
                _console.text = "Please select an image!";
                return;
            }

            string sceneName = _sceneNameInputField.text;
            string path = FileManager.Instance.sceneFolderPath + $"/{sceneName}.json";

            if (File.Exists(path)) {
                GameObject obj = Instantiate(_overWritePanel, transform);
                OverWriteConfirm confirm = obj.GetComponent<OverWriteConfirm>();

                bool confirmationResult = await confirm.GetResult();
                Destroy(confirm.gameObject);

                if (!confirmationResult) {
                    return;
                }
            }

            Camera mainCamera = Camera.main;
            if (mainCamera is null) {
                Debug.Log("Main camera is null!");
                throw new Exception("Main camera is null!");
            }
            
            SceneData scene = new SceneData(sceneName)
            {
                ZoomScale = mainCamera.orthographicSize,
                CamPosition = new Position(mainCamera.transform.position)
            };

            string json = JsonConvert.SerializeObject(scene);

            FileManager fileManager = FileManager.Instance;
            
            await Task.WhenAll(new[]
            {
                File.WriteAllTextAsync(fileManager.sceneFolderPath + $"/{sceneName}.json", json),
                File.WriteAllBytesAsync(fileManager.sceneImgPath + $"/{sceneName}.png", _image)
            });


            SceneHandler.Instance.LoadMap(sceneName);

            GameManager.Instance.currentScene = new Scene(scene);

            MainPanelManager.Instance.ListScenes();
            
            ClosePanel();
        }

        void ClearPanel() {
            _sceneNameInputField.text = "";
            _console.text = "";
            _loadedSceneImage.sprite = null;
            _loadedSceneImage.rectTransform.localScale *= 0;
        }

        void ClosePanel() {
            ClearPanel();

            transform.GetChild(0).gameObject.SetActive(false);
        }
    }
}
