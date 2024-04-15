using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InfoPanelHandler : NetworkBehaviour
{
    [SerializeField] private Canvas _canvas;
    [SerializeField] private Button _sceneManagerButton;
    [SerializeField] private GameObject _infoPanel;
    [SerializeField] private float _panelMovementTime;

    private bool _isOpen;
    
    void Start() {
        if (!IsServer) {
            _canvas.gameObject.SetActive(false);
        }
        _sceneManagerButton.onClick.AddListener(SceneManagerPanelHandler);
    }

    void SceneManagerPanelHandler() {
        StartCoroutine(OpenClosePanel());
        if (!_isOpen) return;
    }

    IEnumerator OpenClosePanel() {
        RectTransform rect = _infoPanel.GetComponent<RectTransform>();
        
        Vector3 startPos = rect.localPosition;
        Vector3 endPos = _isOpen ? startPos + Vector3.down * rect.rect.height : startPos - Vector3.down * rect.rect.height;
        
        float elapsedTime = 0f;

        while (elapsedTime < _panelMovementTime) {
            elapsedTime += Time.deltaTime;
            rect.localPosition = Vector3.Lerp(startPos, endPos, (elapsedTime / _panelMovementTime));
            yield return null;
        }

        _isOpen = !_isOpen;
    }
    
    
    
}
