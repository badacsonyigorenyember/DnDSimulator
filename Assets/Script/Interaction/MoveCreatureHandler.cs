using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;

public class MoveCreatureHandler : MonoBehaviour
{
    [SerializeField] private Camera _cam;

    [SerializeField] private float movementSpeed;

    private List<Transform> _movingCreatures = new();
    private bool _canMove;
    private Vector2 _previousMousePosition;

    private void Update() {
        if(Input.GetMouseButtonDown(0))
            SelectCreatureToMove();

        if(Input.GetMouseButton(0))
            MoveCreature();

        if (Input.GetMouseButtonUp(0))
            ReleaseCreature();
        
        _previousMousePosition = _cam.ScreenToWorldPoint(Input.mousePosition);
    }

    void SelectCreatureToMove() {
        Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, LayerMask.GetMask("Creature"));

        if (hit.collider == null) {
            DeselectCreatures();
            return;
        } 
        
        Transform creature = hit.collider.transform; 
        
        if (Input.GetKey(KeyCode.LeftShift)) {
            if (!_movingCreatures.Contains(creature)) {
                SelectCreature(creature);
            }
        }
        else {
            if (!_movingCreatures.Contains(creature)) {
                if (_movingCreatures.Count > 0) {
                    DeselectCreatures();
                }
            
                SelectCreature(creature);
            }
            
            _canMove = true;
        }
    }

    void MoveCreature() {
        if(_movingCreatures.Count == 0) return;
        
        Vector2 mousePositionInWorldSpace = _cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 diffVector = mousePositionInWorldSpace - _previousMousePosition;

        foreach (var creature in _movingCreatures) {
            Vector2 creaturePosition = creature.position;
            creaturePosition += diffVector;
            
            creature.position = creaturePosition;
        }
    }

    void ReleaseCreature() {
        if (!_canMove) return;
        
        _canMove = false;
    }

    void SelectCreature(Transform creature) {
        _movingCreatures.Add(creature);

        creature.GetComponent<SpriteRenderer>().color = Color.yellow;
    }

    void DeselectCreatures() {
        foreach (var creature in _movingCreatures) { 
            creature.GetComponent<SpriteRenderer>().color = Color.white;
        }
        
        _movingCreatures.Clear();
    }
}