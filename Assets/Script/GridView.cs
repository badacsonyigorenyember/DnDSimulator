using System;
using Unity.Netcode;
using UnityEngine;

public class GridView : MonoBehaviour
{
    public float gridWidth;
    public float gridHeight;
    public int gridMapSize;
    public Vector2 offset;

    public Grid grid;
    public GameObject obj;
    public GameObject obj2;

    private void Start() {
        
    }

    private void Update() {
        
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            obj2.transform.position = new Vector3(pos.x, pos.y, 0);
            
            var gridposition = grid.WorldToCell(new Vector3Int((int) pos.x, (int) pos.y, (int) pos.z));

            obj.transform.position = grid.CellToWorld(new Vector3Int((int)pos.x, (int)pos.y, 0));
            obj.transform.position += Vector3.one * grid.cellSize.x / 2;
        
    }
}
