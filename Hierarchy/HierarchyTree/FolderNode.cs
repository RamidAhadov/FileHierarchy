namespace Hierarchy.HierarchyTree;

public class FolderNode:Node
{
    public FolderNode(string name, string path, Node? parent) : base(name, path, parent, NodeType.Folder)
    {
        Children = new List<Node>();
    }
    public FolderNode(string name, string path):base(name,path,default,NodeType.Folder)
    {
        Children = new List<Node>();
    }
    public List<Node> Children { get; set; }

    public int Count
    {
        get
        {
            return GetAllCount();
        }
    }

    public void AddFolder(FolderNode node)
    {
        if (node == null)
        {
            throw new NullReferenceException();
        }
        
        AddChild(node);
    }
    
    public void AddFolder(string folderName)
    {
        if (string.IsNullOrEmpty(folderName))
        {
            throw new ArgumentNullException();
        }

        var folder = new FolderNode(folderName, Path, this);
        
        AddChild(folder);
    }

    public void AddFile(FileNode node)
    {
        if (node == null)
        {
            throw new NullReferenceException();
        }
        
        AddChild(node);
    }

    public void RemoveFolder(FolderNode node)
    {
        if (node == null)
        {
            throw new NullReferenceException();
        }
        
        RemoveChild(node);
    }
    
    public void RemoveFile(FileNode node)
    {
        if (node == null)
        {
            throw new NullReferenceException();
        }
        
        RemoveChild(node);
    }

    private void AddChild(Node addedNode)
    {
        if (Exists(addedNode))
        {
            throw new Exception($"{addedNode.Name} file already exists in this directory.");
        }
        
        Children.Add(addedNode);
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