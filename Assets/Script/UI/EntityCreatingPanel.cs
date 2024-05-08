using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using SimpleFileBrowser;
using TMPro;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class EntityCreatingPanel : MonoBehaviour
{
	[SerializeField] private Button _entityCreatingButton;
	
	[SerializeField] private TMP_InputField _entityNameInputField;
	[SerializeField] private TMP_InputField _maxHpInputField;
	[SerializeField] private TMP_InputField _initiativeModifierInputField;
	[SerializeField] private Toggle _isCharacterInput;
	[SerializeField] private Image _loadedImage;
	
	[SerializeField] private Button _selectImageButton;
	[SerializeField] private Button _cancelEntityCreatingButton;
	[SerializeField] private Button _saveEntityButton;
	
	[SerializeField] private TextMeshProUGUI _consol;
	[SerializeField] private GameObject _overWritePanel;
	
	private byte[] _image;
	
	private List<string> _exampleInputs = new List<string>()
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

	void Start()
	{
		FileBrowser.SetFilters( true, new FileBrowser.Filter( "Images", ".png" ));
		FileBrowser.SetDefaultFilter( ".png" );
		FileBrowser.SetExcludedExtensions( ".lnk", ".tmp", ".zip", ".rar", ".exe" );
		FileBrowser.AddQuickLink( "Users", "C:\\Users", null );

		_entityCreatingButton.onClick.AddListener(Init);
		_saveEntityButton.onClick.AddListener(SaveEntity);
		_cancelEntityCreatingButton.onClick.AddListener(ClosePanel);
		_selectImageButton.onClick.AddListener(SelectImage);
	}

	void Init() {
		transform.GetChild(0).gameObject.SetActive(true);
		_entityCreatingButton.gameObject.SetActive(false);
		
		int rand = Random.Range(0, _exampleInputs.Count);

		_entityNameInputField.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text =
			_exampleInputs[rand];
		_consol.text = "";
	}

	void ClearPanel() {
		_entityNameInputField.text = "";
		_maxHpInputField.text = "";
		_initiativeModifierInputField.text = "";
		_isCharacterInput.isOn = false;
		_loadedImage.sprite = null;
	}

	void ClosePanel() {
		ClearPanel();
		
		transform.GetChild(0).gameObject.SetActive(false);
		_entityCreatingButton.gameObject.SetActive(true);
	}

	void SelectImage() {
		_consol.text = "";
		_loadedImage.rectTransform.localScale = Vector3.one;
		
		FileBrowser.ShowLoadDialog(
			(path) => {
				_image = File.ReadAllBytes(path.First());

				Texture2D texture = new Texture2D(1, 1);
				texture.LoadImage(_image);

				if (texture.width is not (280 or 560) || texture.height is not (280 or 560)) {
					_consol.text = $"Bad image size! Should be 280x280 or 560x560, but it's {texture.width}x{texture.height}!";
					_loadedImage.sprite = null;
					return;
				}

				if (texture.width != texture.height) {
					_consol.text = $"Image is not square shaped, it's {texture.width}x{texture.height}!";
					_loadedImage.sprite = null;
					return;
				}
				
				_loadedImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
					Vector2.one / 2f, 200f);
				
				_loadedImage.rectTransform.localScale = Vector3.one * texture.width / 280;
			},
			() => {
				Debug.Log("Canceled");
			}, 
			FileBrowser.PickMode.Files, false, null, 
			null, "Select image for entity", "Select");
	}
	

	async void SaveEntity() {
		if (!Regex.IsMatch(_entityNameInputField.text, @"^[a-zA-Z\s]{3,}$")) {
			_consol.text = "Bad name format. Name can't contain any number and must be at least 3 character long!";
			return;
		}

		if (_image == null) {
			_consol.text = "Please select an image!";
			return;
		}

		EntityData data = new EntityData
		{
			entityName = _entityNameInputField.text,
			maxHp = string.IsNullOrEmpty(_maxHpInputField.text) ? 0 : Convert.ToInt32(_maxHpInputField.text),
			isCharacter = _isCharacterInput.isOn,
			initiativeModifier = string.IsNullOrEmpty(_initiativeModifierInputField.text) ? 0 : Convert.ToInt32(_initiativeModifierInputField.text)
		};
		
		string path = GameManager.ENTITY_DATA_PATH + $"/{data.entityName}.json";
		
		if (File.Exists(path)) {
			GameObject obj = Instantiate(_overWritePanel, transform);
			OverWriteConfirm confirm = obj.GetComponent<OverWriteConfirm>();

			bool confirmationResult = await confirm.GetResult();
			Destroy(confirm.gameObject);

			if (!confirmationResult) {
				return;
			}
		}

		string json = JsonUtility.ToJson(data);

		Task.WaitAll(
		File.WriteAllTextAsync(path, json), 
			File.WriteAllBytesAsync(GameManager.ENTITY_IMG_PATH + $"/{data.entityName}.png", _image)
		);
		
		Debug.Log("Successful save!");
		
		ClearPanel();

	}



}