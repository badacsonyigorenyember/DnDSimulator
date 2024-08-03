using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FileHandling;
using Models.Interfaces;
using Newtonsoft.Json;
using UI.MainPanel.ListElements;
using UnityEngine;
using UnityEngine.UI;
using Utils.Data;


namespace UI.MainPanel
{
    public class SceneListPanel : MonoBehaviour
    {
        [SerializeField] private Transform _listElementsContainer;

        [SerializeField] private GameObject _sceneListElementPrefab;

        public void ListScenes() {
            string[] paths = Directory.GetFiles(FileManager.Instance.sceneFolderPath);

            foreach (var path in paths) {
                if (path.EndsWith(".meta")) return;
                
                string json = File.ReadAllText(path);
                SceneData data = JsonConvert.DeserializeObject<SceneData>(json);

                if (data == null) return;

                InstantiateScene(data.Name);
            }
        }

        void InstantiateScene(string sceneName) {
            SceneListElement sceneListElement = Instantiate(_sceneListElementPrefab, _listElementsContainer).GetComponent<SceneListElement>();
            sceneListElement.SetSceneListElement(sceneName);
        }
    }
}