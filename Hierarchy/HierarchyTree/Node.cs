namespace Hierarchy.HierarchyTree;

public abstract class Node
{
    public Node(string name, FolderNode? parent, NodeType type)
    {
        Name = name;
        Path = parent == null ? "../" : Utilities.Path.SetPath(parent.Path, parent.Name);
        Parent = parent;
        Type = type;
    }
    public string Name { get; set; }
    public string Path { get; set; }
    public FolderNode? Parent { get; set; }
    public NodeType Type { get; set; }
}