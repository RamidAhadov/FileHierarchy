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
    
    public void MoveDirectory(FolderNode folderNode, string newPath)
    {
        if (!_tree.Exists(folderNode))
        {
            throw new FileNotFoundException();
        }

        string sourcePath = Path.MergePaths(_rootPath, folderNode.Path);
        sourcePath = Path.SetPath(sourcePath,folderNode.Name);
        newPath = Path.MergePaths(newPath, folderNode.Name);
        var directoryInfo = new DirectoryInfo(newPath);
        if (!_tree.Exists(newPath))
        {
            if (!directoryInfo.Exists)
            {
                throw new DirectoryNotFoundException();
            }
            
            //_tree.
        }
        folderNode.MoveNode(newPath);
        Directory.Move(sourcePath,newPath);
    }

    public void MoveFile(FileNode fileNode, string newPath)
    {
        if (!_tree.Exists(fileNode))
        {
            throw new FileNotFoundException();
        }

        string sourcePath = Path.MergePaths(_rootPath, fileNode.Path);
        //File.Move();
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