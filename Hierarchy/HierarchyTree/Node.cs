namespace Hierarchy.HierarchyTree;

public abstract class Node
{
    public Node(string name, string path, Node? node, NodeType type)
    {
        Name = name;
        Path = path;
        Parent = node;
        Type = type;
    }
    public string Name { get; set; }
    public string Path { get; set; }
    public Node? Parent { get; set; }
    public NodeType Type { get; set; }
}