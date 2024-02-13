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
    private string Name;
    
    private List<Button> Buttons = new List<Button>();

    private Action<ScrollButtonState> DownloadAction;
    
    public void Init(WebFile data) {
        Data = data;
        Name = data.Name;
        
        Init();
        
        if (data.type == "dir") {
            SetActive(ScrollButtonState.Open);
        }
        else {
            SetActive(DataHandler.MonsterIsOnDisk(Data.Name)
                ? ScrollButtonState.Instantiate
                : ScrollButtonState.Download);
        }
    }

    public void Init(Entity entity) {
        Name = entity.Name;
        
        Init();
        
        SetActive(ScrollButtonState.Instantiate);
    }

    private void Init() {
        Text = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        Text.text = Name;
        
        Button DownloadButton = transform.GetChild(1).GetComponent<Button>();
        DownloadButton.onClick.AddListener(Download);

        Button InstantiateButton = transform.GetChild(2).GetComponent<Button>();
        InstantiateButton.onClick.AddListener(Instantiate);
        
        Button OpenButton = transform.GetChild(3).GetComponent<Button>();
        OpenButton.onClick.AddListener(Open);
        
        Buttons.AddRange( new List<Button> { DownloadButton, InstantiateButton, OpenButton });

        
    }

    

    private void SetActive(ScrollButtonState state){
        for (int i = 0; i < Buttons.Count; i++) {
            if (i == (int)state) {
                Buttons[i].gameObject.SetActive(true);
            }
            else {
                Buttons[i].gameObject.SetActive(false);
            }
        }
    }

    private void Download() {
        DownloadAction += SetActive;
        StartCoroutine(DataHandler.DownloadImage(Data, DownloadAction));
    }

    private void Instantiate() {
        Entity e = DataHandler.CreateEntity(Name);
        
        GameManager.Entities.Add(e);
    }

    private void Open() {
        DataHandler.currentPath += "/" + Name;
        StartCoroutine(DataHandler.GetImagesList());
    }
    
    
    
    
    
}
