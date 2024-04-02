namespace Hierarchy.HierarchyTree.Nodes;

public class FileNode(
    string name,
    FolderNode? parent = default,
    string? localPath = default)
    : Node(name,
        parent,
        localPath,
        NodeType.File)
{
    private string _fileName = name;
    public string Extension
    {
        get
        {
            return _fileName.Substring(_fileName.IndexOf('.') + 1);
        }
    }

    public int Size { get; set; }
}