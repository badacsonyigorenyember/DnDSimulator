using UnityEngine;

public class CreateButton : MonoBehaviour
{
    public SelectedList selected;

    public void SetType(SelectedList type) {
        selected = type;
    }
}
