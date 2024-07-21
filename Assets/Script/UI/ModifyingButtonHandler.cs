using UnityEngine;
using UnityEngine.UI;

public class ModifyingButtonHandler : MonoBehaviour
{
    public ModifyCreatureHandler handler;
    public string variable;

    [SerializeField] private Button _increaseButton;
    [SerializeField] private Button _decreaseButton;

    private void Start() {
        _increaseButton.onClick.AddListener(() => {
            handler.ModifyValue(variable, 1);
        });

        _decreaseButton.onClick.AddListener(() => {
            handler.ModifyValue(variable, -1);
        });
    }
}
