using System.Collections;
using Hierarchy.Exceptions;
using Hierarchy.HierarchyTree.Nodes;
using Path = Hierarchy.Utilities.Path;

namespace Hierarchy.HierarchyTree;

public class Tree:IEnumerable<Node>
{
    private FolderNode _head;
    private FolderNode _currentFolder;
    internal readonly string? LocalRootPath;

    public Tree(string name)
    {
        _head = new FolderNode(name);
        _head.IsOpen = true;
        _currentFolder = _head;
    }

    public Tree(string name, string localRootPath)
    {
        //BUG - Add without last section and configure other uses
        //localRootPath = Path.RemoveLastSection(localRootPath);
        _head = new FolderNode(name, default, Path.RemoveLastSection(localRootPath));
        _head.IsOpen = true;
        _currentFolder = _head;
        LocalRootPath = Path.RemoveLastSection(localRootPath);
    }

    public Tree(string name, FolderNode parent):this(name)
    {
        _head.Parent = parent;
    }

    public int Count => _head.Count;

    public FolderNode GetRoot() => _head;

    public IEnumerator<Node> GetEnumerator()
    {
        foreach (var node in GetAllNodes(_head))
        {
            yield return node;
        }
    }

    public IEnumerable<string> Print()
    {
        var list = GetAllNodes(_head);
        foreach (var node in list)
        {
            yield return node.Name;
            yield return node.Path;
        }
    }

    public Node Find(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            throw new ArgumentNullException();
        }

        path = Path.RemoveFirstSection(path);
        var addresses = Path.SplitPath(path);

        return Find(addresses);
    }
    
    public int TotalFolderCount()
    {
        return GetTotalFolderCount(_head);
    }
    
    public int TotalFolderCount(FolderNode folderNode)
    {
        return GetTotalFolderCount(folderNode);
    }

    public int TotalFileCount()
    {
        return GetTotalFileCount(_head);
    }
    
    public int TotalFileCount(FolderNode folderNode)
    {
        return GetTotalFileCount(folderNode);
    }

    internal bool Exists(Node node)
    {
        if (node == null)
        {
            throw new NullReferenceException();
        }

        var tempPath = Path.RemoveFirstSection(node.Path);
        string[] addresses = Path.SplitPath(tempPath);
        var result = Find(addresses);
        if (result == null)
        {
            return false;
        }

        return true;
    }

    internal bool Exists(string path)
    {
        path = Path.RemoveFirstSection(path);
        var addresses = Path.SplitPath(path);
        try
        {
            var result = Find(addresses);
            if (result == null)
            {
                return false;
            }
        }
        catch (Exception)
        {
            return false;
        }

        return true;
    }

    internal void RemoveNode(Node node)
    {
        var tempPath = Path.RemoveFirstSection(node.Path);
        var addresses = Path.SplitPath(tempPath);
        var result = Find(addresses);
        if (result == null)
        {
            throw new FolderNotFoundException($"{node.Name} not found.");
        }

        if (node == _head)
        {
            Clear();
        }
    }

    internal FolderNode OpenFolder(string folderName)
    {
        FolderNode folderNode = null;
        foreach (var child in _currentFolder.Children)
        {
            if (child is FolderNode folder)
            {
                folderNode = folder;
                break;
            }
        }

        if (folderNode == null)
        {
            throw new FolderNotFoundException($"{folderName} not exists in {_currentFolder.Name}.");
        }

        return folderNode.OpenFolder();
    }

    internal FolderNode GetCurrentFolder()
    {
        return _currentFolder;
    }

    internal void AppendTree(Tree tree)
    {
        tree._head.Parent = this._head;
        _head.Children.Add(tree._head);
    }
    
    private FolderNode RecursiveSearch(FolderNode node)
    {
        FolderNode openFolder = null;
        foreach (var nodeChild in node.Children)
        {
            if (nodeChild is FolderNode folderNode)
            {
                openFolder = folderNode;
                break;
            }
        }

        if (openFolder == null)
        {
            return node;
        }

        return RecursiveSearch(openFolder);
    }
    
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private void Clear()
    {
        _head.Children.Clear();
        _head = null;
    }

    private int GetTotalFolderCount(FolderNode folderNode)
    {
        int count = 0;
        GetTotalCount(folderNode, ref count, NodeType.Folder);
        
        return count;
    }
    
    private int GetTotalFileCount(FolderNode folderNode)
    {
        int count = 0;
        GetTotalCount(folderNode, ref count, NodeType.File);
        
        return count;
    }

    private void GetTotalCount(FolderNode folderNode, ref int count, NodeType type)
    {
        foreach (var node in folderNode.Children)
        {
            if (node.Type == type)
            {
                count++;
            }

            if (node is FolderNode folder)
            {
                GetTotalCount(folder, ref count, type);
            }
        }
    }

    private Node Find(string[] addresses)
    {
        if (addresses == null)
        {
            throw new NullReferenceException();
        }

        return RecursiveFind(addresses, _head);
    }

    private Node RecursiveFind(string[] addresses,FolderNode folder)
    {
        if (addresses.Length == 0)
        {
            return folder;
        }

        var nodeName = addresses[0].Replace(":", "/");
        var node = folder.Children.FirstOrDefault(c => c.Name == nodeName);

        if (node == null)
        {
            throw new NullReferenceException("Item cannot be find");
        }

        if (addresses.Length == 1)
        {
            return node;
        }

        if (node is FolderNode folderNode)
        {
            return RecursiveFind(addresses.Skip(1).ToArray(), folderNode);
        }

        return null;
    }
    
    private List<Node> GetAllNodes(FolderNode folderNode)
    {
        var list = new List<Node>();
        foreach (var child in folderNode.Children)
        {
            list.Add(child);
            if (child is FolderNode node)
            {
                list.AddRange(GetAllNodes(node));
            }
        }

        return list;
    }
}