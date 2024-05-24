using System;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.UI;

public class ModifyCreatureHandler : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI _pageTitle;
    [SerializeField] private TMP_InputField _nameInputField;
    [SerializeField] private TMP_InputField _currentHpInputField;
    [SerializeField] private TMP_InputField _maxHpInputField;
    [SerializeField] private TMP_InputField _initiativeInputField;
    [SerializeField] private Toggle _visibleToggle;
    
    [SerializeField] private Button _cancelButton;
    [SerializeField] private Button _saveButton;
    
    [SerializeField] private Camera _cam;
    
    private Creature _creature;

    private void Start() {
        _cancelButton.onClick.AddListener(ClosePanel);
        _saveButton.onClick.AddListener(SaveCreature);
    }

    private void Update() {
        if (Input.GetMouseButtonDown(1)) {
            Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, LayerMask.GetMask("Creature"));

            if (hit.collider != null) {
                Init(hit.collider.GetComponent<Creature>());
            }
        }
    }
    
    void Init(Creature creature) {
        transform.GetChild(0).gameObject.SetActive(true);
        
        _creature = creature;
        _pageTitle.text = _creature.creatureName + " modifying";
        _nameInputField.text = _creature.creatureName;
        _currentHpInputField.text = _creature.currentHp.ToString();
        _maxHpInputField.text = _creature.maxHp.ToString();
        _initiativeInputField.text = _creature.initiativeModifier.ToString();
        _visibleToggle.isOn = _creature.visible;
    }

    void SaveCreature() {
        _creature.creatureName = _nameInputField.text;
        _creature.currentHp = string.IsNullOrEmpty(_currentHpInputField.text)
            ? 0 : Convert.ToInt32(_currentHpInputField.text);
        _creature.maxHp = string.IsNullOrEmpty(_maxHpInputField.text) 
            ? 0 : Convert.ToInt32(_maxHpInputField.text);

        if (_creature.currentHp <= 0) {
            _creature.GetComponent<NetworkObject>().Despawn();
        }
        else {
            _creature.initiativeModifier = string.IsNullOrEmpty(_initiativeInputField.text) 
                ? 0 : Convert.ToInt32(_initiativeInputField.text);
            _creature.visible = _visibleToggle.isOn;
        
            _creature.SetVisibleClientRpc(_visibleToggle.isOn);
        }
        
        ClosePanel();
    }
    
    void ClearPanel() {
        _pageTitle.text = "";
        _nameInputField.text = "";
        _currentHpInputField.text = "";
        _maxHpInputField.text = "";
        _initiativeInputField.text = "";
        _visibleToggle.isOn = true;
    }

    void ClosePanel() {
        ClearPanel();
		
        transform.GetChild(0).gameObject.SetActive(false);
    }
    
    

    
}
