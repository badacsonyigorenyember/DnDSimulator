using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string header;
    public string content;

    private Coroutine _delay;

    public void Init(string header, string content = "") {
        this.header = header;
        this.content = content;
    }

    public void OnPointerEnter(PointerEventData eventData) {
        _delay = StartCoroutine(Call());
    }

    public void OnPointerExit(PointerEventData eventData) {
        StopCoroutine(_delay);
        TooltipSystem.Hide();
    }

    IEnumerator Call() {
        yield return new WaitForSeconds(0.75f);

        TooltipSystem.Show(header, content, transform.position);
    }
}
