using Hierarchy.Utilities;

namespace Hierarchy.HierarchyTree.Nodes;

public class FileNode:Node
{
    public FileNode(string name) : base(name, default, NodeType.File)
    {
        try
        {
            NodeName.ValidateFileName(name);
        }
        catch (ArgumentException e)
        {
            Console.WriteLine(e);
            throw;
        }
        
        _fileName = name;
    }
    public FileNode(string name, FolderNode? parent) : base(name, parent,NodeType.File)
    {
        try
        {
            NodeName.ValidateFileName(name);
        }
        catch (ArgumentException e)
        {
            Console.WriteLine(e);
            throw;
        }
        
        _fileName = name;
    }

    private string _fileName;
    public string Extension
    {
        get
        {
            return _fileName.Substring(_fileName.IndexOf('.') + 1);
        }
    }

    public int Size { get; set; }
}