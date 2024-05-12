using System;
using UnityEngine;
using UnityEngine.UI;


public class CreateButton : MonoBehaviour
{
    public SelectedList selected;

    public void SetType(SelectedList type) {
        selected = type;
    }
}
