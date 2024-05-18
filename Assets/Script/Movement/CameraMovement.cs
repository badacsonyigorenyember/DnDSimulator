using System;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private float _movementSpeed;
    [SerializeField] private float _zoomSpeed;
    [SerializeField] private float _minZoomScale;
    [SerializeField] private float _maxZoomScale;

    private Camera _cam;
    
    private void Start() {
        _cam = GetComponent<Camera>();
    }

    private void Update() {
        Vector2  _inputDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        
        transform.Translate(_inputDirection * (_movementSpeed * Time.deltaTime));
        
        float _inputScroll = Input.GetAxis("Mouse ScrollWheel");
        _cam.orthographicSize =
            Mathf.Clamp(_cam.orthographicSize - _inputScroll * _zoomSpeed, _minZoomScale, _maxZoomScale);

    }
}