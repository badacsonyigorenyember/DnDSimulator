using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EntityListScrollView : MonoBehaviour
{
    [SerializeField] private Transform Content;
    [SerializeField] private GameObject Prefab;
    [SerializeField] private TMP_InputField SearchInput;

    [SerializeField] private Button ShowButton;
    [SerializeField] private Button HideButton;

    private List<Entity> Entities = new List<Entity>();

    private List<GameObject> EntityObjs = new List<GameObject>();
    
    private string searchText = "";
    
    void Start() {
        Get5etoolsInfos.OnDataDownloaded += FillListWithData;
        
        HideButton.onClick.AddListener(HidePanel);
        ShowButton.onClick.AddListener(Showpanel);
    }

    private void Update() {
        if (SearchInput.isFocused) {
            if (searchText != SearchInput.text) {
                FillScrollViewWithData(Entities.Where(e => e.Name.ToLower().Contains(SearchInput.text.ToLower())).ToList());
                searchText = SearchInput.text;
            }
        }
    }

    void FillListWithData() {
        Entities = Get5etoolsInfos.entites;
        
        FillScrollViewWithData();
    }

    void FillScrollViewWithData(List<Entity> list = null) {
        foreach (var obj in EntityObjs) {
            Destroy(obj);
        }
        EntityObjs.Clear();
        
        foreach (var entity in list ?? Entities) {
            GameObject e = Instantiate(Prefab, Content);
            e.name = entity.Name;
            if (e.TryGetComponent(out RectTransform rect)) {
                rect.anchorMax = new Vector2(1, 1);
                rect.position = new Vector3(0, 0, 0);
            }

            if (e.TryGetComponent(out EntityScrollListDisplay display)) {
                display.Init(entity);
            }
            
            EntityObjs.Add(e);
        }
    }

    public void Showpanel() {
        Debug.Log("Show");
        StartCoroutine(Move(190, ShowButton, HideButton));
    }

    public void HidePanel() {
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

        
        a.gameObject.SetActive(false);
        b.gameObject.SetActive(true);
        
    }
}
