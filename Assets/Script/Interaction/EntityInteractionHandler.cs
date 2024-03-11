using System;
using Unity.VisualScripting;
using UnityEngine;

public class EntityInteractionHandler : MonoBehaviour
{
    public GameObject entityPanel;

    public static EntityInteractionHandler Instance;

    private void Awake() {
        Instance = this;
    }

    public void OpenEntityPanel(Entity e) {
        entityPanel.SetActive(true);
        entityPanel.GetComponent<EntityEditPanelHandler>().Init(e);

    }

    public void CancelEntityPanel() {
        entityPanel.SetActive(false);
    }
    
}
