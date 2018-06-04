using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Variables/DirectionVariable")]
public class DirectionVariable : ScriptableObject
{
    public Vector2 Value;
    public DirectionVariable OppositeValue;

    public void SetValue(Vector2 value)
    {
        Value = value;
    }

    public void SetValue(DirectionVariable value)
    {
        Value = value.Value;
        if(value.OppositeValue!=null)
        {
            OppositeValue = value.OppositeValue;
        }
    }
}
