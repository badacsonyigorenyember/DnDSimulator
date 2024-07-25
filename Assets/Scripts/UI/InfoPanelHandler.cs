using System;
using System.Collections;
using System.IO;
using UI.Tooltip;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class InfoPanelHandler : NetworkBehaviour
    {
        [SerializeField] private Button _sceneListButton;
        [SerializeField] private Button _creatureListButton;
        [SerializeField] private CreateButton _createButton;

        [SerializeField] private GameObject _infoPanel;
        [SerializeField] private Transform _container;
        [SerializeField] private float _panelMovementTime;

        [SerializeField] private GameObject _creatureListElementPrefab;
        [SerializeField] private GameObject _sceneListElementPrefab;

        private bool _isOpen;
        private SelectedList _selectedList;

        private float _listElementInspectionTimer;

        public static Action RefreshAction;

        void Start() {
            _sceneListButton.onClick.AddListener(() => ListElements(SelectedList.Scene));
            _creatureListButton.onClick.AddListener(() => ListElements(SelectedList.Creature));

            _isOpen = false;
            _selectedList = SelectedList.NotSelected;

            RefreshAction += () => {
                ListElements(_selectedList, false);
                Debug.Log("Refreshed!");
            };


            RectTransform rect = _infoPanel.GetComponent<RectTransform>();

            if (rect.localScale.x > 0) {
                Debug.Log("rect");
                rect.localScale = Vector3.zero;
            }
        }

        void ListElements(SelectedList listType, bool activatePanel = true) {
            if (activatePanel) {
                if (_selectedList == listType || _selectedList == SelectedList.NotSelected) {
                    StartCoroutine(OpenClosePanelCoroutine());
                    _isOpen = !_isOpen;
                }

                if (!_isOpen) {
                    _selectedList = SelectedList.NotSelected;
                    return;
                }
            }

            ClearList();

            string[] files;
            _selectedList = listType;
            _createButton.SetType(listType);
            _createButton.GetComponent<TooltipTrigger>().Init("Create " + _selectedList.ToString().ToLower());

            switch (listType) {
                case SelectedList.Creature:
                    files = Directory.GetFiles(GameManager.CREATURE_IMG_PATH);

                    foreach (var file in files) {
                        if (Path.GetExtension(file) != ".png") continue;

                        string creatureName = Path.GetFileNameWithoutExtension(file);
                        byte[] img = File.ReadAllBytes(file);

                        GameObject obj = Instantiate(_creatureListElementPrefab, _container);
                        obj.GetComponent<CreatureListElement>().SetCreature(creatureName, img);
                    }

                    break;
                case SelectedList.Scene:
                    files = Directory.GetFiles(GameManager.SCENE_PATH);

                    foreach (var file in files) {
                        if (Path.GetExtension(file) != ".json") continue;

                        string sceneName = Path.GetFileNameWithoutExtension(file);

                        GameObject obj = Instantiate(_sceneListElementPrefab, _container);
                        obj.GetComponent<SceneListElement>().SetCreature(sceneName);
                    }

                    break;
            }
        }

        void ClearList() {
            foreach (Transform child in _container) {
                if (child.GetComponent<CreatureListElement>() != null || child.GetComponent<SceneListElement>() != null)
                    Destroy(child.gameObject);
            }
        }

        IEnumerator OpenClosePanelCoroutine() {
            RectTransform rect = _infoPanel.GetComponent<RectTransform>();

            Vector3 startScale = rect.localScale;
            Vector3 endScale = _isOpen ? Vector3.zero : Vector3.one;

            float elapsedTime = 0f;

            while (elapsedTime < _panelMovementTime) {
                elapsedTime += Time.deltaTime;
                rect.localScale = Vector3.Lerp(startScale, endScale, elapsedTime / _panelMovementTime);

                yield return null;
            }
        }
    }

    public enum SelectedList
    {
        NotSelected,
        Scene,
        Creature
    }
}