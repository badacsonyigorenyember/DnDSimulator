using System;
using Network;
using TMPro;
using UI.Tooltip;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI.MainPanel.ListElements
{
    public class SceneListElement : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;
        
        public string sceneName;
        
        public void SetSceneListElement(string name) {
            sceneName = name;
            _text.text = name;
            
            GetComponent<TooltipTrigger>().header = name;
        }

        private void OnMouseDown() {
            Debug.Log("Clicked");
            SceneHandler.Instance.LoadScene(sceneName);
        }
    }
}