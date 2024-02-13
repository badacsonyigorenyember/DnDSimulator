using System;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EntityScrollListDisplay : MonoBehaviour
{
    private TextMeshProUGUI Text;
    private MonsterData Data;
    private Button DownloadButton;
    private Button InstantiateButton;
    
    private Action DownloadAction;

    public void Init(MonsterData monsterDataDownloaded) {
        Data = monsterDataDownloaded;
        
        Text = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        Text.text = monsterDataDownloaded.Name;
        
        DownloadButton = transform.GetChild(1).GetComponent<Button>();
        DownloadButton.onClick.AddListener(Download);

        InstantiateButton = transform.GetChild(2).GetComponent<Button>();
        InstantiateButton.onClick.AddListener(Instantiate);

        if (DataHandler.MonsterIsOnDisk(Data.Name)) {
            DownloadButton.gameObject.SetActive(false);
            InstantiateButton.gameObject.SetActive(true);
        }
    }

    private void Download() {
        DownloadAction += ChangeButton;
        StartCoroutine(DataHandler.DownloadImage(Data, DownloadAction));
        Data.SaveToDisk();
    }

    private void ChangeButton() {
        DownloadButton.gameObject.SetActive(false);
        InstantiateButton.gameObject.SetActive(true);
    }

    private void Instantiate() {
        Entity e = new Entity(Data);
        GameObject a = new GameObject();
        a.name = Data.Name;
        a.AddComponent<SpriteRenderer>().sprite = e.CreateSprite();
    }
    
    
    
    
    
}
