using Hierarchy.Utilities;

namespace Hierarchy.HierarchyTree.Nodes;

public abstract class Node
{
    //Configure GoUpper and GoLower.
    //Find parent via stream.
    //Move delete via stream
    private FolderNode _parent;
    private string _name;
    private string _path;
    public Node(string name, FolderNode? parent, NodeType type)
    {
        // Bug - Test it
        bool flag = true;
        name = name.Trim();
        try
        {
            if (type == NodeType.File)
            {
                NodeName.ValidateFileName(name);
            }

            if (type == NodeType.Folder)
            {
                NodeName.ValidateFolderName(name);
            }
        }
        catch (ArgumentException e)
        {
            Console.WriteLine(e);
            flag = false;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            flag = false;
        }

        if (flag)
        {
            _name = name;
            _path = parent == null ? "../" : Utilities.Path.SetPath(parent.Path, parent.Name);
            _parent = parent;
            Type = type;
        }
    }

    public string Name
    {
        get
        {
            return _name;
        }
        private set
        {
            _name = value;
            if (this is FolderNode folderNode)
            {
                UpdateChildrenPaths(folderNode);
            }
        }
    }

    public string Path
    {
        get
        {
            return _path;
        }
        set
        {
            _path = value;
            if (this is FolderNode folderNode)
            {
                UpdateChildrenPaths(folderNode);
            }
        }
    }

    public FolderNode? Parent
    {
        get => _parent;
        set
        {
            _parent = value;
            UpdatePath(_parent);
        }
    }
    public NodeType Type { get; }
    //New Feature
    public bool IsSelected { get; set; }

    public virtual void MoveNode(string newPath)
    {
        var newPathFolders = Utilities.Path.SplitPath(newPath);
        if (!Utilities.Path.IsFolderPath(newPathFolders))
        {
            throw new Exception("Wrong path has entered.");
        }

        var root = GetRootNode(this);
        var newParent = (FolderNode)GetNode(newPathFolders, root);
        if (Parent != null)
        {
            Parent.Children.Remove(this);
        }
        
        Parent = newParent;
        Parent.Children.Add(this);
    }

    public void Rename(string newName)
    {
        if (Type == NodeType.File)
        {
            NodeName.ValidateFileName(newName);
        }

        if (Type == NodeType.Folder)
        {
            NodeName.ValidateFolderName(newName);
        }
        
        Name = newName;
    }
    
    private FolderNode GetRootNode(Node startNode)
    {
        if (startNode.Parent == null)
        {
            return (FolderNode)startNode;
        }
        
        return GetRootNode(startNode.Parent);
    }

    private void UpdatePath(FolderNode parent)
    {
        Path = parent == null ? "../" : Utilities.Path.SetPath(parent.Path, parent.Name);
    }

    private void UpdateChildrenPaths(FolderNode node)
    {
        foreach (var child in node.Children)
        {
            child.Path = Utilities.Path.SetPath(node.Path, node.Name);
        }
    }

    private Node GetNode(string[] path, FolderNode startNode)
    {
        if (path.Length == 0)
        {
            return startNode;
        }
        var node = (FolderNode)startNode.Children.FirstOrDefault(n => n.Name == path[0] && n.Type == NodeType.Folder);
        
        return GetNode(path.Skip(1).ToArray(),node);
    }
}