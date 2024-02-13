using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EntityListScrollView : MonoBehaviour
{
    [SerializeField] private Transform Content;
    [SerializeField] private GameObject Prefab;
    [SerializeField] private TMP_InputField SearchInput;

    [SerializeField] private Button ShowButton;
    [SerializeField] private Button HideButton;

    private List<MonsterData> Monsters = new List<MonsterData>();

    private List<GameObject> MonsterDataObjs = new List<GameObject>();
    
    private string searchText = "";
    
    void Start() {
        DataHandler.OnDataDownloaded += FillListWithData;
        
        HideButton.onClick.AddListener(HidePanel);
        ShowButton.onClick.AddListener(Showpanel);
    }
    

    private void Update() {
        if (SearchInput.isFocused) {
            if (searchText != SearchInput.text) {
                FillScrollViewWithData(Monsters.Where(m => m.Name.ToLower().Contains(SearchInput.text.ToLower())).ToList());
                searchText = SearchInput.text;
            }
        }
    }

    void FillListWithData() {
        Monsters = DataHandler.monsters;
        
        FillScrollViewWithData();
    }

    void FillScrollViewWithData(List<MonsterData> list = null) {
        foreach (var obj in MonsterDataObjs) {
            Destroy(obj);
        }
        MonsterDataObjs.Clear();

        foreach (var monster in list ?? Monsters) {
            GameObject e = Instantiate(Prefab, Content);
            e.name = monster.Name;
            if (e.TryGetComponent(out RectTransform rect)) {
                rect.anchorMax = new Vector2(1, 1);
                rect.position = new Vector3(0, 0, 0);
            }

            

            if (e.TryGetComponent(out EntityScrollListDisplay display)) {
                display.Init(monster);
            }
            
            MonsterDataObjs.Add(e);
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
