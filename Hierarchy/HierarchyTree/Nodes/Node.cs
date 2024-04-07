using Hierarchy.Exceptions;
using Hierarchy.Utilities;

namespace Hierarchy.HierarchyTree.Nodes;

public abstract class Node:IDisposable
{
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
            UpdateChildrenPaths();
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
            UpdateChildrenPaths();
            UpdateChildrenLocalPaths();
        }
    }

    public FolderNode? Parent
    {
        get => _parent;
        set
        {
            _parent = value;
            UpdatePath();
            UpdateLocalPath();
        }
    }

    public string? LocalPath
    {
        get
        {
            return _localPath;
        }
        private set
        {
            _localPath = value;
        }
    }

    public NodeType Type { get; }
    public bool IsSelected { get; set; }
    
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        Disposed = true;
    }
    
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

        newName = newName.Replace('/', ':');
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

    private void UpdatePath()
    {
        Path = _parent == null ? "../" : Utilities.Path.SetPath(_parent.Path, _parent.Name);
    }

    private void UpdateChildrenPaths()
    {
        if (this is FolderNode folderNode)
        {
            foreach (var child in folderNode.Children)
            {
                child.Path = Utilities.Path.SetPath(folderNode.Path, folderNode.Name);
            }
        }
    }

    private Node? GetNode(string[] path, FolderNode? startNode)
    {
        if (path.Length == 0)
        {
            return startNode;
        }
        
        var node = (FolderNode)startNode.Children.FirstOrDefault(n => 
            n.Name == path[0] && n.Type == NodeType.Folder);
        
        return GetNode(path.Skip(1).ToArray(),node);
    }

    private void UpdateLocalPath()
    {
        LocalPath = _parent == null ? _localPath : Utilities.Path.SetPath(_parent.LocalPath, _parent.Name);
    }
    
    private void UpdateChildrenLocalPaths()
    {
        if (this is FolderNode folderNode)
        {
            foreach (var child in folderNode.Children)
            {
                child.LocalPath = _localPath + folderNode.Name + "/";
            }
        }
    }
}