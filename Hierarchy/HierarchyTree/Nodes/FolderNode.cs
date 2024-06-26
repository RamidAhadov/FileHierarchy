using Hierarchy.Utilities;

namespace Hierarchy.HierarchyTree.Nodes;

public class FolderNode:Node
{
    public FolderNode(string name, FolderNode? parent = default, string? localPath = default) 
        : base(name.Replace(':', '/'), parent, localPath, NodeType.Folder)
    {
        Children = new List<Node>();
    }
    
    public List<Node> Children { get; }

    public int Count
    {
        get
        {
            return GetAllCount();
        }
    }

    public bool IsOpen { get; set; }

    internal void AddFolder(FolderNode node)
    {
        if (node == null)
        {
            throw new NullReferenceException();
        }
        
        try
        {
            AddChild(node);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
    
    internal void AddFolder(string folderName)
    {
        var folder = new FolderNode(folderName, this);
        
        try
        {
            AddChild(folder);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    internal void AddFolder(string folderName, string localPath)
    {
        var folder = new FolderNode(folderName, this, localPath);

        try
        {
            AddChild(folder);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    internal void AddFile(string fileName)
    {
        var file = new FileNode(fileName, this);
        
        try
        {
            AddChild(file);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    internal void AddFile(string fileName, string localPath)
    {
        var file = new FileNode(fileName, this, localPath);

        try
        {
            AddChild(file);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
    
    internal void AddFile(FileNode node)
    {
        if (node == null)
        {
            throw new NullReferenceException();
        }

        try
        {
            AddChild(node);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    internal void RemoveFolder(FolderNode node)
    {
        if (node == null)
        {
            throw new NullReferenceException();
        }
        
        RemoveChild(node);
    }
    
    internal void RemoveFile(FileNode node)
    {
        if (node == null)
        {
            throw new NullReferenceException();
        }
        
        RemoveChild(node);
    }

    internal FolderNode OpenFolder()
    {
        IsOpen = true;
        return this;
    }

    private void AddChild(Node addedNode)
    {
        if (Exists(addedNode))
        {
            var nodeType = addedNode.Type == NodeType.File ? "file" : "folder";
            throw new Exception($"{addedNode.Name} {nodeType} already exists in this directory.");
        }

        switch (addedNode.Type)
        {
            case NodeType.File:
                NodeName.ValidateFileName(addedNode.Name);
                break;
            case NodeType.Folder:
                NodeName.ValidateFolderName(addedNode.Name);
                break;
        }
        
        Children.Add(addedNode);
        if (Parent == null)
        {
            addedNode.Parent = this;
        }
    }

    private void RemoveChild(Node removedNode)
    {
        if (removedNode == null)
        {
            throw new NullReferenceException();
        }

        if (!Exists(removedNode))
        {
            throw new Exception($"{removedNode.Name} cannot be found.");
        }

        Children.Remove(removedNode);
    }

    private bool Exists(Node node)
    {
        var matchingChild = Children.FirstOrDefault(child => child.Name == node.Name);
        if (matchingChild != null)
        {
            return true;
        }

        return false;
    }

    private int GetAllCount()
    {
        var count = Children.Count;
        if (count == 0)
        {
            return count;
        }
        
        foreach (var child in Children)
        {
            if (child is FolderNode node)
            {
                count += node.Count;
            }
        }

        return count;
    }
}