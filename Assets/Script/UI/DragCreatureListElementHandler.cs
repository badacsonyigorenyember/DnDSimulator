using System.Collections.Generic;
using Script.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragCreatureListElementHandler : MonoBehaviour
{
    [SerializeField] private Camera _cam;
    [SerializeField] private GraphicRaycaster _gr;

    [SerializeField] private int distance;

    [SerializeField] private GameObject _creaturePrefab;

    private static bool validRelease;

    private GameObject _selectedCreature;
    private GameObject _instantiatedCreature;
    private Vector2 _startPosition;
    private CreatureData _creature;
    private Texture2D _creatureImg;

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            SelectCreature();
        }

        if (Input.GetMouseButton(0)) {
            MoveCreature();
        }

        if (Input.GetMouseButtonUp(0)) {
            ReleaseCreature();
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            ReleaseCreature(false);
        }
    }

    void SelectCreature() {
        PointerEventData data = new PointerEventData(null)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        _gr.Raycast(data, results);

        foreach (var result in results) {
            if (result.gameObject.GetComponent<CreatureListElement>() != null) {
                _selectedCreature = result.gameObject;
                _startPosition = result.screenPosition;
            }
        }
    }

    void MoveCreature() {
        if (_selectedCreature == null) return;

        if (_instantiatedCreature == null) {
            if (!(Vector2.Distance(_startPosition, Input.mousePosition) > distance)) return;

            _instantiatedCreature = Instantiate(_selectedCreature, transform.parent);
            Destroy(_instantiatedCreature.GetComponent<BoxCollider2D>());
            _instantiatedCreature.GetComponent<Image>().raycastTarget = false;
        }

        Vector3 position = _cam.ScreenToWorldPoint(Input.mousePosition);
        position.z = 90;

        _instantiatedCreature.transform.position = position;
    }

    async void ReleaseCreature(bool instantiate = true) {
        if (_selectedCreature == null || _instantiatedCreature == null) return;

        Destroy(_instantiatedCreature);
        _instantiatedCreature = null;

        if (validRelease && instantiate) {
            Vector3 position = _cam.ScreenToWorldPoint(Input.mousePosition);
            position.z = 0;

            await CreatureFileHandler.Instance.SpawnCreature(_selectedCreature.name, position);
        }

        _selectedCreature = null;
    }

    public static void ValidRelease(bool value) {
        validRelease = value;
    }
}
