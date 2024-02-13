using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FileListScrollDisplay : MonoBehaviour
{
    private TextMeshProUGUI Text;
    private WebFile Data;
    
    private List<Button> Buttons = new List<Button>();

    private Action DownloadAction;

    public void Init(WebFile webFile) {
        Data = webFile;
        
        Text = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        Text.text = webFile.Name;
        
        Button DownloadButton = transform.GetChild(1).GetComponent<Button>();
        DownloadButton.onClick.AddListener(Download);

        Button InstantiateButton = transform.GetChild(2).GetComponent<Button>();
        InstantiateButton.onClick.AddListener(Instantiate);
        
        Button OpenButton = transform.GetChild(3).GetComponent<Button>();
        OpenButton.onClick.AddListener(Open);
        
        Buttons.AddRange( new List<Button> { DownloadButton, InstantiateButton, OpenButton });

        if (webFile.type == "dir") {
            SetActive("Open");
        }
        else {
            SetActive("Download");
        }
    }

    private void SetActive(string n){
        foreach (var button in Buttons) {
            button.gameObject.SetActive(button.name == n);
        }
    }

    private void Download() {
        
    }

    private void Instantiate() {
        
        
        /*Entity e = new Entity(Data);
        GameObject a = new GameObject();
        a.name = Data.Name;
        a.AddComponent<SpriteRenderer>().sprite = e.CreateSprite();
        a.AddComponent<CircleCollider2D>();
        a.layer = LayerMask.NameToLayer("Entity");
        
        e.Obj = a;
        GameManager.Entities.Add(e);*/
    }

    private void Open() {
        DataHandler.currentPath += "/" + Data.Name;
        StartCoroutine(DataHandler.GetImagesList());
    }
    
    
    
    
    
}
