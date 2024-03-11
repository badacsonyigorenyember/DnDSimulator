using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class FogOfWar : NetworkBehaviour
{
    public List<Entity> characters = new();

    private Dictionary<int, List<Vector2>> _fowPoints = new();
    
    public static FogOfWar Instance { get; private set; }

    public GameObject mask;
    public GameObject fog;
    
    private void Awake() {
        Instance = this;
    }

    private void Start() {
        string materialName = IsServer ? "serverFogMaterial" : "clientFogMaterial";
        fog.GetComponent<MeshRenderer>().material = Resources.Load<Material>("materials/" + materialName);

        NetworkManager.Singleton.OnClientConnectedCallback += RefreshClientRpc;

    }

    void GetVisiblePoints(Entity ch) {
        int ID = characters.IndexOf(ch);
        float precision = 0.5f;
        float angle = 0;

        _fowPoints[ID] = new List<Vector2> {ch.transform.position};

        while (angle < 360 * 1 / precision) {
            Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            
            RaycastHit2D hit = Physics2D.Raycast(ch.transform.position, dir, Mathf.Infinity, LayerMask.GetMask("Wall"));

            if (hit.collider != null) {
                _fowPoints[ID].Add(hit.point);
            }

            angle += precision;
        }
    }

    void Refresh() {
        foreach (var character in characters) {
            GetVisiblePoints(character);
        }
        
        BuildMesh();
    }

    void BuildMesh() {
        CombineInstance[] instances = new CombineInstance[_fowPoints.Count];
        int counter = 0;

        foreach (var key in _fowPoints) {
            List<Vector3> verticies = new();
            List<int> triangles = new();
            var list = key.Value;
            
            verticies.Add(list[0]);
            
            for (int i = 1; i < list.Count; i++) {
                verticies.Add(list[i]);
                triangles.Add(0);
                triangles.Add(i != list.Count - 1 ? i + 1 : 1);
                triangles.Add(i);
            }
            
            Mesh mesh = new Mesh
            {
                vertices = verticies.ToArray(),
                triangles = triangles.ToArray()
            };

            mesh.RecalculateNormals();

            instances[counter].mesh = mesh;
            instances[counter++].transform = mask.transform.localToWorldMatrix;
        }

        Mesh maskMesh = mask.GetComponent<MeshFilter>().mesh;
        maskMesh.CombineMeshes(instances);
        
        maskMesh.RecalculateNormals();
        
        Player.Instance.visibleBorder = maskMesh.bounds;
        Player.Instance.shouldMove = true;
        
    }

    [ClientRpc]
    public void RefreshClientRpc(ulong id = 0) {
        Refresh();
    }
    
    
    
}

