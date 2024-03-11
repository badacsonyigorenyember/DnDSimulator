using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CameraMovement : NetworkBehaviour
{
    public Vector3 position;
    public float speed;
    public float zoomSpeed;
    public Vector2 zoomScale;
    private Camera cam;
    public bool moving;
    public float entitySpeed;
    public static bool canZoom;

    public List<GameObject> entitiesToMove;

    private void Start() {
        cam = Camera.main;
        entitiesToMove = new List<GameObject>();
        moving = false;
        canZoom = true;
        GameObject.Find("fow").GetComponent<MeshRenderer>().enabled = true;
        
        
        
    }

    public override void OnNetworkSpawn() {
        if (!IsOwner) {
            Destroy(this);
        }
    }

    void Update() {
        Move(new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0));
        if (canZoom) Zoom();
        
    }

    public static void CanZoom() {
        canZoom = !canZoom;
    }


    void Move(Vector3 dir) {
        transform.Translate(dir * (speed * Time.deltaTime));
    }

    void Zoom() {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        cam.orthographicSize =
            Mathf.Clamp(cam.orthographicSize - scroll * zoomSpeed, zoomScale.x, zoomScale.y);

    }

    
}
