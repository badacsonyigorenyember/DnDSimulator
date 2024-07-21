using UnityEngine;

public class TooltipSystem : MonoBehaviour
{
    private static TooltipSystem Instance;

    [SerializeField] private Tooltip _tooltip;

    private void Awake() {
        Instance = this;
    }

    public static void Show(string header, string content, Vector2 position) {
        Instance._tooltip.SetText(header, content);

        Instance._tooltip.transform.position = position;

        Instance._tooltip.StartFade(true);
    }

    public static void Hide() {
        Instance._tooltip.StartFade(false);
    }
}
