using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class MouseInputHandler : MonoBehaviour
{
    public float cellSize = 1.101f;
    public Vector2 offset = new Vector2(-0.449f, 0.3f);

    public MouseHandlingState state;
    
    private Vector2 _mouseSnappedPosition;
    private bool _canMoveEntities;
    private Vector2 _previousMouseSnappedPosition;
    private Vector2 _mousePositionDifference;

    private List<GameObject> _entitiesToMove;

    public GameObject mousecursor;
    public GameObject selectionPrefab;

    private Camera _cam;
    public static MouseInputHandler Instance { get; private set; }
    public static bool validClick;


    private void Awake() {
        Instance = this;
        state = MouseHandlingState.CharacterMove;
    }

    private void Start() {
        _cam = Camera.main;
        _entitiesToMove = new List<GameObject>();
        validClick = true;
    }

    private void Update() {
        SnapMouse();

        switch (state) {
            case MouseHandlingState.InitiativeSelect:
            case MouseHandlingState.CharacterMove:
                GetEntityOnClick();
                break;
            case MouseHandlingState.WallEdit:
                GetWallPositionOnClick();
                break;
        }

        mousecursor.transform.position = _mouseSnappedPosition;

    }

    void SnapMouse() {
        Vector2 mouseWorldPosition = GetMouseWorldPosition();

        float snapSize = 10 / cellSize; 
        
        _mouseSnappedPosition = new Vector2(Mathf.Round(mouseWorldPosition.x * snapSize) / snapSize, Mathf.Round(mouseWorldPosition.y * snapSize) / snapSize);
    }

    

    #region EntityHandle
    
    void GetEntityOnClick() {
        if (WallEditor.Instance.drawModeEnabled) return;
        
        if(!validClick) return;
        
        if (Input.GetMouseButtonDown(0)) {
            var hit = Physics2D.Raycast(GetMouseWorldPosition(), Vector2.zero);
            
            if (hit.collider == null) {
                ClearEntitisToMove();
                EntityInteractionHandler.Instance.CancelEntityPanel();
                return;
            }
            
            GameObject hitObj = hit.collider.gameObject;

            if (hitObj.layer == LayerMask.NameToLayer("Entity")) {
                if (state == MouseHandlingState.InitiativeSelect) {
                    InitiativeHandler.Instance.AddEntity(hitObj.GetComponent<Entity>());
                    state = MouseHandlingState.CharacterMove;
                    return;
                }
                if (Input.GetKey(KeyCode.LeftShift)) {
                    AddEntityToEntitiesToMove(hitObj);
                }
                else {
                    if (_entitiesToMove.Count > 0) {
                        if (!_entitiesToMove.Contains(hitObj)) {
                            ClearEntitisToMove();
                            
                        }
                    }
                    
                    AddEntityToEntitiesToMove(hitObj);
                    _canMoveEntities = true;
                    _previousMouseSnappedPosition = _mouseSnappedPosition;
                }
            }
            else {
                ClearEntitisToMove();
                _canMoveEntities = false;
                EntityInteractionHandler.Instance.CancelEntityPanel();

                if (hitObj.layer == LayerMask.NameToLayer("Door")) {
                    hitObj.transform.parent.GetComponent<Door>().OpenCloseClientRpc();
                    FogOfWar.Instance.RefreshClientRpc();

                }
            }
            
        }

        if (_canMoveEntities && Input.GetMouseButton(0)) {
            MoveEntities();
        }
        
        if (_canMoveEntities && Input.GetMouseButtonUp(0)) {
            _canMoveEntities = false;
        }

        if (Input.GetMouseButtonDown(1)) {
            var hit = Physics2D.Raycast(GetMouseWorldPosition(), Vector2.zero);

            if (hit.collider == null) {
                EntityInteractionHandler.Instance.CancelEntityPanel();
                return;
            }

            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Entity")) {
                EntityInteractionHandler.Instance.OpenEntityPanel(hit.collider.gameObject.GetComponent<Entity>());
            }
            else {
                EntityInteractionHandler.Instance.CancelEntityPanel();
            }
        }
    }

    void AddEntityToEntitiesToMove(GameObject entity) {
        if (!_entitiesToMove.Contains(entity)) {
            _entitiesToMove.Add(entity);
            Instantiate(selectionPrefab, entity.transform);
        }
    }

    void ClearEntitisToMove() {
        foreach (var entity in _entitiesToMove) {
            foreach (Transform child in entity.transform) {
                if(child.name == selectionPrefab.name + "(Clone)")
                    Destroy(child.gameObject);
            }
        }
        
        _entitiesToMove.Clear();
    }

    void MoveEntities() {
        if (!_canMoveEntities) return;

        _mousePositionDifference = _mouseSnappedPosition - _previousMouseSnappedPosition;

        if (_mousePositionDifference == Vector2.zero) return;
        
        _previousMouseSnappedPosition = _mouseSnappedPosition;


        foreach (var entity in _entitiesToMove) {
            Vector2 entityPosition = entity.transform.position;
            entityPosition += _mousePositionDifference;
            entity.transform.position = entityPosition;
            
        }
        
        FogOfWar.Instance.RefreshClientRpc();
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
    InitiativeSelect
}
