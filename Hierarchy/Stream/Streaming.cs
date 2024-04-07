using Hierarchy.Exceptions;
using Hierarchy.HierarchyTree;
using Hierarchy.HierarchyTree.Nodes;
using Path = Hierarchy.Utilities.Path;
#pragma warning disable CS8604 // Possible null reference argument.

namespace Hierarchy.Stream;

public class Streaming
{
    private Tree _tree;
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

    public void Remove(Node node)
    {
        var localNodePath = Path.MergePaths(_tree.LocalRootPath
            , Path.SplitPath(node.Path)) + node.Name;
        CheckDiskForExists(node, localNodePath);
        DeleteNode(node, localNodePath);
    }

    public void Remove(string localNodePath)
    {
        var treeRelationPath = Path.FindRelation(_tree.LocalRootPath, localNodePath);
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

        var localRootPath = Path.MergePaths(_tree.LocalRootPath, Path.SplitPath(node.Path));
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
        var treeRelationPath = Path.FindRelation(_tree.LocalRootPath, path);
        if (treeRelationPath == null)
        {
            throw new NodeNotFoundException($"{path} not exists in current hierarchy.");
        }

        var node = _tree.Find(treeRelationPath);
        RenameNode(path, newName, node);
    }

    public void CreateFolder(FolderNode node,string newFolderName)
    {
        if (!_tree.Exists(node))
        {
            throw new FolderNotFoundException($"{node.Name} not exists in current hierarchy");
        }

        CreateFolderNode(node, newFolderName);
    }

    public void CreateFolder(string path, string newFolderName)
    {
        var treeRelationPath = Path.FindRelation(_tree.LocalRootPath, path);
        if (treeRelationPath == null)
        {
            throw new NodeNotFoundException("The node not found in current hierarchy");
        }

        var node = _tree.Find(treeRelationPath);
        if (node is FolderNode folderNode)
        {
            CreateFolderNode(folderNode, newFolderName);
        }
    }

    public FolderNode GoUp()
    {
        var currentFolder = _tree.GetCurrentFolder();
        if (currentFolder.Parent != null)
        {
            currentFolder.IsOpen = false;
            currentFolder = currentFolder.Parent;
            
            return currentFolder;
        }
        
        var localRootPath = Path.MergePaths(_tree.LocalRootPath,
            Path.SplitPath(currentFolder.Path));
        var newLocalRootPath = Path.RemoveLastSection(localRootPath);
        var newTree = new Tree(new DirectoryInfo(newLocalRootPath).Name,newLocalRootPath);
        newTree.AppendTree(_tree);
        GetChildren(newTree.GetRoot(),newLocalRootPath);
        _tree = newTree;
        return newTree.GetRoot();
        
    }

    public int TotalCount()
    {
        return GetTotalCount(default);
    }
    
    public int TotalCount(FolderNode folderNode)
    {
        return GetTotalCount(folderNode, default);
    }
    
    public int TotalCount(string path)
    {
        return GetTotalCount(path, default);
    }

    public int TotalFolderCount()
    {
        return GetTotalCount(NodeType.Folder);
    }

    public int TotalFolderCount(FolderNode folderNode)
    {
        return GetTotalCount(folderNode, NodeType.Folder);
    }

    public int TotalFolderCount(string path)
    {
        return GetTotalCount(path, NodeType.Folder);
    }

    public int TotalFileCount()
    {
        return GetTotalCount(NodeType.File);
    }
    
    public int TotalFileCount(FolderNode folderNode)
    {
        return GetTotalCount(folderNode, NodeType.File);
    }
    
    public int TotalFileCount(string path)
    {
        return GetTotalCount(path, NodeType.File);
    }
    
    
    private int GetTotalCount(NodeType? type)
    {
        switch (type)
        {
            case NodeType.Folder:
                return _tree.TotalFolderCount();
            case NodeType.File:
                return _tree.TotalFileCount();
            case null:
                return _tree.Count;
            default:
                throw new InvalidOperationException("Unknown NodeType");
        }
    }

    private int GetTotalCount(FolderNode folderNode, NodeType? type)
    {
        if (folderNode == null)
        {
            throw new ArgumentNullException();
        }
        
        switch (type)
        {
            case NodeType.Folder:
                return _tree.TotalFolderCount(folderNode);
            case NodeType.File:
                return _tree.TotalFileCount(folderNode);
            case null:
                return folderNode.Count;
            default:
                throw new InvalidOperationException("Unknown NodeType");
        }
    }
    
    private int GetTotalCount(string path, NodeType? type)
    {
        var treeRelationPath = Path.FindRelation(_tree.LocalRootPath, path);
        var node = _tree.Find(treeRelationPath);
        if (node == null)
        {
            throw new FolderNotFoundException($"{Path.GetLastSection(path)} not found.");
        }

        if (node is FolderNode folderNode)
        {
            switch (type)
            {
                case NodeType.Folder:
                    return _tree.TotalFolderCount(folderNode);
                case NodeType.File:
                    return _tree.TotalFileCount(folderNode);
                case null:
                    return folderNode.Count;
            }
        }

        throw new FolderNotFoundException("Cannot retrieve total count of the file.");
    }
    
    private void CreateFolderNode(FolderNode node, string newFolderName)
    {
        node.AddFolder(newFolderName);
        var localRootPath = Path.MergePaths(_tree.LocalRootPath, Path.SplitPath(node.Path));
        var newFolderPath = Path.MergePaths(localRootPath, node.Name, newFolderName);
        Directory.CreateDirectory(newFolderPath);
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
            sourcePath = Path.MergePaths(_tree.LocalRootPath, splitNodePath); // - /Users/macbook/Desktop/RamidNew/Aha:/ - Changed version
            sourcePath = node.Type == NodeType.File ? 
                Path.MergeFileNameToPath(sourcePath, node.Name) : 
                Path.MergePaths(sourcePath, node.Name);
        }
        else
        {
            sourcePath = destinationPath;
        }

        var treeRelationPath = Path.FindRelation(_tree.LocalRootPath, newPath);
        if (treeRelationPath == null || !_tree.Exists(Path.RemoveLastSection(treeRelationPath))) //../Desktop/RamidNew/ - changed version - ../RamidNew/
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

    private void GetChildren(FolderNode folder, string path, bool isFirstCall = true)
    {
        var files = Directory.GetFiles(path).Where(file => !Path.GetFileName(file).StartsWith(".")).ToList();
        var directories = Directory.GetDirectories(path).ToList();

        if (isFirstCall)
        {
            RemoveExistingItems(files, folder.Children.Select(child => child.LocalPath + child.Name));
            RemoveExistingItems(directories, folder.Children.Select(child => child.LocalPath + child.Name.Replace('/',':')));
        }

        foreach (var file in files)
        {
            folder.AddFile(Path.GetFileName(file), Path.RemoveLastSection(file));
        }

        foreach (var directory in directories)
        {
            var folderName = Path.GetFileName(directory);
            folder.AddFolder(folderName, Path.RemoveLastSection(directory));
        }

        foreach (var child in folder.Children.OfType<FolderNode>())
        {
            var subPath = directories.FirstOrDefault(d => d.EndsWith(Path.ReplaceIfExists(child.Name, '/', ':')));
            if (subPath != null)
            {
                GetChildren(child, subPath, false);
            }
        }
    }

    private void RemoveExistingItems(List<string> items, IEnumerable<string> existingPaths)
    {
        foreach (var existingPath in existingPaths)
        {
            items.Remove(existingPath);
        }
    }

}