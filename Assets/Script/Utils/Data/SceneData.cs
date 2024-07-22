using System.Collections.Generic;
using UnityEngine;

namespace Script.Utils.Data
{
    public class SceneData
    {
        public string name;
        public List<CreatureDto> creatures;
        public float zoomScale;
        public Vector2 camPosition;

        public SceneData(string name) {
            this.name = name;
            creatures = new List<CreatureDto>();
        }
    }
}
