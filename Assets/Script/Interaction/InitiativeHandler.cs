using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class InitiativeHandler : NetworkBehaviour
{
    public Button addEntityButton;
    public Transform uiContainer;
    public GameObject listElementPrefab;

    private List<Entity> _entities = new();
    private Node _root;
    private int _counter; 

    public static InitiativeHandler Instance;
    
    private void Awake() {
        Instance = this;
    }

    private void Start() {
        _root = new Node();
        addEntityButton.onClick.AddListener(() => MouseInputHandler.Instance.state = MouseHandlingState.InitiativeSelect);
    }

    public void AddEntity(Entity entity) {
        if(_entities.Contains(entity)) return;
        
        ClearContainer();
        Insert(entity, _root);
        _entities.Add(entity);

        _counter = 0;
        GetEntitiesList(_root);
    }

    public void RemoveEntityFromList(Entity entity) {
        Node node = GetParentNodeOfEntityToRemove(_root, entity);

        if(node == null) return;

        node.children.RemoveWhere(n => n.entity == entity);
        _entities.Remove(entity);

        if (node.children.Count == 2 && node != _root) {
            node.entity = node.children.First().entity;
            node.children.Clear();
        }

        ClearContainer();
        
        _counter = 0;
        GetEntitiesList(_root);
    }

    void ClearContainer() {
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
                
                Insert(other, existingNode);
                Insert(entity, existingNode);
            }
        }
    }

    void GetEntitiesList(Node root) {
        if (root.entity != null) {
            Instantiate(listElementPrefab, uiContainer).GetComponent<InitiativePanelHelper>().Init(root.entity, ++_counter);
        }

        foreach (var child in root.children) {
            GetEntitiesList(child);
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
}

public class Node
{
    public int value;
    public Entity entity;
    public SortedSet<Node> children = new(new NodeComparer());

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

