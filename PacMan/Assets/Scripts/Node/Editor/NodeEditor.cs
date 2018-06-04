using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Node))]
[CanEditMultipleObjects]
public class NodeEditor : Editor
{

    Node _node;

    private void OnEnable()
    {
        _node = target as Node;
    }
    public override void OnInspectorGUI()
    {
       

        DrawDefaultInspector();

        //if (node.isTeleport)
        //{
        //    node.targetNode = (Node)EditorGUILayout.ObjectField("Next Target Node:", node.targetNode, typeof(Node), true);
        //    node.previouseNode = (Node)EditorGUILayout.ObjectField("Another Teleport Node:", node.previouseNode, typeof(Node), true);
        //}

        if (GUILayout.Button("Define Neighbours"))
        {
            _node.DefineNeighbours();
        }
    }
}
