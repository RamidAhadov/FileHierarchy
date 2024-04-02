using Hierarchy.Exceptions;
using Hierarchy.Utilities;

namespace Hierarchy.HierarchyTree.Nodes;

public abstract class Node:IDisposable
{
    //Configure GoUpper and GoLower.
    //Find parent via stream.
    //Move delete via stream
    private FolderNode? _parent;
    private string _name;
    private string _path;
    private string? _localPath;
    public Node(string name, FolderNode? parent, string? localPath,NodeType type)
    {
        _name = name;
        _path = parent == null ? "../" : Utilities.Path.SetPath(parent.Path, parent.Name);
        _parent = parent;
        _localPath = localPath;
        Type = type;
    }

    public string Name
    {
        get
        {
            return _name.Trim();
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
        private set
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

    public string? LocalPath
    {
        get
        {
            return _localPath;
        }
        set
        {
            //TODO - UpdateLocalPath feature.
            _localPath = value;
        }
    }
    public NodeType Type { get; }
    public bool IsSelected { get; set; }
    internal bool Disposed { get; set; }

    internal virtual void MoveNode(string newPath)
    {
        newPath = Utilities.Path.RemoveFirstSection(newPath);
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

    internal void Rename(string newName)
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

    internal void Delete()
    {
        if (_parent != null)
        {
            _parent.Children.Remove(this);
            Dispose();
        }
        else
        {
            throw new DeleteRootException();
        }
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

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        Disposed = true;
    }
}