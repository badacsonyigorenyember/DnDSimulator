using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class EntityEditPanelHandler : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI hpText;
    public TMP_InputField currentHpModifyInput;
    public TMP_InputField maxHpModifyInput;
    public TMP_InputField initiativeInput;
    public Entity entityInfo;

    public Button hpModifyButton;
    public Button statModifyButtin;

    private void Start() {
        hpModifyButton.onClick.AddListener(ModifyHp);
        statModifyButtin.onClick.AddListener(StatModify);
    }

    public void Init(Entity e) {
        entityInfo = e;
        nameText.text = e.name;
        SetHpText();
    }

    void SetHpText() {
        hpText.text = entityInfo.currentHp + "/" + entityInfo.maxHp;
    }

    void StatModify() {
        string stringValue = maxHpModifyInput.text;
        if (stringValue == "") return;

        int intValue = int.Parse(stringValue);
        entityInfo.maxHp += intValue;
        
        //TODO initiative!!
    }

    void ModifyHp() {
        string stringValue = currentHpModifyInput.text;
        if (stringValue == "") return;

        int intValue = int.Parse(stringValue);
        entityInfo.currentHp += intValue;

        if (entityInfo.currentHp <= 0) {
            entityInfo.currentHp = 0;

            if (!entityInfo.isCharacter) KillEntity();
            return;
        }
        
        SetHpText();
    }

    void KillEntity() {
        //InitiativeHandler.Instance.RemoveEntityFromList(entityInfo);
        entityInfo.GetComponent<NetworkObject>().Despawn();
        entityInfo = null;
        gameObject.SetActive(false);
    }
}
