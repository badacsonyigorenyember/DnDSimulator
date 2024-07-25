using UnityEngine;
using UnityEngine.UI;

namespace Script.UI
{
    public class MapScaler : MonoBehaviour
    {
        [SerializeField] private Slider _scaleSlider;

        private void Start() {
            _scaleSlider.onValueChanged.AddListener(ScaleMap);
        }

        void ScaleMap(float value) {
            transform.localScale = Vector3.one * value;
        }
    }
}
