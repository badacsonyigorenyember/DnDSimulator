using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EntityScrollListDisplay : MonoBehaviour
{
    private TextMeshProUGUI Text;
    private MonsterObj Data;
    private Button DownloadButton;
    private Button InstantiateButton;
    
    private System.Action DownloadAction;

    public void Init(MonsterObj monsterObj, bool exists) {
        Data = monsterObj;
        
        Text = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        Text.text = monsterObj.Name;
        
        DownloadButton = transform.GetChild(1).GetComponent<Button>();
        DownloadButton.onClick.AddListener(Download);

        InstantiateButton = transform.GetChild(2).GetComponent<Button>();
        InstantiateButton.onClick.AddListener(Instantiate);

        if (exists) {
            DownloadButton.gameObject.SetActive(false);
            InstantiateButton.gameObject.SetActive(true);
        }
    }

    private void Download() {
        DownloadAction += ChangeButton;
        StartCoroutine(Get5etoolsInfos.DownloadImage(Data, DownloadAction));
    }

    private void ChangeButton() {
        DownloadButton.gameObject.SetActive(false);
        InstantiateButton.gameObject.SetActive(true);
    }

    private void Instantiate() {
    }
    
    
    
    
    
}
