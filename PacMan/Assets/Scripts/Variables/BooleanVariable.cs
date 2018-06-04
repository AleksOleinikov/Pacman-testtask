using UnityEngine;
[CreateAssetMenu(menuName = "Variables/BooleanVariable")]
public class BooleanVariable : ScriptableObject
{
    public bool Value;

    public void SetValue(bool boolValue)
    {
        Value = boolValue;
    }

    public void SetValue(BooleanVariable boolVariable)
    {
        Value = boolVariable.Value;
    }
}
