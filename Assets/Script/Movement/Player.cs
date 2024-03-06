using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : NetworkBehaviour
{
    [SerializeField] private float speed = 10;
    [SerializeField] private float entitySpeed = 200;

    private Vector3 inputDirection;
    [SerializeField] private Camera cam;

    private List<NetworkObject> entitiesToMove = new();

    private bool canMoveEntities;

    public override void OnNetworkSpawn() {
        if (!IsOwner) {
            gameObject.SetActive(false);
            return;
        }
        cam.gameObject.SetActive(true);
    }

    private void Update() {
        if (!IsOwner) return;
        
        inputDirection = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
        MoveCharacter();
    }

    private void FixedUpdate() {
        if (!IsOwner) return;
        
        Move();
    }
    
    void Move() {
        transform.Translate(inputDirection * (speed * Time.deltaTime));
    }
    
    void MoveCharacter() {
        if (Input.GetMouseButtonDown(0)) {
            Vector2 rayOrigin = cam.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.zero);

            if (hit.collider != null) {
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Entity")) {
                    if (Input.GetKey(KeyCode.LeftShift)) {
                        entitiesToMove.Add(hit.collider.gameObject.GetComponent<NetworkObject>());
                    }
                    else {
                        entitiesToMove.Add(hit.collider.gameObject.GetComponent<NetworkObject>());
                        canMoveEntities = true;
                    }
                }
                else {
                    entitiesToMove.Clear();
                }
            }
        }

        if (canMoveEntities && Input.GetMouseButton(0)) {
            foreach (var entity in entitiesToMove) {
                Vector3 entityPos = entity.transform.position;
                Vector3 dir = (cam.ScreenToWorldPoint(Input.mousePosition) - entityPos).normalized;
                dir.z = 0;
                entity.transform.Translate(dir * (entitySpeed * Time.deltaTime));
                if(entity.TryGetComponent(out Character ch)) {
                    ch.Moving();
                }
            }
        }

        if (Input.GetMouseButtonUp(0)) {
            if (canMoveEntities) {
                entitiesToMove.Clear();
            }
            
            canMoveEntities = false;

        }
    }
    
    
}
