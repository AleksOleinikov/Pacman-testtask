using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Nodes/AvailableDirections")]
public class AvailableDirections : ScriptableObject
{
    public List<DirectionVariable> Value;

    public void SetValue(AvailableDirections availableDirections)
    {
        Value = availableDirections.Value;
    }
}
