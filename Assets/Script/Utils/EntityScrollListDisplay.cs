using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EntityScrollListDisplay : MonoBehaviour
{
    private TextMeshProUGUI Text;
    private Entity Data;
    private Button DownloadButton;
    private Button InstantiateButton;
    
    private Action DownloadAction;

    public void Init(Entity entity) {
        Data = entity;
        
        Text = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        Text.text = entity.Name;
        
        DownloadButton = transform.GetChild(1).GetComponent<Button>();
        DownloadButton.onClick.AddListener(Download);

        InstantiateButton = transform.GetChild(2).GetComponent<Button>();
    }

    private void Download() {
        DownloadAction += ChangeButton;
        StartCoroutine(Get5etoolsInfos.DownloadImage(Data, DownloadAction));
    }

    private void ChangeButton() {
        DownloadButton.gameObject.SetActive(false);
        InstantiateButton.gameObject.SetActive(true);
    }
    
    
    
    
    
}
