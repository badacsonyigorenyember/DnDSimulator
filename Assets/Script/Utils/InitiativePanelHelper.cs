using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InitiativePanelHelper : MonoBehaviour
{
    public Button removeEntityButton;
    public Entity entity;
    public int place;

    private void Start() {
        removeEntityButton.onClick.AddListener(() => {
            InitiativeHandler.Instance.RemoveEntityFromList(entity);
            Destroy(gameObject);
        });
    }

    public void Init(Entity entity, int place, bool isCurrentEntity) {
        this.entity = entity;
        this.place = place;
        TextMeshProUGUI textMeshPro = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        textMeshPro.text = this.place + ". - " + this.entity.name;
        textMeshPro.color = isCurrentEntity ? Color.green : Color.black;
    }
}
