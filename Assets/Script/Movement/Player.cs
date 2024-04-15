using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : NetworkBehaviour
{
    [SerializeField] private float speed = 10;
    [SerializeField] private float zoomSpeed = 10;
    [SerializeField] private float minZoomScale;
    [SerializeField] private float maxZoomScale;
    [SerializeField] private float clientLerpTime;

    private float clientLerpElapsedTime;
    private Vector3 _inputDirection;
    private float currentOrthographicSize;
    private Vector3 currentCamPosition; 

    public Bounds visibleBorder;
    public bool shouldMove;

    public Camera cam;

    public static Player Instance { get; private set; }

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        currentCamPosition = cam.transform.position;
    }

    private bool canMoveEntities;

    private void Update() {
        _inputDirection = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
        if (IsServer) {
            serverMove();
            Zoom();
        }
        else {
            clientMove();
        }
    }

    void serverMove() {
        transform.Translate(_inputDirection * (speed * Time.deltaTime));

        if (Input.GetKey(KeyCode.LeftShift)) {
            SendPosClientRpc(transform.position, cam.orthographicSize);
        }
    }

    [ClientRpc]
    void SendPosClientRpc(Vector2 position, float ortographicScale) {
        if (IsServer) return;

        transform.position = Vector3.MoveTowards(transform.position, position, Time.deltaTime * 50);
        transform.position = new Vector3(transform.position.x, transform.position.y, -10);

        cam.orthographicSize = ortographicScale;

    }

    void clientMove() {
    }

    void Zoom() {
        if(!MouseInputHandler.validClick) return;
        
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        cam.orthographicSize =
            Mathf.Clamp(cam.orthographicSize - scroll * zoomSpeed, minZoomScale, maxZoomScale);

    }
}
