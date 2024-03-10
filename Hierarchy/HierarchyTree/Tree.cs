using System.Collections;
using Path = Hierarchy.Utilities.Path;

namespace Hierarchy.HierarchyTree;

public class Tree:IEnumerable<Node>
{
    private FolderNode _head;

    public Tree(string name)
    {
        _head = new FolderNode(name);
    }

    public Tree(string name, Node parent):this(name)
    {
        _head.Parent = parent;
    }

    public int Count => _head.Count;

    public FolderNode GetRoot() => _head;

    public IEnumerator<Node> GetEnumerator()
    {
        foreach (var node in GetAllNodes(_head))
        {
            yield return node;
        }
    }

    public void Print()
    {
        var list = GetAllNodes(_head);
        foreach (var node in list)
        {
            Console.WriteLine(node.Name);
            Console.WriteLine(node.Path);
        }
    }

    public Node FindNode(string path)
    {
        var folders = Path.SplitPath(path);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    
    private List<Node> GetAllNodes(FolderNode folderNode)
    {
        var list = new List<Node>();
        foreach (var child in folderNode.Children)
        {
            list.Add(child);
            if (child is FolderNode node)
            {
                list.AddRange(GetAllNodes(node));
            }
        }

        return list;
    }
}