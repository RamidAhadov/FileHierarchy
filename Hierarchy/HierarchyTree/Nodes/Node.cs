namespace Hierarchy.HierarchyTree.Nodes;

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

    public virtual void MoveNode(string newPath)
    {
        var newPathFolders = Utilities.Path.SplitPath(newPath);
        if (!Utilities.Path.IsFolderPath(newPathFolders))
        {
            throw new Exception("Wrong path has entered.");
        }

        var currentPathFolders = Utilities.Path.SplitPath(Path);

        if (!IsUpper(ref newPathFolders,currentPathFolders))
        {
            
        }

    }

    private void UpdatePath(FolderNode parent)
    {
        Path = parent == null ? "../" : Utilities.Path.SetPath(parent.Path, parent.Name);
    }

    private void GoAddress(string[] address)
    {
        
    }

    private bool IsUpper(ref string[] newPathAddress, string[] oldPathAddress)
    {
        int lowerCount = 0;
        for (int i = 0; i < newPathAddress.GetUpperBound(0); i++)
        {
            if (newPathAddress[i] != oldPathAddress[i])
            {
                return true;
            }

            lowerCount++;
        }

        newPathAddress = newPathAddress.Skip(lowerCount).ToArray();

        return false;
    }

    private Node GetNode(string path, FolderNode startNode)
    {
        
    }
}