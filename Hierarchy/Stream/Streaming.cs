using Hierarchy.Exceptions;
using Hierarchy.HierarchyTree;
using Hierarchy.HierarchyTree.Nodes;
using Path = Hierarchy.Utilities.Path;
#pragma warning disable CS8604 // Possible null reference argument.

namespace Hierarchy.Stream;

public class Streaming
{
    private Tree? _tree;
    public Streaming(string path)
    {
        _tree = new Tree(new DirectoryInfo(path).Name, path);
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
    
    public void MoveDirectory(FolderNode folder, string newPath)
    {
        try
        {
            MoveNode(folder, newPath,default);
        }
        catch (Exception e)
        {
            //TODO - Will write logs in future.
            Console.WriteLine(e.InnerException?.Message);
        }
    }

    public void MoveDirectory(string destPath, string newPath)
    {
        if (!Path.IsFolderPath(destPath))
        {
            throw new ArgumentException($"{destPath} is not correct path.");
        }
        
        var treeRelationPath = Path.FindRelation(_tree.LocalRootPath, destPath);
        if (treeRelationPath == null)
        {
            throw new FolderNotFoundException($"{destPath} not found in current hierarchy");
        }
        
        if (!_tree.Exists(treeRelationPath))
        {
            throw new FolderNotFoundException($"{destPath} not found in current hierarchy");
        }

        var folder = _tree.Find(treeRelationPath);
        if (folder == null && folder.Type == NodeType.Folder)
        {
            throw new FolderNotFoundException("Destination folder not found.");
        }

        try
        {
            MoveNode(folder, newPath,destPath);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.InnerException?.Message);
        }
    }

    public void MoveFile(FileNode fileNode, string newPath)
    {
        MoveNode(fileNode,newPath,default);
    }

    public void MoveFile(string destPath, string newPath)
    {
        if (!Path.IsFolderPath(destPath))
        {
            throw new ArgumentException($"{destPath} is not correct path.");
        }
        
        var treeRelationPath = Path.FindRelation(_tree.LocalRootPath, destPath);
        if (treeRelationPath == null)
        {
            throw new FolderNotFoundException($"{destPath} not found in current hierarchy");
        }
        
        if (!_tree.Exists(treeRelationPath))
        {
            throw new FolderNotFoundException($"{destPath} not found in current hierarchy");
        }

        var movedFile = _tree.Find(treeRelationPath);
        MoveNode(movedFile,newPath,destPath);
    }

    private void MoveNode(Node node, string newPath, string? destinationPath)
    {
        string sourcePath;
        if (destinationPath == null)
        {
            var splitNodePath = Path.SplitPath(node.Path);
            sourcePath = Path.MergePaths(Path.RemoveLastSection(_tree.LocalRootPath), splitNodePath);
            if (node.Type == NodeType.File)
            {
                sourcePath = Path.MergeFileNameToPath(sourcePath, node.Name);
            }
            else
            {
                sourcePath = Path.MergePaths(sourcePath, node.Name);
            }
            
        }
        else
        {
            sourcePath = destinationPath;
        }

        var treeRelationPath = Path.FindRelation(_tree.LocalRootPath, newPath);
        if (treeRelationPath == null || !_tree.Exists(Path.RemoveLastSection(treeRelationPath)))
        {
            AdjustTree(node, newPath, node.Type);
        }

        if (!node.Disposed)
        {
            node.MoveNode(treeRelationPath);
        }

        newPath = Path.MergePaths(newPath, node.Name);
        switch (node.Type)
        {
            case NodeType.File:
                newPath = newPath.TrimEnd('/');
                File.Move(sourcePath, newPath);
                break;
            case NodeType.Folder:
                Directory.Move(sourcePath, newPath);
                break;
        }
    }
    
    private bool Exists(string path)
    {
        if (Directory.Exists(path))
        {
            return true;
        }

        return false;
    }

    private void AdjustTree(Node node, string path, NodeType type)
    {
        var info = GetFileSystemInfo(path);

        if (!info.Exists)
        {
            throw type switch
            {
                NodeType.File => new DirectoryNotFoundException(),
                NodeType.Folder => new FileNotFoundException(),
                _ => throw new InvalidOperationException("Unknown NodeType")
            };
        }

        node.Delete();
    }

    private FileSystemInfo GetFileSystemInfo(string path)
    {
        return new DirectoryInfo(Path.RemoveLastSection(path).TrimEnd('/'));
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