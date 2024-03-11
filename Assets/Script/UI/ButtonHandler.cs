using UnityEngine;

public class ButtonHandler : MonoBehaviour
{
    public void SetActive(bool value) {
        transform.GetChild(0).gameObject.SetActive(value);
    }
}
