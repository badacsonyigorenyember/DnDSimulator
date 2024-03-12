using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InitiativePanelHelper : MonoBehaviour
{
    public Button removeEntityButton;
    public Entity entity;

    private void Start() {
        removeEntityButton.onClick.AddListener(() => {
            InitiativeHandler.Instance.RemoveEntityFromList(entity);
            Destroy(gameObject);
        });
    }

    public void Init(Entity entity, int place) {
        this.entity = entity;
        transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = place + ". - " + this.entity.name;
    }
}
