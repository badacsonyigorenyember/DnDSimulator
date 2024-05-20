
using System;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class MoveCreatureHandler : MonoBehaviour
{
    [SerializeField] private Camera _cam;

    [SerializeField] private float movementSpeed;

    private Transform _movingCreature;

    private void Update() {
        if(Input.GetMouseButtonDown(0))
            SelectCreatureToMove();

        if(Input.GetMouseButton(0))
            MoveCreature();

        if (Input.GetMouseButtonUp(0))
            ReleaseCreature();
    }

    void SelectCreatureToMove() {
        if(_movingCreature != null) return;
         
        Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, LayerMask.GetMask("Creature"));
        
        if (hit.collider != null) {
            _movingCreature = hit.collider.transform;
        }
    }

    void MoveCreature() {
        if(_movingCreature == null) return;
        
        var mousePositionInWorldSpace = _cam.ScreenToWorldPoint(Input.mousePosition);
        var direction = mousePositionInWorldSpace - _movingCreature.transform.position;
        direction.z = _movingCreature.position.z;
        
        _movingCreature.Translate(direction * (movementSpeed * Time.deltaTime));
    }

    void ReleaseCreature() {
        _movingCreature = null;
    }
}
