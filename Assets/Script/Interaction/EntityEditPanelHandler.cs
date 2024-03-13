using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class EntityEditPanelHandler : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI hpText;
    public TMP_InputField hpModifyInput;
    public Entity entityInfo;

    public Button hpModifyButton;
    public Button killButton;

    private void Start() {
        hpModifyButton.onClick.AddListener(ModifyHp);
        killButton.onClick.AddListener(KillEntity);
    }

    public void Init(Entity e) {
        entityInfo = e;
        nameText.text = e.name;
        SetHpText();
    }

    void SetHpText() {
        hpText.text = entityInfo.currentHp + "/" + entityInfo.maxHp;
    }

    void ModifyHp() {
        string stringValue = hpModifyInput.text;
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
        InitiativeHandler.Instance.RemoveEntityFromList(entityInfo);
        entityInfo.GetComponent<NetworkObject>().Despawn();
        entityInfo = null;
        gameObject.SetActive(false);
    }
}
