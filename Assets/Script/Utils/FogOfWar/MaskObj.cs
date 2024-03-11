using System;
using UnityEngine;

public class MaskObj : MonoBehaviour
{
    private void Start() {
        GetComponent<MeshRenderer>().material.renderQueue = 3002;
    }
}
