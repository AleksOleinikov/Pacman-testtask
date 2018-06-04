using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Variables/FloatVariable")]
public class FloatVariable : ScriptableObject
{
    public float Value;
    
    public void SetValue(float floatValue)
    {
        Value = floatValue;
    }

    public void SetValue(FloatVariable floatVariable)
    {
        Value = floatVariable.Value;
    }

    public void ApplyChange(float floatValue)
    {
        Value += floatValue;
    }

    public void ApplyChange(FloatVariable floatVariable)
    {
        Value += floatVariable.Value;
    }
}
