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
            MoveFolder(folder, newPath,default);
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
            MoveFolder((FolderNode)folder, newPath,destPath);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.InnerException?.Message);
        }
    }

    public void MoveFile(FileNode fileNode, string newPath)
    {
        string sourcePath = Path.MergePaths(Path.RemoveLastSection(_tree.LocalRootPath), Path.SplitPath(fileNode.Path));

        string filePath;
        string newFilePath;
        try
        {
            filePath = Path.MergeFileNameToPath(sourcePath, fileNode.Name);
            newFilePath = Path.MergeFileNameToPath(newPath, fileNode.Name);
        }
        catch (ArgumentException)
        {
            return;
        }

        var treeRelationPath = Path.FindRelation(_tree.LocalRootPath, newPath);
        if (treeRelationPath == null)
        {
            AdjustTree(fileNode,filePath,NodeType.File);
        }

        if (!fileNode.Disposed)
        {
            fileNode.MoveNode(treeRelationPath);
        }

        File.Move(filePath,newFilePath);
    }

    private void MoveFolder(FolderNode folderNode, string newPath, string? destinationPath)
    {
        string sourcePath;
        if (destinationPath == null)
        {
            sourcePath = Path.MergePaths(Path.RemoveLastSection(_tree.LocalRootPath), folderNode.Path,folderNode.Name);
        }
        else
        {
            sourcePath = destinationPath;
        }
        
        newPath = Path.MergePaths(newPath, folderNode.Name);
        var treeRelationPath = Path.FindRelation(_tree.LocalRootPath, newPath);
        if (!_tree.Exists(Path.RemoveLastSection(treeRelationPath)))
        {
            AdjustTree(folderNode, newPath, NodeType.Folder);
        }

        if (!folderNode.Disposed)
        {
            folderNode.MoveNode(treeRelationPath);
        }
        
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

    private void AdjustTree(Node node, string path, NodeType type)
    {
        var info = GetFileSystemInfo(path, type);

        if (!CheckInfoExists(info))
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

    private FileSystemInfo GetFileSystemInfo(string path, NodeType type)
    {
        return type switch
        {
            NodeType.File => new FileInfo(path),
            NodeType.Folder => new DirectoryInfo(Path.RemoveLastSection(path)),
            _ => throw new InvalidOperationException("Unknown NodeType")
        };
    }

    private bool CheckInfoExists(FileSystemInfo info)
    {
        return info.Exists;
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