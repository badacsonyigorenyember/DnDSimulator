using System;
using System.Collections.Generic;
using System.Linq;
using FileHandling;
using Models;
using Network;
using Structs;
using UI.MainPanel.ListElements;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utils;
using Utils.Data;

namespace UI
{
    public class DragCreatureListElementHandler : MonoBehaviour
    {
        [SerializeField] private Camera _cam;
        [SerializeField] private GraphicRaycaster _gr;

        [SerializeField] private int distance;

        [SerializeField] private ScrollRect _listScrollView;

        private static bool validRelease;

        public GameObject _selectedCreature;
        public GameObject _instantiatedCreature;
        private Vector2 _startPosition;
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
            Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, LayerMask.GetMask("MonsterListElement")); //TODO megcsinálni jobbra

            if (hit.collider == null) {
                return;
            }
            
            _selectedCreature = hit.collider.gameObject;
            _startPosition = _cam.WorldToScreenPoint(hit.point);
        }

        void MoveCreature() {
            if (_selectedCreature == null) return;

            if (_instantiatedCreature == null) {
                if (!(Vector2.Distance(_startPosition, Input.mousePosition) > distance)) return;

                _listScrollView.enabled = false;

                _instantiatedCreature = Instantiate(_selectedCreature, _gr.transform);
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

                Creature creature = new Creature(_selectedCreature.GetComponent<MonsterListElement>().uuid)
                {
                 Position = new Position(position)
                };

                await SceneHandler.Instance.SpawnCreature(creature);

                Debug.Log(GameManager.Instance.entities.Count);

            }

            _selectedCreature = null;
            _listScrollView.enabled = true;
        }

        public static void ValidRelease(bool value) {
            validRelease = value;
        }
    }
}
