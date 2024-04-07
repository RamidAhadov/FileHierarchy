using Hierarchy.HierarchyTree;
using Hierarchy.HierarchyTree.Nodes;

namespace Hierarchy.Stream.Abstraction;

public interface IStream
{
    Tree GetTree();
    void MoveDirectory(FolderNode folder, string newPath);
    void MoveDirectory(string destPath, string newPath);
    void MoveFile(FileNode fileNode, string newPath);
    void MoveFile(string destPath, string newPath);
    void Remove(Node node);
    void Remove(string localNodePath);
    void Rename(Node node, string newName);
    void Rename(string path, string newName);
    void CreateFolder(FolderNode node, string newFolderName);
    void CreateFolder(string path, string newFolderName);
    FolderNode GoUp();
    int TotalCount();
    int TotalCount(FolderNode folderNode);
    int TotalCount(string path);
    int TotalFolderCount();
    int TotalFolderCount(FolderNode folderNode);
    int TotalFolderCount(string path);
    int TotalFileCount();
    int TotalFileCount(FolderNode folderNode);
    int TotalFileCount(string path);
}