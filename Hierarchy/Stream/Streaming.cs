using System.Text.RegularExpressions;
using Hierarchy.HierarchyTree;
using Hierarchy.HierarchyTree.Nodes;
using Path = Hierarchy.Utilities.Path;
#pragma warning disable CS8604 // Possible null reference argument.

namespace Hierarchy.Stream;

public partial class Streaming
{
    public Tree ReadDirectory(string path)
    {
        var tree = new Tree(new DirectoryInfo(path).Name);
        if (!Exists(path))
        {
            throw new DirectoryNotFoundException();
        }
        
        GetChildren(tree.GetRoot(),path);

        return tree;
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