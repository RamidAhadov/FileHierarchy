namespace Hierarchy.HierarchyTree;

public abstract class Node
{
    public Node(string name, string path, Node? node)
    {
        Name = name;
        Path = path;
        Parent = node;
    }
    public string Name { get; set; }
    public string Path { get; set; }
    public Node? Parent { get; set; }
}