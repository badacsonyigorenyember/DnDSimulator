using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode()]
public class Tooltip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _headerText;
    [SerializeField] private TextMeshProUGUI _contentText;

    [SerializeField] private LayoutElement _layoutElement;
    [SerializeField] private CanvasGroup gp;

    [SerializeField] private int _characterWrapLimit;

    [SerializeField] private float _fadeDuration;

    public void SetText(string header, string content = "") {
        _headerText.text = header;

        if (!string.IsNullOrEmpty(content)) {
            _contentText.gameObject.SetActive(true);
            _contentText.text = content;
        }
        else {
            _contentText.gameObject.SetActive(false);
        }

        int headerLength = _headerText.text.Length;
        int contentLength = _contentText.text.Length;

        _layoutElement.enabled = headerLength > _characterWrapLimit || contentLength > _characterWrapLimit;
    }

    public void StartFade(bool value) {
        StartCoroutine(Fade(value));
    }

    public IEnumerator Fade(bool value) {
        float elapsedTime = 0f;

        float start = gp.alpha;
        float end = value ? 1 : 0;

        while (elapsedTime < _fadeDuration) {
            elapsedTime += Time.deltaTime;
            gp.alpha = Mathf.Lerp(start, end, elapsedTime / _fadeDuration);
            yield return null;
        }

        gp.alpha = end;
    }
}
