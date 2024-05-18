using System;
using TMPro;
using UnityEngine;

public class SceneListElement : MonoBehaviour
{
    public void SetCreature(string n) {
        name = n ;

        GetComponent<TextMeshProUGUI>().text = n;
        GetComponent<TooltipTrigger>().header = n;
    }

    private void OnMouseDown() {
        SceneHandler.Instance.LoadScene(name);
    }
}
