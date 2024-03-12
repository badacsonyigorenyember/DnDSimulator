using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    private List<Door> _doors = new();

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
        
        LoadEverythingFromDisk();
        
        FogOfWar.Instance.RefreshClientRpc();
    }

    private void FixedUpdate() {
        MoveDoor();
    }
    
    public void AddWallPosition(Vector2 position) {
        if (!MouseInputHandler.validClick || _currentDoor != null) return;

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

        WriteWallsToDisk();
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
        if(_currentDoor == null) return;
        
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
        _currentDoor.GetComponent<NetworkObject>().Spawn();
        _currentDoor.ChangeDoorHitBox(false);
        _doors.Add(_currentDoor);
    }

    void LoadDoor(Vector2 position, Quaternion rotation, bool isClosed) {
        GameObject obj = Instantiate(doorObj, position, rotation);
        obj.GetComponent<NetworkObject>().Spawn();

        Door door = obj.GetComponent<Door>();
        
        if(!isClosed)
            door.OpenCloseClientRpc();
        
        _doors.Add(door);
    }

    public void CancelDoor() {
        if (_currentDoor == null) return;
        
        _currentDoor.GetComponent<NetworkObject>().Despawn();
        _doors.Remove(_doors.Last());
        
        FogOfWar.Instance.RefreshClientRpc();
    }

    #region FileManagement

    void WriteWallsToDisk() {
        Directory.CreateDirectory(_savePath);
        StreamWriter sw = new StreamWriter(_savePath + "/walls.txt");

        EdgeCollider2D[] walls = wallObj.GetComponents<EdgeCollider2D>();

        for(int i = 1; i < walls.Length; i++) {
            foreach (var position in walls[i].points) {
                sw.Write(position.x + " " + position.y + ";");
            }
            sw.Write("\n");
        }

        sw.Close();
    }

    void WriteDoorsToDisk() {
        Directory.CreateDirectory(_savePath);
        StreamWriter sw = new StreamWriter(_savePath + "/doors.txt");

        foreach (var door in _doors) {
            Vector2 position = door.transform.position;
            sw.Write(position.x + " " + position.y + " " + (door.transform.localRotation.z == 0 ? 0 : 1) + " " + (door._isClosed ? 1 : 0) + "\n");
        }
        
        sw.Close();
    }
    
    void WriteEverythingToDisk() {
        WriteWallsToDisk();
        WriteDoorsToDisk();
    }

    void LoadWallsFromDisk() {
        if (!File.Exists(_savePath + "/walls.txt")) return;
        
        var lines = File.ReadAllLines(_savePath + "/walls.txt");

        foreach (var line in lines) {
            EdgeCollider2D edgeCollider = wallObj.AddComponent<EdgeCollider2D>();
            
            string[] stringParts = line.Split(new[]{";"," "}, StringSplitOptions.RemoveEmptyEntries);
            List<Vector2> positions = new List<Vector2>();

            for (int i = 0; i < stringParts.Length; i += 2) {
                Vector2 point = new Vector2(float.Parse(stringParts[i]), float.Parse(stringParts[i + 1]));
                
                positions.Add(point);
            }
            
            edgeCollider.points = positions.ToArray();
        }
    }

    void LoadDoorsFromDisk() {
        if (!File.Exists(_savePath + "/doors.txt")) return;
        
        var lines = File.ReadAllLines(_savePath + "/doors.txt");

        foreach (var line in lines) {
            string[] stringParts = line.Split(new[]{" "}, StringSplitOptions.RemoveEmptyEntries);

            Vector2 position = new Vector2(float.Parse(stringParts[0]), float.Parse(stringParts[1]));
            Quaternion rotation = Quaternion.Euler(0, 0, stringParts[2] == "0" ? 0 : 90);
            bool isCLosed = stringParts[3] == "1";
            
            LoadDoor(position, rotation, isCLosed);
        }
    }

    void LoadEverythingFromDisk() {
        LoadWallsFromDisk();
        LoadDoorsFromDisk();
    }
    
    private void OnApplicationQuit() {
        WriteEverythingToDisk();
    }

    #endregion
    
    

    


    
}