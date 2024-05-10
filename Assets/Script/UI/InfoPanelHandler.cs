using System;
using System.Collections;
using System.IO;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class InfoPanelHandler : NetworkBehaviour
{
    [SerializeField] private Button _sceneListButton;
    [SerializeField] private Button _creatureListButton;
    
    [SerializeField] private GameObject _infoPanel;
    [SerializeField] private Transform _container;
    [SerializeField] private float _panelMovementTime;

    [SerializeField] private GameObject _creatureListElementPrefab;
    [SerializeField] private GameObject _sceneListElementPrefab;
    
    [SerializeField] private GameObject _listElementNameTextBoxPrefab;

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
        };
    }

    void ListElements(SelectedList listType, bool activatePanel = true) {
        if (activatePanel ) {
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

        switch (listType) {
            case SelectedList.Creature:
                files = Directory.GetFiles(GameManager.CREATURE_IMG_PATH);
        
                foreach (var file in files) {
                    if(Path.GetExtension(file) != ".png") continue;

                    string creatureName = Path.GetFileNameWithoutExtension(file);
                    byte[] img = File.ReadAllBytes(file);
            
                    GameObject obj = Instantiate(_creatureListElementPrefab, _container);
                    obj.GetComponent<CreatureListElement>().SetCreature(creatureName, img);
                }
                break;
            case SelectedList.Scene:
                files = Directory.GetFiles(GameManager.CREATURE_IMG_PATH);
        
                foreach (var file in files) {
                    if(Path.GetExtension(file) != ".json") continue;
                    
                    
                }
                break;
        }
    }
    
    void ClearList() {
        foreach (Transform child in _container) {
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
    
    

    enum SelectedList
    {
        NotSelected,
        Scene,
        Creature
    }
}
