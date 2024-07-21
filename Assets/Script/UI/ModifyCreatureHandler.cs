using Script.Utils;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ModifyCreatureHandler : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI _pageTitle;
    [SerializeField] private TMP_InputField _nameInputField;
    [SerializeField] private TextMeshProUGUI _currentHPText;
    [SerializeField] private TextMeshProUGUI _maxHPText;
    [SerializeField] private TextMeshProUGUI _armorClassText;
    [SerializeField] private TextMeshProUGUI _initiativeModifierText;
    [SerializeField] private Toggle _visibleToggle;

    [SerializeField] private Button _cancelButton;

    [SerializeField] private Camera _cam;

    private CreatureBehaviour _creatureBehaviour;

    private void Start() {
        _cancelButton.onClick.AddListener(ClosePanel);
    }

    private void Update() {
        if (Input.GetMouseButtonDown(1)) {
            Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, LayerMask.GetMask("Creature"));

            if (hit.collider != null) {
                Init(hit.collider.GetComponent<CreatureBehaviour>());
            }
        }
    }

    void Init(CreatureBehaviour creatureBehaviour) {
        transform.GetChild(0).gameObject.SetActive(true);

        _creatureBehaviour = creatureBehaviour;
        _pageTitle.text = _creatureBehaviour.creatureName + " modifying";

        _nameInputField.text = _creatureBehaviour.creatureName;
        _nameInputField.onValueChanged.AddListener((value) => _creatureBehaviour.creatureName = value);

        _currentHPText.text = _creatureBehaviour.currentHp.ToString();
        _maxHPText.text = _creatureBehaviour.maxHp.ToString();
        _armorClassText.text = _creatureBehaviour.armorClass.ToString();
        _initiativeModifierText.text = _creatureBehaviour.initiativeModifier.ToString();

        _visibleToggle.isOn = _creatureBehaviour.visible;
        _visibleToggle.onValueChanged.AddListener((value) => _creatureBehaviour.SetVisibleClientRpc(value));
    }

    public void ModifyValue(string modifyVariable, int value) {
        switch (modifyVariable) {
            case "maxHP":
                _creatureBehaviour.maxHp = Mathf.Max(0, _creatureBehaviour.maxHp + value);
                _maxHPText.text = _creatureBehaviour.maxHp.ToString();
                break;
            case "currentHP":
                _creatureBehaviour.currentHp = Mathf.Max(0, _creatureBehaviour.currentHp + value);
                _currentHPText.text = _creatureBehaviour.currentHp.ToString();
                break;
            case "armor":
                _creatureBehaviour.armorClass = Mathf.Max(0, _creatureBehaviour.armorClass + value);
                _armorClassText.text = _creatureBehaviour.armorClass.ToString();
                break;
            case "initiative":
                _creatureBehaviour.initiativeModifier = Mathf.Max(0, _creatureBehaviour.initiativeModifier + value);
                _initiativeModifierText.text = _creatureBehaviour.initiativeModifier.ToString();
                break;
        }
    }

    void ClearPanel() {
        _pageTitle.text = "";
        _nameInputField.text = "";
        _currentHPText.text = "";
        _maxHPText.text = "";
        _initiativeModifierText.text = "";
        _visibleToggle.isOn = true;
    }

    void ClosePanel() {
        ClearPanel();

        transform.GetChild(0).gameObject.SetActive(false);
    }
}
