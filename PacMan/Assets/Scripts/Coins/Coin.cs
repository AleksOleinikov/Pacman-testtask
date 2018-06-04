using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private bool energizer;
    [Header("Value Settings")]
    [SerializeField] private FloatReference coinValue;
    [SerializeField] private FloatVariable scoreVariable;
    [Header("Event Settings")]
    [SerializeField] private GameEvent coinCollectedEvent;
    [ConditionalHide("energizer",true)]
    [SerializeField] private GameEvent energizerCollectedEvent;
    [Header("Set Settings")]
    [SerializeField] private CoinsSet activeCoinsSet;
    [SerializeField] private CoinsSet allCoinsSet;

    private void Awake()
    {
        if (allCoinsSet != null) allCoinsSet.Add(this);
        else Debug.LogError("AllCoinsSet is null for " + this.name);
    }

    private void OnEnable()
    {
        if (activeCoinsSet != null) activeCoinsSet.Add(this);
        else Debug.LogError("ActiveCoinsSet is null for " + this.name);
    }

    private void OnDisable()
    {
        if (activeCoinsSet != null) activeCoinsSet.Remove(this);
        else Debug.LogError("ActiveCoinsSet is null for " + this.name);
    }

    private void OnDestroy()
    {
        if (allCoinsSet != null) allCoinsSet.Remove(this);
        else Debug.LogError("AllCoinsSet is null for " + this.name);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag=="Player")
        {
            CollectCoin();
        }
    }

    private void CollectCoin()
    {
        if (scoreVariable != null && coinValue != null) scoreVariable.ApplyChange(coinValue);
        else Debug.LogError("ScoreVariable or/and CoinValue is null for " + this.name);

        if (coinCollectedEvent!=null)coinCollectedEvent.Raise();
        else Debug.LogError("CoinCollectedEvent is null for "+this.name);

        if (energizer)
        {
            if (energizerCollectedEvent != null) energizerCollectedEvent.Raise();
            else Debug.LogError("EnergizerCollectedEvent is null for " + this.name);
        }
        this.gameObject.SetActive(false);
    }
}
