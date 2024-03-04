namespace Hierarchy.HierarchyTree;

public class TreeNode
{
    public string Name { get; set; }
    public string Path { get; set; }
    public List<TreeNode>? Children { get; set; }
    public TreeNode? Parent { get; set; }

    public TreeNode()
    {
        Children = new List<TreeNode>();
    }
}