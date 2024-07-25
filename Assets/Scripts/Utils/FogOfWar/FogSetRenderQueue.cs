using UnityEngine;

namespace FogOfWar
{
    public class FogSetRenderQueue : MonoBehaviour
    {
        private void Start() {
            GetComponent<MeshRenderer>().material.renderQueue = 3002;
        }
    }
}
