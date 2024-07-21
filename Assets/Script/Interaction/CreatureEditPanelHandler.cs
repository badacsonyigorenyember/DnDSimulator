using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CreatureEditPanelHandler : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI hpText;
    public TMP_InputField currentHpModifyInput;
    public TMP_InputField maxHpModifyInput;
    public TMP_InputField initiativeInput;
    public Creature creatureInfo;

    public Button hpModifyButton;
    public Button statModifyButtin;

    private void Start() {
        hpModifyButton.onClick.AddListener(ModifyHp);
        statModifyButtin.onClick.AddListener(StatModify);
    }

    public void Init(Creature c) {
        creatureInfo = c;
        nameText.text = c.creatureName;
        SetHpText();
    }

    void SetHpText() {
        hpText.text = creatureInfo.currentHp + "/" + creatureInfo.maxHp;
    }

    void StatModify() {
        string stringValue = maxHpModifyInput.text;
        if (stringValue == "") return;

        int intValue = int.Parse(stringValue);
        creatureInfo.maxHp += intValue;

        //TODO initiative!!
    }

    void ModifyHp() {
        string stringValue = currentHpModifyInput.text;
        if (stringValue == "") return;

        int intValue = int.Parse(stringValue);
        creatureInfo.currentHp += intValue;

        if (creatureInfo.currentHp <= 0) {
            creatureInfo.currentHp = 0;

            if (!creatureInfo.isPlayer) KillCreature();
            return;
        }

        SetHpText();
    }

    void KillCreature() {
        //InitiativeHandler.Instance.RemoveEntityFromList(entityInfo);
        creatureInfo.GetComponent<NetworkObject>().Despawn();
        creatureInfo = null;
        gameObject.SetActive(false);
    }
}
