using System.Collections.Generic;
using UnityEngine;

namespace Utils.Data
{
    public class SceneData
    {
        public string Name { get; set; }
        public List<string> Creatures { get; set; }
        public List<string> Players { get; set; }
        public float ZoomScale { get; set; }
        public Vector2 CamPosition { get; set; }
    }
}
