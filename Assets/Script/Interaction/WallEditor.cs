using System;
using System.Collections.Generic;
using System.IO;
using Unity.IO.LowLevel.Unsafe;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Application = UnityEngine.Device.Application;

public class WallEditor : MonoBehaviour
{
    public Button drawModeButton;
    public Button finishButton;
    public Button createDoorButton;
    public bool drawModeEnabled;

    public GameObject wallObj;
    public GameObject doorObj;
    private EdgeCollider2D _currentWall;
    private Door _currentDoor;
    
    private List<Vector2> _wallMakingList = new();
    private List<GameObject> _wallHelperObjectList = new();

    public GameObject wallHelperObject;
    private string _savePath = "/walldata";

    public static WallEditor Instance { get; private set; }

    private void Awake() {
        Instance = this;
        _savePath = Application.dataPath + _savePath;
    }

    private void Start() {
        drawModeButton.onClick.AddListener(() => {
            drawModeEnabled = !drawModeEnabled;
            drawModeButton.GetComponent<ButtonHandler>().SetActive(drawModeEnabled);
            MouseInputHandler.Instance.state =
                drawModeEnabled ? MouseHandlingState.WallEdit : MouseHandlingState.CharacterMove;
            createDoorButton.gameObject.SetActive(drawModeEnabled);
        });
        
        finishButton.onClick.AddListener(FinishWall);
        createDoorButton.onClick.AddListener(CreateDoor);
        
        LoadFromDisk();
        FogOfWar.Instance.RefreshClientRpc();
    }

    private void FixedUpdate() {
        MoveDoor();
    }


    public void AddWallPosition(Vector2 position) {
        if (!MouseInputHandler.validClick) return;

        if (_wallMakingList.Count == 0) {
            finishButton.gameObject.SetActive(true);
            createDoorButton.gameObject.SetActive(false);
            _currentWall = wallObj.AddComponent<EdgeCollider2D>();
        }

        _wallHelperObjectList.Add(Instantiate(wallHelperObject, position, Quaternion.identity));
        _wallMakingList.Add(position);
        _currentWall.points = _wallMakingList.ToArray();
    }

    void FinishWall() {
        _currentWall.points = _wallMakingList.ToArray();
        _currentWall = null;

        WriteToDisk();
        Reset();
        
        FogOfWar.Instance.RefreshClientRpc();
    }

    public void CancelWall() {
        Destroy(_currentWall);
        
        Reset();
        
    }

    void Reset() {
        _wallHelperObjectList.ForEach(Destroy);
        _wallMakingList.Clear();
        finishButton.gameObject.SetActive(false);
        createDoorButton.gameObject.SetActive(true);
    }

    public void SelectDoor(Door door) {
        if (_wallMakingList.Count > 0 || _currentDoor != null) return;
        
        _currentDoor = door;
        _currentDoor.ChangeDoorHitBox(false);
        
        createDoorButton.gameObject.SetActive(false);
    }

    public void ReleaseDoor() {
        _currentDoor.ChangeDoorHitBox(true);
        _currentDoor = null;
        createDoorButton.gameObject.SetActive(true);
        FogOfWar.Instance.RefreshClientRpc();
    }

    void MoveDoor() {
        if (_currentDoor == null) return;

        _currentDoor.transform.position = MouseInputHandler.Instance.GetMouseSnappedPosition();
    }

    public void RotateDoor() {
        if(_currentDoor == null) return;
        
        float rotation = _currentDoor.transform.localRotation.z != 0 ? 0 : 90;
        _currentDoor.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, rotation));
    }

    void CreateDoor() {
        _currentDoor = Instantiate(doorObj, MouseInputHandler.Instance.GetMouseSnappedPosition(), Quaternion.identity).GetComponent<Door>();
        _currentDoor.ChangeDoorHitBox(false);
    }

    public void CancelDoor() {
        if (_currentDoor == null) return;
        
        _currentDoor.GetComponent<NetworkObject>().Despawn();
    }
    
    
    
    

    #region FileManagement

    void WriteToDisk() {
        Directory.CreateDirectory(_savePath);
        StreamWriter sw = new StreamWriter(_savePath + "/wall.txt");

        EdgeCollider2D[] walls = wallObj.GetComponents<EdgeCollider2D>();

        for(int i = 1; i < walls.Length; i++) {
            foreach (var position in walls[i].points) {
                sw.Write(position.x + " " + position.y + ";");
            }
            sw.Write("\n");
        }

        sw.Close();
    }

    void LoadFromDisk() {
        if (!File.Exists(_savePath + "/wall.txt")) return;
        
        var lines = File.ReadAllLines(_savePath + "/wall.txt");

        foreach (var line in lines) {
            EdgeCollider2D collider = wallObj.AddComponent<EdgeCollider2D>();
            
            string[] stringParts = line.Split(new[]{";"," "}, StringSplitOptions.RemoveEmptyEntries);
            List<Vector2> positions = new List<Vector2>();

            for (int i = 0; i < stringParts.Length; i += 2) {
                Vector2 point = new Vector2(float.Parse(stringParts[i]), float.Parse(stringParts[i + 1]));
                
                positions.Add(point);
            }
            
            collider.points = positions.ToArray();
        }
    }
    
    private void OnApplicationQuit() {
        WriteToDisk();
    }

    #endregion
    
    

    


    
}