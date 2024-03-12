using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class InitiativeHandler : NetworkBehaviour
{
    public Button addEntityButton;
    public Button nextEntityTurnButton;
    public Button clearTurnButton;
    public Transform uiContainer;
    public GameObject listElementPrefab;
    public GameObject currentEntityTurnHighLighterPrefab;

    private List<Entity> _entities = new();
    private List<GameObject> _entityPanels = new();
    private Node _root;
    private int _counter;
    private Entity _currentEntityTurn;

    public static InitiativeHandler Instance;
    
    private void Awake() {
        Instance = this;
    }

    private void Start() {
        _root = new Node();
        addEntityButton.onClick.AddListener(() => MouseInputHandler.Instance.state = MouseHandlingState.InitiativeSelect);
        nextEntityTurnButton.onClick.AddListener(SetNextEntityInTurn);
        clearTurnButton.onClick.AddListener(ClearTurn);
        _counter = 0;
    }

    void SetNextEntityInTurn() {
        if (_currentEntityTurn != null) {
            foreach (Transform child in _currentEntityTurn.transform) {
                if(child.name == currentEntityTurnHighLighterPrefab.name + "(Clone)")
                    child.GetComponent<NetworkObject>().Despawn();
            }
        }

        if (_counter + 1 > _entities.Count)
            _counter = 1;
        else
            _counter++;

        GetNextEntityInTurn(_root, _counter);
        if(_currentEntityTurn == null) return;

        GameObject obj = Instantiate(currentEntityTurnHighLighterPrefab);
        NetworkObject netObj = obj.GetComponent<NetworkObject>();
        netObj.Spawn();
        netObj.TrySetParent(_currentEntityTurn.transform, false);
        
        SetInitiativePanelColor();
    }

    void SetInitiativePanelColor() {
        foreach (var entityPanel in _entityPanels) {
            InitiativePanelHelper panelHelper = entityPanel.GetComponent<InitiativePanelHelper>();
            panelHelper.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color =
                panelHelper.place == _counter ? Color.green : Color.black;
        }
    }

    void ClearTurn() {
        _counter = 0;
        _entities.Clear();
        _root = new Node();

        if (_currentEntityTurn != null) {
            foreach (Transform child in _currentEntityTurn.transform) {
                if(child.name == currentEntityTurnHighLighterPrefab.name + "(Clone)")
                    child.GetComponent<NetworkObject>().Despawn();
            }
        }

        _currentEntityTurn = null;
        
        foreach (var entityPanel in _entityPanels) {
            Destroy(entityPanel);
        }
    }

    public void AddEntity(Entity entity) {
        if(_entities.Contains(entity)) return;
        
        ClearContainer();

        Insert(entity, _root);
        _entities.Add(entity);

        int orderNumber = 0;
        GetEntitiesList(_root, ref orderNumber);
    }

    public void RemoveEntityFromList(Entity entity) {
        Node node = GetParentNodeOfEntityToRemove(_root, entity);

        if(node == null) return;
        
        if (_currentEntityTurn == entity) {
            foreach (Transform child in _currentEntityTurn.transform) {
                if(child.name == currentEntityTurnHighLighterPrefab.name + "(Clone)")
                    child.GetComponent<NetworkObject>().Despawn();
            }
        }

        node.children.RemoveWhere(n => n.entity == entity);
        _entities.Remove(entity);

        if (_entities.Count == 0) _counter = 0;

        if (node.children.Count == 2 && node != _root) {
            node.entity = node.children.First().entity;
            node.children.Clear();
        }

        ClearContainer();

        int counter = 0;
        GetEntitiesList(_root, ref counter);
    }

    void ClearContainer() {
        _entityPanels.Clear();
        
        foreach (Transform child in uiContainer) {
            Destroy(child.gameObject);
        }
    }

    int GenerateRandomThrow() {
        return Random.Range(1, 21);
    }
    
    void Insert(Entity entity, Node root) {
        int randomNumber = GenerateRandomThrow();
        Node newNode = new Node(randomNumber + entity.initiativeModifier, entity);

        bool added = root.InsertAsChild(newNode);

        if (!added) {
            Node existingNode = root.children.First(node => node.value == newNode.value);

            if (existingNode.entity == null) {
                Insert(entity, existingNode);
            }
            else {
                Entity other = existingNode.entity;
                existingNode.entity = null;
                existingNode.orderNumber = 0;
                
                Insert(other, existingNode);
                Insert(entity, existingNode);
            }
        }
    }

    void GetEntitiesList(Node root, ref int counter) {
        if (root.entity != null) {
            root.orderNumber = ++counter;
            _entityPanels.Add(Instantiate(listElementPrefab, uiContainer));
            _entityPanels.Last().GetComponent<InitiativePanelHelper>().Init(root.entity, root.orderNumber, root.orderNumber == _counter);
        }

        foreach (var child in root.children) {
            GetEntitiesList(child, ref counter);
        }
    }

    Node GetParentNodeOfEntityToRemove(Node parent, Entity entity) {
        if (parent.entity == entity) return _root;
        
        foreach (var child in parent.children) {
            var res = GetParentNodeOfEntityToRemove(child, entity);
            if (res != null)
                return res;
        }

        return null;
    }
    
    void GetNextEntityInTurn(Node root, int orderNumber) {
        if (root.orderNumber == orderNumber) {
            _currentEntityTurn = root.entity;
        }

        foreach (var child in root.children) {
            GetNextEntityInTurn(child, orderNumber);
        }
    }
}

public class Node
{
    public int value;
    public Entity entity;
    public SortedSet<Node> children = new(new NodeComparer());
    public int orderNumber;

    public Node() {
        value = 0;
    }

    public Node(int value, Entity entity) {
        this.value = value;
        this.entity = entity;
    }

    public bool InsertAsChild(Node other) {
        if (children.All(child => child.value != other.value)) {
            children.Add(other);
            return true;
        }

        return false;
    }
}

public class NodeComparer : IComparer<Node>
{
    public int Compare(Node a, Node b) {
        return b.value.CompareTo(a.value);
    }
}

