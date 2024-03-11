using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class InitiativeHandler : NetworkBehaviour
{
    public List<Entity> entities;

    public void RollInitiative() {
        
    }

    int RollRandomNumber(int modifier = 0) {
        return Random.Range(1, 20) + modifier;
    }
}

public class Tree
{
    private Node root;

    public void InsertNode(Entity e) {
        if (root.isLeaf) {
            if (root.entity == null) {
                root.value = e.RollInitiative();
                root.entity = e;
            }
            else {
                root.isLeaf = false;
                InsertNode(root.entity);
            }
        }
    }
    
}

public class Node
{
    public int value;
    public Entity entity;
    public bool isLeaf;
    public List<Node> children;

    public Node() {
        isLeaf = true;
        children = new List<Node>();
    }

    public void InsertIntoNode(Entity e) {
        if (isLeaf) {
            
        }
    }
}

