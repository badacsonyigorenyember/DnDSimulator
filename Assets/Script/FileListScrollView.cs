using System;
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

    private static Action<bool> SelectionMode;
    private static bool ShowingOnlyDownloaded;
     
    
    private string searchText = "";
    
    void Start() {
        StartCoroutine(DataHandler.GetImagesList());
        DataHandler.OnDataDownloaded += () => {
            searchText = "";
            SearchInput.text = "";
            FillScrollViewWithData(false);
        };

        SelectionMode += FillScrollViewWithData;

        ShowingOnlyDownloaded = false;
        
        HideButton.onClick.AddListener(HidePanel);
        ShowButton.onClick.AddListener(Showpanel);
        BackButton.onClick.AddListener(Back);
    }
    

    private void Update() {
        if (SearchInput.isFocused && searchText != SearchInput.text) {
            searchText = SearchInput.text;
            FillScrollViewWithData(ShowingOnlyDownloaded);
        }
    }

    void FillScrollViewWithData(bool onlyDownloaded) {
        foreach (var obj in FileObjects) {
            Destroy(obj);
        }
        FileObjects.Clear();

        if (onlyDownloaded) {
            BackButton.transform.parent.gameObject.SetActive(false);
            FillScrollViewWithEntities();
            return;
        }
        
        BackButton.transform.parent.gameObject.SetActive(true);

        foreach (var file in DataHandler.files.Where(f => f.Name.ToLower().Contains(searchText.ToLower())).ToList()) {
            CreateScrollViewObject(file.Name).Init(file);
        }
        
    }

    void FillScrollViewWithEntities() {
        foreach (var entity in DataHandler.GetDownloadedEntities().Where(f => f.Name.ToLower().Contains(searchText.ToLower())).ToList()) {
            CreateScrollViewObject(entity.Name).Init(entity);
        }
        
        
    }

    private FileListScrollDisplay CreateScrollViewObject(string n) {
        GameObject obj = Instantiate(Prefab, Content);
        obj.name = n;
            
        if (obj.TryGetComponent(out RectTransform rect)) {
            rect.anchorMax = new Vector2(1, 1);
            rect.position = new Vector3(0, 0, 0);
        }
        
        FileObjects.Add(obj);

        return obj.GetComponent<FileListScrollDisplay>();
    }

    private void Showpanel() {
        Debug.Log("Show");
        StartCoroutine(Move(190, ShowButton, HideButton));
    }

    private void HidePanel() {
        Debug.Log("Hide");
        StartCoroutine(Move(-540, HideButton, ShowButton));
    }

    private IEnumerator Move(int destination, Button a, Button b) {
        
        //TODO refelible
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

    public static void SelectionModeChange(Image img) {
        ShowingOnlyDownloaded = !ShowingOnlyDownloaded;
        
        var tempColor = img.color;
        tempColor.a = ShowingOnlyDownloaded ? 255 : 0;
        img.color = tempColor; 
        
        SelectionMode.Invoke(ShowingOnlyDownloaded);

    }

}
