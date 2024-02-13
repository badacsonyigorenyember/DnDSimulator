using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FileListScrollView : MonoBehaviour
{
    [SerializeField] private Transform Content;
    [SerializeField] private GameObject Prefab;
    [SerializeField] private TMP_InputField SearchInput;

    [SerializeField] private Button ShowButton;
    [SerializeField] private Button HideButton;
    [SerializeField] private Button BackButton;
    
    private List<GameObject> FileObjects = new List<GameObject>();
    
    private string searchText = "";
    
    void Start() {
        StartCoroutine(DataHandler.GetImagesList());
        DataHandler.OnDataDownloaded += () => {
            searchText = "";
            SearchInput.text = "";
            FillScrollViewWithData();
        };
        
        HideButton.onClick.AddListener(HidePanel);
        ShowButton.onClick.AddListener(Showpanel);
        BackButton.onClick.AddListener(Back);
    }
    

    private void Update() {
        if (SearchInput.isFocused && searchText != SearchInput.text) {
            searchText = SearchInput.text;
            FillScrollViewWithData();
        }
    }

    void FillScrollViewWithData() {
        foreach (var obj in FileObjects) {
            Destroy(obj);
        }
        FileObjects.Clear();

        foreach (var file in DataHandler.files.Where(f => f.Name.ToLower().Contains(searchText.ToLower())).ToList()) {
            GameObject obj = Instantiate(Prefab, Content);
            obj.name = file.Name;
            
            if (obj.TryGetComponent(out RectTransform rect)) {
                rect.anchorMax = new Vector2(1, 1);
                rect.position = new Vector3(0, 0, 0);
            }
            
            obj.GetComponent<FileListScrollDisplay>().Init(file);
            
            FileObjects.Add(obj);
        }
        
    }

    private void Showpanel() {
        Debug.Log("Show");
        StartCoroutine(Move(190, ShowButton, HideButton));
    }

    private void HidePanel() {
        Debug.Log("Hide");
        StartCoroutine(Move(-190, HideButton, ShowButton));
    }

    private IEnumerator Move(int destination, Button a, Button b) {
        RectTransform t = GetComponent<RectTransform>();

        Vector3 startPos = t.position;
        float elapsedTime = 0f;
        
        while (elapsedTime < 0.3f) {
            t.position = Vector3.Lerp(startPos, startPos + Vector3.right * destination , elapsedTime / 0.3f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        t.position = startPos + Vector3.right * destination;

        
        a.transform.parent.gameObject.SetActive(false);
        b.transform.parent.gameObject.SetActive(true);
        
    }

    private void Back() {
        DataHandler.GoBackOneFolder();
        StartCoroutine(DataHandler.GetImagesList());
    }
}
