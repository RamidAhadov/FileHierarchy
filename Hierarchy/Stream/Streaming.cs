using Hierarchy.HierarchyTree;
using Hierarchy.HierarchyTree.Nodes;
using Path = Hierarchy.Utilities.Path;
#pragma warning disable CS8604 // Possible null reference argument.

namespace Hierarchy.Stream;

public class Streaming
{
    private Tree? _tree;
    private string _rootPath;
    public Streaming(string path)
    {
        _tree = new Tree(new DirectoryInfo(path).Name);
        _rootPath = path;
        if (!Exists(path))
        {
            throw new DirectoryNotFoundException();
        }
        
        GetChildren(_tree.GetRoot(),path);
    }
    public Tree GetTree()
    {
        if (_tree == null)
        {
            throw new NullReferenceException();
        }

        return _tree;
    }

    //Add select method to tree
    //Move selected nodes
    public void MoveDirectory(FolderNode node, string newPath)
    {
        if (!_tree.Exists(node))
        {
            throw new FileNotFoundException();
        }

        // if (!Directory.Exists(newPath))
        // {
        //     throw new DirectoryNotFoundException();
        // }

        string sourcePath = Path.MergePaths(_rootPath, node.Path);
        sourcePath = Path.SetPath(sourcePath,node.Name);
        newPath = Path.MergePaths(newPath, node.Name);
        Directory.Move(sourcePath,newPath);
    }

    private bool Exists(string path)
    {
        if (Directory.Exists(path))
        {
            return true;
        }

        return false;
    }

    private void GetChildren(FolderNode folder,string path)
    {
        string[] children = Directory.GetFiles(path);
        foreach (var child in children)
        {
            folder.AddFile(Path.GetFileName(child));
        }

        string[] subDirectories = Directory.GetDirectories(path);
        foreach (var subDirectory in subDirectories)
        {
            var folderName = Path.ReplaceIfExists(Path.GetFileName(subDirectory),':','/');
            folder.AddFolder(folderName);
        }

        foreach (var child in folder.Children)
        {
            if (child is FolderNode folderNode)
            {
                var subPath = subDirectories
                    .FirstOrDefault(d => d.EndsWith(Path.ReplaceIfExists(folderNode.Name,'/',':')));
                GetChildren(folderNode,subPath);
            }
        }
    }
}