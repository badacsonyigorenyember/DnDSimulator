using Script.Utils;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CreatureEditPanelHandler : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI hpText;
    public TMP_InputField currentHpModifyInput;
    public TMP_InputField maxHpModifyInput;
    public TMP_InputField initiativeInput;
    [FormerlySerializedAs("creatureInfo")] public CreatureBehaviour creatureBehaviourInfo;

    public Button hpModifyButton;
    public Button statModifyButtin;

    private void Start() {
        hpModifyButton.onClick.AddListener(ModifyHp);
        statModifyButtin.onClick.AddListener(StatModify);
    }

    public void Init(CreatureBehaviour c) {
        creatureBehaviourInfo = c;
        nameText.text = c.creatureName;
        SetHpText();
    }

    void SetHpText() {
        hpText.text = creatureBehaviourInfo.currentHp + "/" + creatureBehaviourInfo.maxHp;
    }

    void StatModify() {
        string stringValue = maxHpModifyInput.text;
        if (stringValue == "") return;

        int intValue = int.Parse(stringValue);
        creatureBehaviourInfo.maxHp += intValue;

        //TODO initiative!!
    }

    void ModifyHp() {
        string stringValue = currentHpModifyInput.text;
        if (stringValue == "") return;

        int intValue = int.Parse(stringValue);
        creatureBehaviourInfo.currentHp += intValue;

        if (creatureBehaviourInfo.currentHp <= 0) {
            creatureBehaviourInfo.currentHp = 0;

            if (!creatureBehaviourInfo.isPlayer) KillCreature();
            return;
        }

        SetHpText();
    }

    void KillCreature() {
        //InitiativeHandler.Instance.RemoveEntityFromList(entityInfo);
        creatureBehaviourInfo.GetComponent<NetworkObject>().Despawn();
        creatureBehaviourInfo = null;
        gameObject.SetActive(false);
    }
}
