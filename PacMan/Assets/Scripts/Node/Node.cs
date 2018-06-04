using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{    
    [Tooltip("Позиции ближайших узлов")]
    public List<Transform> neighboursPositions;
    public AvailableDirections availableDirections;
    [Header("Neighbours Settings")]
    [SerializeField] private NodeSet nodeSet;

    [Header("Other Settings")]
    public bool isTeleport;
    [Tooltip("Если узел поворота в дом призраков")]
    public bool isHome;
    [ConditionalHide("isHome",true)]
    public Transform homePosition;
    [ConditionalHide("isHome",true)]
    public DirectionVariable homeDirection;


    private void Awake()
    {
        if (!isHome)
        {
            nodeSet.Add(this);
        }
    }

    /// <summary>
    /// Используется для определения позиций соседних нодов (необходимо настроить допустимые направления)
    /// </summary>
    public void DefineNeighbours()
    {
        if (!isTeleport)
        {
            neighboursPositions.Clear();
            foreach (DirectionVariable direction in availableDirections.Value)
            {
                float nearestNodeDistance = 0;
                Transform nearestNode = null;
                foreach (Node item in nodeSet.Items)
                {
                    //Debug.Log(item.name);
                    Vector3 itemLocalPosition = item.transform.localPosition;
                    if (direction.Value.x != 0)
                    {
                        switch ((int)direction.Value.x)
                        {
                            case 1:
                                if (itemLocalPosition.z == this.transform.localPosition.z && itemLocalPosition.x > this.transform.localPosition.x)
                                {
                                    if (nearestNodeDistance == 0 || Vector3.Distance(itemLocalPosition, this.transform.position) < nearestNodeDistance)
                                    {
                                        if (nearestNode != null)
                                        {
                                            neighboursPositions.Remove(nearestNode);
                                        }
                                        nearestNode = item.transform;
                                        neighboursPositions.Add(item.transform);
                                        nearestNodeDistance = Vector3.Distance(itemLocalPosition, this.transform.position);
                                    }
                                }
                                break;
                            case -1:
                                if (itemLocalPosition.z == this.transform.localPosition.z && itemLocalPosition.x < this.transform.localPosition.x)
                                {
                                    if (nearestNodeDistance == 0 || Vector3.Distance(itemLocalPosition, this.transform.position) < nearestNodeDistance)
                                    {
                                        if (nearestNode != null)
                                        {
                                            neighboursPositions.Remove(nearestNode);
                                        }
                                        nearestNode = item.transform;
                                        neighboursPositions.Add(item.transform);
                                        nearestNodeDistance = Vector3.Distance(itemLocalPosition, this.transform.position);
                                    }
                                }
                                break;
                        }
                    }
                    else
                    {
                        switch ((int)direction.Value.y)
                        {
                            case 1:
                                if (itemLocalPosition.x == this.transform.localPosition.x && itemLocalPosition.z > this.transform.localPosition.z)
                                {
                                    if (nearestNodeDistance == 0 || Vector3.Distance(itemLocalPosition, this.transform.position) < nearestNodeDistance)
                                    {
                                        if (nearestNode != null)
                                        {
                                            neighboursPositions.Remove(nearestNode);
                                        }
                                        nearestNode = item.transform;
                                        neighboursPositions.Add(item.transform);
                                        nearestNodeDistance = Vector3.Distance(itemLocalPosition, this.transform.position);
                                    }
                                }
                                break;
                            case -1:
                                if (itemLocalPosition.x == this.transform.localPosition.x && itemLocalPosition.z < this.transform.localPosition.z)
                                {
                                    if (nearestNodeDistance == 0 || Vector3.Distance(itemLocalPosition, this.transform.position) < nearestNodeDistance)
                                    {
                                        if (nearestNode != null)
                                        {
                                            neighboursPositions.Remove(nearestNode);
                                        }
                                        nearestNode = item.transform;
                                        neighboursPositions.Add(item.transform);
                                        nearestNodeDistance = Vector3.Distance(itemLocalPosition, this.transform.position);
                                    }
                                }
                                break;
                        }
                    }

                }
            }
        }
        else
        {
            Debug.Log("Это телепорт, настрой руками, пока что");
        }
    }
    
}
