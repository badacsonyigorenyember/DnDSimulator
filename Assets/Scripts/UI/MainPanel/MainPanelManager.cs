using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI.MainPanel
{
    public class MainPanelManager : MonoBehaviour
    {
        [SerializeField] private Button _monsterListPanelButton;

        [SerializeField] private MonsterListPanel _monsterListPanel;

        private void Start() {
            _monsterListPanelButton.onClick.AddListener(_monsterListPanel.ListMonsters);
        }
    }
}