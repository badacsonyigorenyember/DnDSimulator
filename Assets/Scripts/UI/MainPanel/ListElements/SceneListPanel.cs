#nullable enable
using System.IO;
using FileHandling;
using Newtonsoft.Json;
using UI.MainPanel.ListElements;
using UnityEngine;
using Utils.Data;


namespace UI.MainPanel
{
    public class SceneListPanel : MonoBehaviour
    {
        [SerializeField] private Transform _listElementsContainer;

        [SerializeField] private GameObject _sceneListElementPrefab;

        public void ListScenes() {
            ClearScenes();
            string[] paths = Directory.GetFiles(FileManager.Instance.sceneFolderPath);

            foreach (var path in paths) {
                if (path.EndsWith(".meta")) continue;
                
                string json = File.ReadAllText(path);
                SceneData data = JsonConvert.DeserializeObject<SceneData>(json);

                if (data == null) continue;

                InstantiateScene(data.Name);
            }
        }

        void InstantiateScene(string sceneName) {
            SceneListElement sceneListElement = Instantiate(_sceneListElementPrefab, _listElementsContainer).GetComponent<SceneListElement>();
            sceneListElement.SetSceneListElement(sceneName);
        }

        private void ClearScenes() {
            for (int i = _listElementsContainer.childCount - 1; i >= 0; i--) {
                SceneListElement? sceneListElement = _listElementsContainer.GetChild(i).GetComponent<SceneListElement>();
                Destroy(sceneListElement?.gameObject);
            }
        }
    }
}