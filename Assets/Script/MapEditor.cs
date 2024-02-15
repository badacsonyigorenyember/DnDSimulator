using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Slider = UnityEngine.UI.Slider;

public class MapEditor : MonoBehaviour
{
    private Slider slider;
    private Button saveButton;
    private TextMeshProUGUI mapNameText;
    private TMP_InputField scaleInput;

    public Map map;
    
}
