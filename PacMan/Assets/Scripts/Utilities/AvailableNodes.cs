[System.Serializable]
public class AvailableNode
{
    public DirectionVariable direction;
    public Node node;

    public AvailableNode(DirectionVariable direction, Node node)
    {
        this.direction = direction;
        this.node = node;
    }
}
