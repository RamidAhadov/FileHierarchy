namespace Hierarchy.HierarchyTree;

public class FileNode:Node
{
    public FileNode(string name, string path, Node? node) : base(name, path, node,NodeType.File)
    {
        _fileName = name;
    }
    
    public FileNode(string name, string path) : base(name, path, default,NodeType.File)
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
        set
        {
            _fileName = value;
        }
    }

    public int Size { get; set; }
}