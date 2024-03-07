namespace Hierarchy.HierarchyTree;

public class FileNode:Node
{
    private string _fileName;
    public FileNode(string name, string path, Node? node) : base(name, path, node)
    {
        _fileName = name;
    }
    
    public FileNode(string name, string path) : base(name, path, default)
    {
        _fileName = name;
    }

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