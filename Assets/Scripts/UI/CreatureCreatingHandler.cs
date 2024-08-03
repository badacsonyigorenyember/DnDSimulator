using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FileHandling;
using Newtonsoft.Json;
using Script.Structs;
using SimpleFileBrowser;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils.Data;
using Utils.Interfaces;
using Random = UnityEngine.Random;

namespace UI
{
    public class CreatureCreatingHandler : MonoBehaviour
    {
        [SerializeField] private Button _openCreatingButton;

        [SerializeField] private TMP_InputField _creatureNameInputField;
        [SerializeField] private TMP_InputField _maxHPInputField;
        [SerializeField] private TMP_InputField _initiativeModifierInputField;
        [SerializeField] private TMP_InputField _armorClassInputField;
        [SerializeField] private Toggle _isPlayerToggle;
        [SerializeField] private Image _loadedCreatureImage;

        [SerializeField] private Button _selectImageButton;
        [SerializeField] private Button _cancelCreatureButton;
        [SerializeField] private Button _createCreatureButton;

        [SerializeField] private TextMeshProUGUI _console;
        [SerializeField] private GameObject _overWritePanel;

        private byte[] _image;

        private List<string> _exampleNames = new List<string>()
        {
            "Goblin",
            "Orc",
            "Wolf",
            "Adult Red Dragon",
            "Norgar",
            "Xavfyr",
            "Gylcosecolleoth",
            "Gifsad",
            "Yoplavir",
        };

        void Start() {
            FileBrowser.SetFilters(true, new FileBrowser.Filter("Images", ".png"));
            FileBrowser.SetDefaultFilter(".png");
            FileBrowser.SetExcludedExtensions(".lnk", ".tmp", ".zip", ".rar", ".exe");
            FileBrowser.AddQuickLink("Users", "C:\\Users");

            _openCreatingButton.onClick.AddListener(() => {
                if (_openCreatingButton.GetComponent<CreateButton>().selected == SelectedList.Creature) {
                    Init();
                }
                else {
                    ClosePanel();
                }
            });
            _createCreatureButton.onClick.AddListener(CreateEntity);
            _cancelCreatureButton.onClick.AddListener(ClosePanel);
            _selectImageButton.onClick.AddListener(SelectImage);
        }

        void Init() {
            transform.GetChild(0).gameObject.SetActive(true);

            int rand = Random.Range(0, _exampleNames.Count);

            _creatureNameInputField.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text =
                _exampleNames[rand];
            _console.text = "";

            _loadedCreatureImage.sprite = null;
        }

        void SelectImage() {
            _console.text = "";
            _loadedCreatureImage.rectTransform.localScale = Vector3.one;

            FileBrowser.ShowLoadDialog(
                (path) => {
                    _image = File.ReadAllBytes(path.First());

                    Texture2D texture = new Texture2D(1, 1);
                    texture.LoadImage(_image);

                    if (texture.width is not (280 or 560) || texture.height is not (280 or 560)) {
                        _console.text = $"Bad image size! Should be 280x280 or 560x560, but it's {texture.width}x{texture.height}!";
                        _loadedCreatureImage.sprite = null;
                        return;
                    }

                    if (texture.width != texture.height) {
                        _console.text = $"Image is not square shaped, it's {texture.width}x{texture.height}!";
                        _loadedCreatureImage.sprite = null;
                        return;
                    }

                    _loadedCreatureImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                        Vector2.one / 2f, 200f);

                    _loadedCreatureImage.rectTransform.localScale = Vector3.one * texture.width / 280;
                },
                () => {
                    Debug.Log("Canceled");
                }, 
                FileBrowser.PickMode.Files, false, null,
                null, "Select image for creature!");
        }

        void CreateEntity() {
            FileManager fileManager = FileManager.Instance;
            if (!Regex.IsMatch(_creatureNameInputField.text, @"^[a-zA-Z\s]{3,}$")) {
                _console.text = "Bad name format. Name can't contain any number and must be at least 3 character long!";
                return;
            }

            if (_image == null) {
                _console.text = "Please select an image!";
                return;
            }

            int health = string.IsNullOrEmpty(_maxHPInputField.text) ? 0 : Convert.ToInt32(_maxHPInputField.text);


            CustomCreatureData data = new CustomCreatureData
            {
                uuid = "asdasd",        //TODO uuid generálás
                name = _creatureNameInputField.text,
                maxHealth = health,
                currentHealth = health,
                initiativeModifier = string.IsNullOrEmpty(_initiativeModifierInputField.text) ? 0 : Convert.ToInt32(_initiativeModifierInputField.text),
                armorClass = string.IsNullOrEmpty(_armorClassInputField.text) ? 0 : Convert.ToInt32(_armorClassInputField.text),
                abilities = new Abilities(),    //TODO: minden adatot neki
                visible = true,
                position = new Vector2(),   //TODO: pozíció lekérése
            };
            
            string path = fileManager.creaturePath + $"/{data.uuid}.json";

            List<IEntityData> allCreatures = JsonConvert.DeserializeObject<List<IEntityData>>(File.ReadAllText(path));

            allCreatures.Add(data);

            string json = JsonConvert.SerializeObject(allCreatures);
            
            Task.WaitAll(
                File.WriteAllTextAsync(path, json),
                File.WriteAllBytesAsync(fileManager.creatureImgPath + $"/{data.uuid}.png", _image)
            );

            Debug.Log("Successful save!");

            ClearPanel();
            InfoPanelHandler.RefreshAction.Invoke();
        }

        void ClearPanel() {
            _creatureNameInputField.text = "";
            _maxHPInputField.text = "";
            _initiativeModifierInputField.text = "";
            _isPlayerToggle.isOn = false;
            _loadedCreatureImage.sprite = null;
            _image = null;
        }

        void ClosePanel() {
            ClearPanel();

            transform.GetChild(0).gameObject.SetActive(false);
        }
    }
}
