using Hierarchy.Utilities;

namespace Hierarchy.HierarchyTree.Nodes;

public class FileNode:Node
{
    public FileNode(string name) : this(name, default)
    {
        
    }

    public FileNode(string name, FolderNode? parent) : base(name, parent, NodeType.File)
    {
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