using Hierarchy.Exceptions;
using Hierarchy.HierarchyTree;
using Hierarchy.HierarchyTree.Nodes;
using Path = Hierarchy.Utilities.Path;
#pragma warning disable CS8604 // Possible null reference argument.

namespace Hierarchy.Stream;

public class Streaming
{
    private readonly Tree? _tree;
    public Streaming(string path)
    {
        _tree = new Tree(new DirectoryInfo(path).Name, path);
        if (!CheckDiskForExists(path,NodeType.Folder))
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
        MoveNode(folder, newPath,default);
    }

    public void MoveDirectory(string destPath, string newPath)
    {
        if (!Path.IsFolderPath(destPath))
        {
            throw new ArgumentException($"{destPath} is not correct path.");
        }
        
        var treeRelationPath = Path.FindRelation(_tree?.LocalRootPath, destPath);
        if (treeRelationPath == null)
        {
            throw new FolderNotFoundException($"{destPath} not found in current hierarchy");
        }
        
        if (!_tree.Exists(treeRelationPath))
        {
            throw new FolderNotFoundException($"{destPath} not found in current hierarchy");
        }

        var folder = _tree.Find(treeRelationPath);
        if (folder == null && folder?.Type == NodeType.Folder)
        {
            throw new FolderNotFoundException("Destination folder not found.");
        }

        
        MoveNode(folder, newPath,destPath);
        
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
        
        var treeRelationPath = Path.FindRelation(_tree?.LocalRootPath, destPath);
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

    public void Remove(Node node)
    {
        var localNodePath = Path.MergePaths(Path.RemoveLastSection(_tree?.LocalRootPath)
            , Path.SplitPath(node.Path));
        CheckDiskForExists(node, localNodePath);
        DeleteNode(node, localNodePath);
    }

    public void Remove(string localNodePath)
    {
        var treeRelationPath = Path.FindRelation(_tree?.LocalRootPath, localNodePath);
        if (treeRelationPath == null)
        {
            throw new NodeNotFoundException("The node not found in current hierarchy");
        }
        
        var node = _tree.Find(treeRelationPath);
        CheckDiskForExists(node, localNodePath);
        DeleteNode(node, localNodePath);
    }

    public void Rename(Node node, string newName)
    {
        if (!_tree.Exists(node))
        {
            throw new NodeNotFoundException($"{node.Name} not exists in current hierarchy.");
        }

        var localRootPath = Path.MergePaths(Path.RemoveLastSection(_tree?.LocalRootPath), Path.SplitPath(node.Path));
        switch (node.Type)
        {
            case NodeType.Folder:
                localRootPath = Path.MergePaths(localRootPath, node.Name);
                break;
            case NodeType.File:
                localRootPath = Path.MergeFileNameToPath(localRootPath, node.Name);
                break;
        }
        RenameNode(localRootPath, newName, node);
    }
    
    public void Rename(string path, string newName)
    {
        var treeRelationPath = Path.FindRelation(_tree?.LocalRootPath, path);
        if (treeRelationPath == null)
        {
            throw new NodeNotFoundException($"{path} not exists in current hierarchy.");
        }

        var node = _tree.Find(treeRelationPath);
        RenameNode(path, newName, node);
    }

    private static void RenameNode(string path, string newName, Node node)
    {
        node.Rename(newName);
        var newPath = Path.RemoveLastSection(path) + "/" + newName;
        switch (node.Type)
        {
            case NodeType.Folder:
                Directory.Move(path,newPath);
                break;
            case NodeType.File:
                File.Move(path,newPath);
                break;
        }
    }

    private static void DeleteNode(Node node, string localNodePath)
    {
        node.Delete();
        var type = node.Type;
        switch (type)
        {
            case NodeType.Folder:
                Directory.Delete(localNodePath);
                break;
            case NodeType.File:
                File.Delete(localNodePath);
                break;
        }
    }

    private void CheckDiskForExists(Node node, string localNodePath)
    {
        var info = GetFileSystemInfo(localNodePath, node.Type);
        if (!info.Exists)
        {
            throw node.Type switch
            {
                NodeType.Folder => new DirectoryNotFoundException($"{node.Name} not exist in the disk."),
                NodeType.File => new FileNotFoundException($"{node.Name} not exist in the disk."),
                _ => new InvalidOperationException("Unknown NodeType")
            };
        }
    }

    private bool CheckDiskForExists(string localNodePath, NodeType type)
    {
        var info = GetFileSystemInfo(localNodePath, type);
        if (!info.Exists)
        {
            return false;
        }

        return true;
    }

    private void MoveNode(Node node, string newPath, string? destinationPath)
    {
        string sourcePath;
        if (destinationPath == null)
        {
            var splitNodePath = Path.SplitPath(node.Path);
            sourcePath = Path.MergePaths(Path.RemoveLastSection(_tree?.LocalRootPath), splitNodePath);
            sourcePath = node.Type == NodeType.File ? 
                Path.MergeFileNameToPath(sourcePath, node.Name) : 
                Path.MergePaths(sourcePath, node.Name);
        }
        else
        {
            sourcePath = destinationPath;
        }

        var treeRelationPath = Path.FindRelation(_tree?.LocalRootPath, newPath);
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

    private void AdjustTree(Node node, string path, NodeType type)
    {
        var info = GetFileSystemInfo(path, NodeType.Folder);

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

    private FileSystemInfo GetFileSystemInfo(string path, NodeType type)
    {
        switch (type)
        {
            case NodeType.Folder:
                return new DirectoryInfo(path);
            case NodeType.File:
                return new FileInfo(path);
            default:
                throw new InvalidOperationException("Unknown NodeType");
        }
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