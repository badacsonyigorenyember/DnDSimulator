using UnityEngine;

namespace Structs
{
    public struct Position
    {
        public float x;
        public float y;

        public Position(Vector2 position) {
            this.x = position.x;
            this.y = position.y;
        }

        public Vector2 GetPosition() {
            return new Vector2(x, y);
        }
    }
}