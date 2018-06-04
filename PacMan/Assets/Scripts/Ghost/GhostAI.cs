using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Mover))]
public class GhostAI : MonoBehaviour
{
    #region Enums
    public enum Ghost { Blinky, Pinky, Inky, Clyde }
    public enum Mode { Chase, Scatter, Frightened }
    #endregion

    [Header("General Settings")]
    [SerializeField] private Ghost ghost;
    [SerializeField] private Transform pacMan;
    [SerializeField] private ColorVariable frightenedColor;
    [SerializeField] private Transform homePosition;
    [Header("Mode Settings")]
    [SerializeField] private Mode activeMode = Mode.Scatter;
    [SerializeField] private Transform scatterModeTarget;
    [SerializeField] private FloatVariable frightenedLength;
    [ConditionalHide("ghost",new int[] {2,3}, true)]
    [SerializeField] private FloatVariable currentLvl;
    [ConditionalHide("ghost",new int[] {2,3}, true)]
    [SerializeField] private CoinsSet activeCoinsSet;
    [ConditionalHide("ghost",new int[] { 1,2 }, true)]
    [SerializeField] private DirectionVariable pacManCurrentDirection;
    [ConditionalHide("ghost",new int[] {2}, true)]
    [SerializeField] private Transform blinkyPosition;
    [Header("Move Settings")]
    [SerializeField] private DirectionVariable currentWantedDirection;
    [SerializeField] private FloatReference normalMoveSpeed;
    [SerializeField] private FloatReference frightenedMoveSpeed;
    [SerializeField] private FloatVariable ghostMoveSpeed;
    [Header("Event Settings")]
    [SerializeField] private GameEvent gotGhostEvent;
    [SerializeField] private GameEvent gotPlayerEvent;

    #region Privates
    private Vector3 targetPosition; //позиция к которой должен прийти призрак
    private List<DirectionVariable> possibleDirectionsList;
    private Material ghostMaterial;
    private Color normalColor;
    private Mode lastMode;

    private float startCoinLimit;

    private bool iAmDead;
    private bool inGhostHouse;
    private bool canStart;

    private Mover _mover;
    #endregion

    private void Awake()
    {
        _mover = this.GetComponent<Mover>();
        possibleDirectionsList = new List<DirectionVariable>();
        ghostMaterial = this.GetComponentInChildren<Renderer>().material;
        normalColor = ghostMaterial.color;        
        if (pacMan==null)
        {
            pacMan = GameObject.FindWithTag("Player").transform;
        }
        ResetToDefault();
    }

    private void Start()
    {
        UpdateTargetPosition();
        FindClosestDirectionToTarget(true);
    }

    private void LateUpdate()
    {
        if (!canStart)
        {
            CanStart();
        }
        else
        {
            if (_mover.onNode)
            {
                if (activeMode == Mode.Chase && !iAmDead)
                    UpdateTargetPosition();
                MakeDecision();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("trigger "+other.name+" "+other.tag+ this.name);
        if (!iAmDead && other.tag=="Player")
        {
            if(activeMode!=Mode.Frightened)
            {
                if (gotPlayerEvent != null)gotPlayerEvent.Raise();
                else Debug.Log("GotPlayerEvent is null for @"+this.name);
            }
            else
            {
                if (gotGhostEvent != null) gotGhostEvent.Raise();
                else Debug.Log("GotGhostEvent is null for @" + this.name);
                GoHome();
            }
        }
        else if(other.tag=="HomeNode")
        {
            if(iAmDead) AtHome();
            else inGhostHouse = false;
        }
    }

    #region Decisions
    /// <summary>
    /// Определяем может ли призрак начать движение
    /// </summary>
    private void CanStart()
    {
        switch(ghost)
        {
            case Ghost.Blinky:
            case Ghost.Pinky:
                canStart = true;
                break;
            case Ghost.Inky:
            case Ghost.Clyde:
                if (activeCoinsSet.Items.Count < startCoinLimit)
                {
                    canStart = true;
                }
                break;
        }
    }

    /// <summary>
    /// Изменяем режим на противоположный
    /// </summary>
    public void ChangeMode()
    {
        if (!iAmDead)
        {
            activeMode = activeMode == Mode.Scatter ? Mode.Chase : Mode.Scatter;
            FindOppositeDirection();
            UpdateTargetPosition();
        }
        else
        {
            lastMode = lastMode == Mode.Scatter ? Mode.Chase : Mode.Scatter;
        }
    }

    /// <summary>
    /// Обновляем целевую позицию в зависимости от режима
    /// </summary>
    private void UpdateTargetPosition()
    {
        switch (activeMode)
        {
            case Mode.Chase:
                switch (ghost)
                {
                    case Ghost.Blinky:
                        targetPosition = pacMan.localPosition;
                        break;
                    case Ghost.Pinky:
                        targetPosition = pacMan.localPosition + new Vector3(pacManCurrentDirection.Value.x * 8, 0, pacManCurrentDirection.Value.y * 8);
                        break;
                    case Ghost.Inky:
                        Vector3 pos1 = pacMan.localPosition + new Vector3(pacManCurrentDirection.Value.x * 4, 0, pacManCurrentDirection.Value.y * 4);
                        targetPosition= blinkyPosition.localPosition + 4 * (pos1 - blinkyPosition.localPosition);
                        break;
                    case Ghost.Clyde:
                        targetPosition = Vector3.Distance(pacMan.localPosition, this.transform.localPosition) > 16? pacMan.localPosition: targetPosition = scatterModeTarget.transform.localPosition;
                        break;
                }
                break;
            case Mode.Scatter:
                targetPosition = scatterModeTarget.localPosition;
                break;
        }
    }

    /// <summary>
    /// Определяем таргетную точку
    /// </summary>
    private void MakeDecision()
    {
        if (activeMode != Mode.Frightened || iAmDead)
        {            
            FindClosestDirectionToTarget();
        }
        else
        {
            FindRandomDirection();
        }
    }
    #endregion

    #region Directions
    /// <summary>
    /// Задаем противоположное направление движения
    /// </summary>
    private void FindOppositeDirection()
    {
        if (CanChangeDirection(currentWantedDirection.OppositeValue.Value))
        {
            currentWantedDirection.SetValue(currentWantedDirection.OppositeValue);
        }
    }
    /// <summary>
    /// Определяем рандомное напраление, не противоположное нынешнему
    /// </summary>
    private void FindRandomDirection()
    {
        if (_mover.availableNodes.Count > 1)
        {
            possibleDirectionsList.Clear();
            for (int i = _mover.availableNodes.Count - 1; i >= 0; i--)
            {
                if (_mover.availableNodes[i].direction != currentWantedDirection.OppositeValue)
                {
                    possibleDirectionsList.Add(_mover.availableNodes[i].direction);
                }
            }
            if (possibleDirectionsList.Count > 0)
            {
                int rnd = UnityEngine.Random.Range(0, possibleDirectionsList.Count);
                currentWantedDirection.SetValue(possibleDirectionsList[rnd]);
            }
        }
        else
        {
            currentWantedDirection.SetValue(_mover.availableNodes[0].direction);
        }
    }

    /// <summary>
    /// Определяем направление которое будет ближе к цели (не противоположное нынешнему)
    /// </summary>
    private void FindClosestDirectionToTarget(bool canOpposite=false)
    {
        possibleDirectionsList.Clear();
        float closestDistance = -1;
        for (int i = _mover.availableNodes.Count - 1; i >= 0; i--)
        {
            float thisDistance = GetDistance(targetPosition, _mover.availableNodes[i].node.transform.localPosition);/*+GetDistance(this.transform.localPosition, availableNodes[i].node.transform.localPosition)*/
            if ((closestDistance < 0 || thisDistance < closestDistance) 
                && (_mover.availableNodes[i].direction != currentWantedDirection.OppositeValue|| canOpposite || currentWantedDirection.Value==Vector2.zero || inGhostHouse))
            {
                closestDistance = thisDistance;
                possibleDirectionsList.Add(_mover.availableNodes[i].direction);
            }
        }
        if (possibleDirectionsList.Count > 0)
        {
            currentWantedDirection.SetValue(possibleDirectionsList[possibleDirectionsList.Count-1]);
        }
    }

    /// <summary>
    /// Проверяем возможно ли изменить сейчас направление на то которое мы хотим
    /// </summary>
    private bool CanChangeDirection(Vector2 direction)
    {
        return _mover.availableNodes.Exists(x => x.direction.Value == direction);
    }

    /// <summary>
    /// Определяем дистанцию между двумя точками
    /// </summary>
    /// <param name="vec1"></param>
    /// <param name="vec2"></param>
    private float GetDistance(Vector3 vec1, Vector3 vec2)
    {
        return Vector3.Distance(vec1, vec2);
    }
    #endregion

    #region Behaviour
    /// <summary>
    /// Начинаем бояться
    /// </summary>
    public void StartFrightened()
    {
        if (!iAmDead)
        {
            lastMode = activeMode == Mode.Frightened ? lastMode: activeMode;
            activeMode = Mode.Frightened;
            ghostMaterial.color = frightenedColor.Value;
            ghostMoveSpeed.SetValue(frightenedMoveSpeed);
            if (canStart)
            {
                FindOppositeDirection();
            }
        }
    }

    /// <summary>
    /// Заканчиваем бояться
    /// </summary>
    public void EndFrightened()
    {
        if (activeMode == Mode.Frightened && !iAmDead)
        {
            ghostMoveSpeed.SetValue(normalMoveSpeed);
            activeMode = lastMode;
            ghostMaterial.color = normalColor;
        }
    }

    /// <summary>
    /// Начинаем идти домой
    /// </summary>
    private void GoHome()
    {
        _mover.goHome = true;
        targetPosition = homePosition.localPosition;
        ghostMoveSpeed.SetValue(normalMoveSpeed.Value*2);
        ghostMaterial.color = Color.white;
        iAmDead = true;
    }

    /// <summary>
    /// Когда зашли домой
    /// </summary>
    private void AtHome()
    {
        _mover.goHome = false;
        iAmDead = false;
        inGhostHouse=true;
        ghostMoveSpeed.SetValue(normalMoveSpeed.Value);
        activeMode= activeMode == Mode.Frightened ? lastMode : activeMode;
        ghostMaterial.color = normalColor;
        UpdateTargetPosition();
    }
    #endregion    

    public void ResetToDefault()
    {
        canStart = false;
        ghostMaterial.color = normalColor;
        ghostMoveSpeed.SetValue(normalMoveSpeed);
        iAmDead = false;
        inGhostHouse = false;
        if (ghost == Ghost.Inky || ghost == Ghost.Clyde)
        {
            startCoinLimit = ghost == Ghost.Inky ? activeCoinsSet.Items.Count * (0.85f + 0.03f*currentLvl.Value) : activeCoinsSet.Items.Count *(0.6875f + 0.0625f*currentLvl.Value);
        }
        if(activeMode!=Mode.Scatter) ChangeMode();
    }
}
