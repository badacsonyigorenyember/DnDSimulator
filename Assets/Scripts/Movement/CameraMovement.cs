using Unity.Netcode;
using UnityEngine;

namespace Movement
{
    public class CameraMovement : NetworkBehaviour
    {
        [SerializeField] private float _movementSpeed;
        [SerializeField] private float _zoomSpeed;
        [SerializeField] private float _minZoomScale;
        [SerializeField] private float _maxZoomScale;

        private Camera _cam;

        public override void OnNetworkSpawn() {
            base.OnNetworkSpawn();
            _cam = GetComponent<Camera>();
            
            if (_minZoomScale == 0)
                _minZoomScale = 0.1f;
        }

        private void Update() {
            Vector2 _inputDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            float _inputScroll = Input.GetAxis("Mouse ScrollWheel");

            transform.Translate(_inputDirection * (_movementSpeed * Time.deltaTime));
            _cam.orthographicSize =
                Mathf.Clamp(_cam.orthographicSize - _inputScroll * _zoomSpeed, _minZoomScale, _maxZoomScale);

            if (IsServer) {
                MoveCameraClientRpc(_cam.transform.position, _cam.orthographicSize);
            }
        }

        [ClientRpc]
        void MoveCameraClientRpc(Vector3 position, float scroll) {
            _cam.transform.position = position;
            _cam.orthographicSize = scroll;
        }
    }
}
