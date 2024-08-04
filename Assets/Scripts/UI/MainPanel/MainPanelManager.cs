using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI.MainPanel
{
    public class MainPanelManager : MonoBehaviour
    {
        [SerializeField] private Button _monsterListPanelButton;
        [SerializeField] private Button _sceneListPanelButton;
        
        [SerializeField] private Button _createSceneButton;

        [SerializeField] private MonsterListPanel _monsterListPanel;
        [SerializeField] private SceneListPanel _sceneListListPanel;
        
        [SerializeField] private SceneCreatingHandler _createScenePanel;

        public static MainPanelManager Instance;
        
        private MainPanelState _state;

        private void Awake() {
            Instance = this;
        }

        private void Start() {
            _monsterListPanelButton.onClick.AddListener(() => ChangePanel(MainPanelState.Monster));
            _sceneListPanelButton.onClick.AddListener(() => ChangePanel(MainPanelState.Scene));
            
            _createSceneButton.onClick.AddListener(_createScenePanel.Init);
            
            _sceneListListPanel.ListScenes();
            _monsterListPanel.ListMonsters();
        }

        void ChangePanel(MainPanelState nextState) {
            switch (nextState) {
                case MainPanelState.Scene: {
                    _monsterListPanel.gameObject.SetActive(false);
                    _sceneListListPanel.gameObject.SetActive(true);
                    break;
                }
                case MainPanelState.Monster: {
                    _sceneListListPanel.gameObject.SetActive(false);
                    _monsterListPanel.gameObject.SetActive(true);
                    break;
                }
            }
        }

        public void ListScenes() {
            _sceneListListPanel.ListScenes();
        }
    }
}

enum MainPanelState
{
    Scene,
    Monster
}