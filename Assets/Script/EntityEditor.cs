using System;
using System.IO.MemoryMappedFiles;
using Unity.VisualScripting;
using UnityEngine;

public class EntityEditor : MonoBehaviour
{
    [SerializeField] private GameObject MapEditor;
    [SerializeField] private GameObject CreatureEditor;

    private Instantiatable entity;

    private void Start() {
        
    }

    private void Update() {
        InputCheck();
    }

    private void InputCheck() {
        if (Input.GetMouseButtonDown(1)) {
            Debug.Log("pressed");
            Vector2 rayOrigin = Camera.main!.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.zero);

            if (hit.collider == null) return;

            Debug.Log("hit");
            
            if (hit.collider.gameObject.layer != LayerMask.NameToLayer("Entity")) return;
            
            Debug.Log("ENTITYYYYY");
            
            if (hit.collider.gameObject.TryGetComponent(out Instantiatable m)) {
                CreatureEditor.gameObject.SetActive(true);
                entity = m;
                
                Debug.Log(m.GetType());
                return;
            }
            
            
                
                
        }
    }

    public void Close() {
        CreatureEditor.gameObject.SetActive(false);
        MapEditor.gameObject.SetActive(false);
    }
}