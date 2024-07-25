/*
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InitiativePanelHelper : MonoBehaviour
{
    public Button removeCreatureButton;
    public Creature creature;
    public int place;

    private void Start() {
        removeCreatureButton.onClick.AddListener(() => {
            InitiativeHandler.Instance.RemoveEntityFromList(creature);
            Destroy(gameObject);
        });
    }

    public void Init(Creature creature, int place, bool isCurrentEntity) {
        this.creature = creature;
        this.place = place;
        TextMeshProUGUI textMeshPro = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        textMeshPro.text = this.place + ". - " + this.creature.creatureName;
        textMeshPro.color = isCurrentEntity ? Color.green : Color.black;
    }
}
*/
