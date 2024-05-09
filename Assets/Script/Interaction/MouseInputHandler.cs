using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MouseInputHandler : MonoBehaviour
{
    /*public float cellSize = 1.101f;
    public Vector2 offset = new Vector2(-0.449f, 0.3f);
    
    public MouseHandlingState state;
    
    private Vector2 _mouseSnappedPosition;
    private bool _canMoveCreatures;
    private Vector2 _previousMouseSnappedPosition;
    private Vector2 _mousePositionDifference;
    
    public List<GameObject> _creaturesToMove;

    //public GameObject mousecursor;
    //public GameObject selectionPrefab;

    private Camera _cam;
    public static MouseInputHandler Instance { get; private set; }
    public static bool validClick;


    private void Awake() {
        Instance = this;
        state = MouseHandlingState.CharacterMove;
    }

    private void Start() {
        _cam = Camera.main;
        _creaturesToMove = new List<GameObject>();
        validClick = true;
    }

    private void Update() {
        SnapMouse();

        switch (state) {
            case MouseHandlingState.InitiativeSelect:
            case MouseHandlingState.CharacterMove:
                GetCreatureOnClick();
                break;
            case MouseHandlingState.WallEdit:
                GetWallPositionOnClick();
                break;
        }

        //mousecursor.transform.position = _mouseSnappedPosition;

    }

    void SnapMouse() {
        Vector2 mouseWorldPosition = GetMouseWorldPosition();

        float snapSize = 10 / cellSize; 
        
        _mouseSnappedPosition = new Vector2(Mathf.Round(mouseWorldPosition.x * snapSize) / snapSize, Mathf.Round(mouseWorldPosition.y * snapSize) / snapSize);
    }

    

    #region CreatureHandle
    
    void GetCreatureOnClick() {
        if(!validClick) return;
        
        if (Input.GetMouseButtonDown(0)) {
            var hit = Physics2D.Raycast(GetMouseWorldPosition(), Vector2.zero);
            
            if (hit.collider == null) {
                ClearCreaturesToMove();
                return;
            }
            
            GameObject hitObj = hit.collider.gameObject;

            if (hitObj.layer == LayerMask.NameToLayer("Creature")) {
                if (state == MouseHandlingState.InitiativeSelect) {
                    InitiativeHandler.Instance.AddCreature(hitObj.GetComponent<Creature>());
                    state = MouseHandlingState.CharacterMove;
                    return;
                }
                if (Input.GetKey(KeyCode.LeftShift)) {
                    AddCreatureToCreaturesToMove(hitObj);
                }
                else {
                    if (_creaturesToMove.Count > 0) {
                        if (!_creaturesToMove.Contains(hitObj)) {
                            ClearCreaturesToMove();
                            
                        }
                    }
                    
                    AddCreatureToCreaturesToMove(hitObj);
                    _canMoveCreatures = true;
                    _previousMouseSnappedPosition = _mouseSnappedPosition;
                }
            }
            else {
                ClearCreaturesToMove();
                _canMoveCreatures = false;
                CreatureInteractionHandler.Instance.CancelCreaturePanel();

                if (hitObj.layer == LayerMask.NameToLayer("Door")) {
                    hitObj.transform.parent.GetComponent<Door>().OpenCloseClientRpc();
                    FogOfWar.Instance.RefreshClientRpc();

                }
            }
            
        }

        if (_canMoveCreatures && Input.GetMouseButton(0)) {
            MoveCreatures();
        }
        
        if (_canMoveCreatures && Input.GetMouseButtonUp(0)) {
            _canMoveCreatures = false;
        }

        if (Input.GetMouseButtonDown(1)) {
            var hit = Physics2D.Raycast(GetMouseWorldPosition(), Vector2.zero);

            if (hit.collider == null) {
                CreatureInteractionHandler.Instance.CancelCreaturePanel();
                return;
            }

            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Creature")) {
                CreatureInteractionHandler.Instance.OpenCreaturePanel(hit.collider.gameObject.GetComponent<Creature>());
            }
            else {
                CreatureInteractionHandler.Instance.CancelCreaturePanel();
            }
        }
    }

    void AddCreatureToCreaturesToMove(GameObject creature) {
        if (!_creaturesToMove.Contains(creature)) {
            _creaturesToMove.Add(creature);
            //Instantiate(selectionPrefab, creature.transform);
        }
    }

    void ClearCreaturesToMove() {
        foreach (var creature in _creaturesToMove) {
            foreach (Transform child in creature.transform) {
                //if(child.name == selectionPrefab.name + "(Clone)")
                    //Destroy(child.gameObject);
            }
        }
        
        _creaturesToMove.Clear();
    }

    void MoveCreatures() {
        if (!_canMoveCreatures) return;

        _mousePositionDifference = _mouseSnappedPosition - _previousMouseSnappedPosition;

        if (_mousePositionDifference == Vector2.zero) return;
        
        _previousMouseSnappedPosition = _mouseSnappedPosition;


        foreach (var creature in _creaturesToMove) {
            Vector2 creaturePosition = creature.transform.position;
            creaturePosition += _mousePositionDifference;
            creature.transform.position = creaturePosition;
            
        }
        
        //FogOfWar.Instance.RefreshClientRpc();
    }
    
    #endregion

    #region WallHandle

    void GetWallPositionOnClick() {
        if(!validClick) return;
        
        if (Input.GetMouseButtonDown(0)) {
            RaycastHit2D hit = Physics2D.Raycast(GetMouseWorldPosition(), Vector3.zero, 20, LayerMask.GetMask("Door"));

            if (hit.collider == null) {
                WallEditor.Instance.AddWallPosition(_mouseSnappedPosition);
                WallEditor.Instance.ReleaseDoor();
                return;
            }
            
            GameObject hitObj = hit.collider.gameObject;
            
            WallEditor.Instance.SelectDoor(hitObj.transform.parent.GetComponent<Door>());
        }

        if (Input.GetMouseButtonDown(1)) {
            WallEditor.Instance.CancelWall();
            WallEditor.Instance.CancelDoor();
        }
        
        if (Input.GetKeyDown(KeyCode.R)) {
            WallEditor.Instance.RotateDoor();
        }
    }
    
    #endregion

    public Vector2 GetMouseWorldPosition() {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = -10;
        
        return _cam.ScreenToWorldPoint(mousePosition);
    }

    public Vector2 GetMouseSnappedPosition() {
        return _mouseSnappedPosition;
    }
    
    public static void IsValidClick(bool value) {
        validClick = value;
    }

}

public enum MouseHandlingState
{
    CharacterMove,
    WallEdit,
    InitiativeSelect*/
}
