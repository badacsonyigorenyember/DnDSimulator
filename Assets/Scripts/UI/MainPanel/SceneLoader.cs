using System;
using Network;
using UI.MainPanel.ListElements;
using UnityEngine;

namespace UI.MainPanel
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] private Camera _cam;

        private void Update() {
            SelectScene();
        }

        void SelectScene() {
            Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, LayerMask.GetMask("SceneListElement"));

            if (hit.collider == null) {
                return;
            }
            
            //SceneHandler.Instance.LoadScene(hit.collider.GetComponent<SceneListElement>().sceneName);
        }
    }
}