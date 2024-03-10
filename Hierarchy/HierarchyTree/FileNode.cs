namespace Hierarchy.HierarchyTree;

public class FileNode:Node
{
    public FileNode(string name, Node? parent) : base(name, Utilities.Path.SetPath(parent.Path, parent.Name), parent,NodeType.File)
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