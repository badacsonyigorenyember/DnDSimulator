using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    private Camera _cam;
    [SerializeField] private float _speed = 10;
    private void Awake() {
        _cam = Camera.main;
        Debug.Log(IsHost);
    }

    public override void OnNetworkSpawn() {
        //if (!IsOwner) enabled = false;
        
    }

    private void FixedUpdate() {
        Move(new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0));
        Debug.Log(IsServer);

    }

    void Move(Vector3 dir) {
        transform.Translate(dir * (_speed * Time.deltaTime));
    }
}
