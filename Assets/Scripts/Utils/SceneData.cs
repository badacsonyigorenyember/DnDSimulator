using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public class SceneData
    {
        public string name;
        public List<string> creatures;
        public float zoomScale;
        public Vector2 camPosition;

        public SceneData(string name) {
            this.name = name;
            creatures = new List<string>();
        }
    }
}
