namespace Hierarchy.HierarchyTree;

public class FolderNode:Node
{
    public FolderNode(string name, string path, FolderNode parent):base(name,path,parent)
    {
        Children = new List<Node>();
    }
    public FolderNode(string name, string path):base(name,path,default)
    {
        Children = new List<Node>();
    }
    public List<Node>? Children { get; set; }
}