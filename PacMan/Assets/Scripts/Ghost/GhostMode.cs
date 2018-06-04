using UnityEngine;

[CreateAssetMenu(menuName = "AI/Mode")]
public class GhostMode : ScriptableObject
{
    public enum Mode { Scatter, Chase, Frightened }

    public Mode ActiveMode;
    public Mode PreviouseMode;
}
