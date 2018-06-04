using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour
{
    [Header("Node Settings")]
    [SerializeField] private Node targetNode;
    [SerializeField] private Node currentNode;
    [SerializeField] private Node previouseNode;
    [Header("Directions Settings")]
    [SerializeField] private DirectionVariable startDirection;
    [Tooltip("Направление в котором хотелось бы двигаться")]
    [SerializeField] private DirectionVariable currentWantedDirection;
    [SerializeField] private DirectionVariable lastDirection;
    public List<AvailableNode> availableNodes;
    [Header("Movement Settings")]
    [SerializeField] private FloatReference moveSpeed;

    [HideInInspector] public bool onNode;
    [HideInInspector] public bool goHome;

    #region Privates
    private Vector2 currentMoveDirection;
    private AvailableNode node;
    private Node startNode;
    #endregion

    private void Start()
    {
        startNode = currentNode;
        ResetToDefault();
    }

    // Update is called once per frame
    void Update ()
    {
        UpdateAvailableDirection();
        ChooseDirection();
        StateController();
    }
    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        if (currentMoveDirection != Vector2.zero)
        {
            Vector3 newPos = transform.localPosition + new Vector3(currentMoveDirection.x, 0, currentMoveDirection.y) * moveSpeed.Value * Time.deltaTime;
            if (!OnNode(newPos))
            {
                transform.localPosition =newPos;
            }
            else
            {
                transform.localPosition = targetNode.transform.localPosition;
            }
        }
        else if(this.transform.localPosition!=currentNode.transform.localPosition)
        {
            this.transform.localPosition = currentNode.transform.localPosition;
        }
    }

    /// <summary>
    /// Телепортируем персонажа к противоположному телепорту
    /// </summary>
    private void Teleport()
    {
        if(currentNode.isTeleport)
        {
            UpdateAvailableDirection();
            this.transform.localPosition = currentNode.neighboursPositions[1].transform.localPosition;
            targetNode = currentNode.neighboursPositions[0].GetComponent<Node>();
            previouseNode = currentNode.neighboursPositions[1].GetComponent<Node>();
            currentNode = null;            
        }
    }

    /// <summary>
    /// Обновляем доступные для движения направления
    /// </summary>
    private void UpdateAvailableDirection()
    {
        if (currentNode != null)
        {
            if (currentMoveDirection != currentWantedDirection.Value)
            {
                this.transform.localPosition = currentNode.transform.localPosition;
            }
            availableNodes.Clear();
            if (goHome && currentNode.isHome)
            {
                node = new AvailableNode(currentNode.homeDirection, currentNode.homePosition.GetComponent<Node>());
                availableNodes.Add(node);
            }
            else
            {
                for (int i = 0; i < currentNode.availableDirections.Value.Count; i++)
                {
                    node = new AvailableNode(currentNode.availableDirections.Value[i], currentNode.neighboursPositions[i].GetComponent<Node>());
                    availableNodes.Add(node);
                }                
            }
        }
        else if(currentMoveDirection!=currentWantedDirection.Value || availableNodes.Count>2)
        {
            DirectionVariable oppositeDirection=null;
            for (int i = availableNodes.Count-1; i >=0; i--)
            {
                if(availableNodes[i].node!=targetNode)
                {
                    availableNodes.Remove(availableNodes[i]);
                }
                else
                {
                    oppositeDirection = availableNodes[i].direction.OppositeValue;
                }
            }
            node = new AvailableNode(oppositeDirection, previouseNode);
            availableNodes.Add(node);
        }
    }
    
    /// <summary>
    /// Определяем в какой стадии сейчас находимся
    /// </summary>
    private void StateController()
    {
        if(currentNode!=null && currentMoveDirection!=Vector2.zero) //начинаем двигаться с места
        {
            targetNode = TakeAvailableNode(currentMoveDirection).node;
            previouseNode = currentNode;
            currentNode = null;
            onNode = false;
        }
        else if(previouseNode!=null && targetNode!=null) //перемещаемся между позициями
        {
            if(OnNode(this.transform.localPosition))
            {
                currentNode = targetNode;
                targetNode = null;
                previouseNode = null;
                UpdateAvailableDirection();
                Teleport();
                onNode = true;
            }
        }
    }

    /// <summary>
    /// Определяем какое направление движения сейчас задасть
    /// </summary>
    private void ChooseDirection()
    {
        //проверяем нужно ли менять направление, если да, то можем ли мы на него изменить
        if(currentWantedDirection.Value!=currentMoveDirection && CanChangeDirection(currentWantedDirection.Value)) 
        {
            currentMoveDirection = currentWantedDirection.Value;
            previouseNode = targetNode;
            targetNode = TakeAvailableNode(currentMoveDirection).node;
            if (lastDirection != null)
            {
                lastDirection.Value = currentMoveDirection;
            }
        }
        //провееряем, можно ли продолжать двигаться в прежнем направлении
        if(!CanChangeDirection(currentMoveDirection))
        {
            currentMoveDirection = Vector2.zero;
        }
    }

    /// <summary>
    /// Проверяем возможно ли изменить сейчас направление на то которое мы хотим
    /// </summary>
    private bool CanChangeDirection(Vector2 direction)
    {
        return availableNodes.Exists(x => x.direction.Value == direction);
    }

    /// <summary>
    /// Получаем ноду по направлению 
    /// </summary>
    private AvailableNode TakeAvailableNode(Vector2 direction)
    {
        return availableNodes.Find(x => x.direction.Value == direction);

    }

    /// <summary>
    /// Определяем, пришли мы к Node или еще не дошли
    /// </summary>
    private bool OnNode(Vector3 position)
    {
        if(targetNode!=null && previouseNode!=null)
        {
            float selfToNode = DistanceFromPreviouseNode(position);
            float targetToNode = DistanceFromPreviouseNode(targetNode.transform.localPosition);
            return selfToNode >= targetToNode;
        }
        return false;
    }
    /// <summary>
    /// Определяем дистанцию до предыдущей Node
    /// </summary>
    /// <param name="position"></param>
    private float DistanceFromPreviouseNode(Vector3 position)
    {
        Vector3 vector = position - previouseNode.transform.localPosition;
        return vector.sqrMagnitude;
    }

    public void ResetToDefault()
    {
        currentWantedDirection.Value = startDirection != null ? startDirection.Value : Vector2.zero;
        this.transform.localPosition = startNode.transform.localPosition;
        currentNode = startNode;
        targetNode = null;
        previouseNode = null;
        goHome = false;
        onNode = true;
        if (lastDirection != null)
        {
            lastDirection.Value = new Vector2(-1, 0);
        }
    }
}


