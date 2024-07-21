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

    private Creature _creature;

    private void Start() {
        _cancelButton.onClick.AddListener(ClosePanel);
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
        _nameInputField.onValueChanged.AddListener((value) => _creature.creatureName = value);

        _currentHPText.text = _creature.currentHp.ToString();
        _maxHPText.text = _creature.maxHp.ToString();
        _armorClassText.text = _creature.armorClass.ToString();
        _initiativeModifierText.text = _creature.initiativeModifier.ToString();

        _visibleToggle.isOn = _creature.visible;
        _visibleToggle.onValueChanged.AddListener((value) => _creature.SetVisibleClientRpc(value));
    }

    public void ModifyValue(string modifyVariable, int value) {
        switch (modifyVariable) {
            case "maxHP":
                _creature.maxHp = Mathf.Max(0, _creature.maxHp + value);
                _maxHPText.text = _creature.maxHp.ToString();
                break;
            case "currentHP":
                _creature.currentHp = Mathf.Max(0, _creature.currentHp + value);
                _currentHPText.text = _creature.currentHp.ToString();
                break;
            case "armor":
                _creature.armorClass = Mathf.Max(0, _creature.armorClass + value);
                _armorClassText.text = _creature.armorClass.ToString();
                break;
            case "initiative":
                _creature.initiativeModifier = Mathf.Max(0, _creature.initiativeModifier + value);
                _initiativeModifierText.text = _creature.initiativeModifier.ToString();
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
