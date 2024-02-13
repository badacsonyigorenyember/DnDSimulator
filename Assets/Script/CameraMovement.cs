using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraMovement : MonoBehaviour
{
    public Vector3 position;
    public float speed;
    public float zoomSpeed;
    public Vector2 zoomScale;
    private Camera cam;
    public bool moving;
    public float entitySpeed;

    public List<GameObject> entitiesToMove;

    private void Start() {
        cam = Camera.main;
        entitiesToMove = new List<GameObject>();
        moving = false;
    }

    void Update() {
        Move(new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0));
        Zoom();
        
        MoveCharacter();
    }


    void Move(Vector3 dir) {
        transform.Translate(dir * speed * Time.deltaTime);
    }

    void Zoom() {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        cam.orthographicSize =
            Mathf.Clamp(cam.orthographicSize - scroll * zoomSpeed, zoomScale.x, zoomScale.y);

    }

    void MoveCharacter() {
        if (Input.GetMouseButtonDown(0)) {
            Vector2 rayOrigin = cam.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.zero);

            if (hit.collider != null) {
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Entity")) {
                    if (Input.GetKey(KeyCode.LeftShift)) {
                        entitiesToMove.Add(hit.collider.gameObject);
                    }
                    else {
                        entitiesToMove.Add(hit.collider.gameObject);
                        moving = true;
                    }
                }
                else {
                    entitiesToMove.Clear();
                }
            }
        }

        if (moving && Input.GetMouseButton(0)) {
            foreach (var entity in entitiesToMove) {
                Vector3 entityPos = entity.transform.position;
                Vector3 dir = (cam.ScreenToWorldPoint(Input.mousePosition) - entityPos).normalized;
                dir.z = 0;
                entity.transform.Translate(dir * entitySpeed * Time.deltaTime);
            }
        }

        if (Input.GetMouseButtonUp(0)) {
            if (moving) {
                entitiesToMove.Clear();
            }
            moving = false;

        }
    }
}
