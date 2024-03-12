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
    }

    private void FixedUpdate() {
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
    }

    void clientMove() {
        if(!shouldMove) return;
        
        clientLerpElapsedTime += Time.deltaTime;
        float t = Mathf.Clamp01(clientLerpElapsedTime / clientLerpTime);

        if (t >= 1) {
            currentOrthographicSize = cam.orthographicSize;
            clientLerpElapsedTime = 0;
            currentCamPosition = cam.transform.position;
            shouldMove = false;
        }

        float width = visibleBorder.size.x;
        float height = visibleBorder.size.y;

        float aspectRatio = cam.aspect;

        float desiredCameraSize = width / aspectRatio >= height ? width / (2 * aspectRatio) : height / 2;
        desiredCameraSize = Mathf.Max(desiredCameraSize, 5);
        
        Vector3 desiredCameraosition = visibleBorder.center + new Vector3(0, 0, -10);

        cam.orthographicSize = Mathf.Lerp(currentOrthographicSize, desiredCameraSize, t);
        cam.transform.position = Vector3.Lerp(currentCamPosition, desiredCameraosition, t);
    }

    void Zoom() {
        if(!MouseInputHandler.validClick) return;
        
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        cam.orthographicSize =
            Mathf.Clamp(cam.orthographicSize - scroll * zoomSpeed, minZoomScale, maxZoomScale);

    }
}
