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
    public List<Node>? Children { get; set; }

    public int Count { get; set; }
}