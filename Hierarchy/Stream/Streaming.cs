using Hierarchy.Exceptions;
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
    
    public void MoveDirectory(FolderNode folder, string newPath)
    {
        try
        {
            MoveFolder(folder, newPath);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.InnerException?.Message);
        }
    }

    public void MoveDirectory(string destPath, string newPath)
    {
        if (Path.IsFolderPath(destPath))
        {
            throw new ArgumentException($"{destPath} is not correct path.");
        }

        var folder = _tree.Find(destPath);
        if (folder == null && folder.Type == NodeType.Folder)
        {
            throw new FolderNotFoundException("Destination folder not found.");
        }

        try
        {
            MoveFolder((FolderNode)folder, newPath);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.InnerException?.Message);
        }
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
    
    private void MoveFolder(FolderNode folderNode, string newPath)
    {
        if (!_tree.Exists(folderNode))
        {
            throw new FileNotFoundException();
        }

        string sourcePath = Path.MergePaths(Path.RemoveLastSection(_rootPath), folderNode.Path,folderNode.Name);
        newPath = Path.MergePaths(newPath, folderNode.Name);
        var directoryInfo = new DirectoryInfo(Path.RemoveLastSection(newPath));
        if (!_tree.Exists(Path.RemoveLastSection(newPath)))
        {
            if (!directoryInfo.Exists)
            {
                throw new DirectoryNotFoundException();
            }

            try
            {
                folderNode.Delete();
            }
            catch (DeleteRootException)
            {
                Console.WriteLine("Cannot move the root directory.");
            }
        }

        if (!folderNode.Disposed)
        {
            folderNode.MoveNode(newPath);
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