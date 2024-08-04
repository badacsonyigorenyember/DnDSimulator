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
            string[] paths = Directory.GetFiles(FileManager.Instance.sceneFolderPath);

            foreach (var path in paths) {//TODO: ne adja hozzá többször újranyitásnál (vagy clear-elni, de a pluszt megtartani, vagy csak nem addolni)
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
    }
}