namespace Hierarchy.HierarchyTree;

public abstract class Node
{
    private FolderNode _parent;
    public Node(string name, FolderNode? parent, NodeType type)
    {
        Name = name;
        Path = parent == null ? "../" : Utilities.Path.SetPath(parent.Path, parent.Name);
        _parent = parent;
        Type = type;
    }
    public string Name { get; set; }
    public string Path { get; set; }

    public FolderNode? Parent
    {
        get => _parent;
        set
        {
            _parent = value;
            UpdatePath(_parent);
        }
    }
    public NodeType Type { get; set; }

    private void UpdatePath(FolderNode parent)
    {
        Path = parent == null ? "../" : Utilities.Path.SetPath(parent.Path, parent.Name);
    }
}