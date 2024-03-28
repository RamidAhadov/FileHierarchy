using System.Collections;
using Hierarchy.Exceptions;
using Hierarchy.HierarchyTree.Nodes;
using Path = Hierarchy.Utilities.Path;

namespace Hierarchy.HierarchyTree;

public class Tree:IEnumerable<Node>
{
    private FolderNode _head;
    internal string? LocalRootPath;

    public Tree(string name)
    {
        _head = new FolderNode(name);
    }

    public Tree(string name, string realRootPath):this(name)
    {
        LocalRootPath = realRootPath;
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

    public void Print()
    {
        var list = GetAllNodes(_head);
        foreach (var node in list)
        {
            Console.WriteLine(node.Name);
            Console.WriteLine(node.Path);
        }
    }

    public Node Find(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            throw new ArgumentNullException();
        }
        var addresses = Path.SplitPath(path);

        return Find(addresses);
    }

    public int GetTotalFolderCount()
    {
        return GetTotalFolderCount(_head);
    }

    public int GetTotalFileCount()
    {
        return GetTotalFileCount(_head);
    }

    internal bool Exists(Node node)
    {
        if (node == null)
        {
            throw new NullReferenceException();
        }
        
        string[] addresses = Path.SplitPath(node.Path);
        var result = Find(addresses);
        if (result == null)
        {
            return false;
        }

        return true;
    }

    internal bool Exists(string path)
    {
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
        var addresses = Path.SplitPath(node.Path);
        var result = Find(addresses);
        if (result == null)
        {
            throw new FolderNotFoundException($"{node.Name} not found.");
        }

        if (node == _head)
        {
            Clear();
        }
        else
        {
            //node.
        }
    }
    
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    //Should be use cache for rollback delete
    private void Clear()
    {
        try
        {
            _head.Children.Clear();
            _head = null;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
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