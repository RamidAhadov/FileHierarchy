using System.Collections;
using Path = Hierarchy.Utilities.Path;

namespace Hierarchy.HierarchyTree;

public class Tree:IEnumerable
{
    private FolderNode _head;

    public Tree(string name)
    {
        _head = new FolderNode(name, "../");
    }

    public Tree(string name, Node parent):this(name)
    {
        _head.Parent = parent;
    }

    public int Count => _head.Count;
    
    public void AddFolder(string folderName)
    {
        if (string.IsNullOrEmpty(folderName))
        {
            throw new NullReferenceException();
        }
        
        _head.Children.Add(new FolderNode(folderName,Path.SetPath(_head.Path,_head.Name),_head));
        _head.Count++;
    }
    
    public void AddFolder(FolderNode folder)
    {
        if (folder == null)
        {
            throw new NullReferenceException();
        }
        
        AddNode(folder);
        
        if (folder.Children == null)
        {
            _head.Count++;
        }
        else
        {
            _head.Count += folder.Children.Count + 1;
        }
    }

    public void AddFile(FileNode fileNode)
    {
        if (fileNode == null)
        {
            throw new NullReferenceException();
        }
        
        AddNode(fileNode);
    }

    public void RemoveFolder(FolderNode folderNode)
    {
        if (folderNode == null)
        {
            throw new NullReferenceException();
        }

        if (!TryRemove(folderNode,_head))
        {
            throw new Exception("Folder cannot be found.");
        }
        
        if (folderNode.Children == null || folderNode.Children.Count == 0)
        {
            _head.Count--;
        }
        else
        {
            _head.Count -= folderNode.Count;
        }
    }

    public void RemoveFile(FileNode fileNode)
    {
        if (fileNode == null)
        {
            throw new NullReferenceException();
        }

        if (!TryRemove(fileNode,_head))
        {
            throw new Exception("File cannot be found.");
        }

        _head.Count--;
    }

    private void AddNode(Node node)
    {
        _head.Children.Add(node);
    }

    private bool TryRemove(Node node,FolderNode startNode)
    {
        if (startNode.Children.Contains(node))
        {
            _head.Children.Remove(node);
            return true;
        }

        foreach (var child in startNode.Children)
        {
            if (child is FolderNode folderNode)
            {
                TryRemove(node,folderNode);
            }
        }

        return false;
    }

    public IEnumerator GetEnumerator()
    {
        throw new NotImplementedException();
    }
}