using UnityEngine;

namespace Interaction
{
    public class CreatureInteractionHandler : MonoBehaviour
    {
        public GameObject creaturePanel;

        public static CreatureInteractionHandler Instance;

        private void Awake() {
            Instance = this;
        }

        public void OpenCreaturePanel(Creature c) {
            creaturePanel.SetActive(true);
            creaturePanel.GetComponent<CreatureEditPanelHandler>().Init(c);
        }

        public void CancelCreaturePanel() {
            creaturePanel.SetActive(false);
        }
    }
}
