using Hierarchy.HierarchyTree;
using Hierarchy.HierarchyTree.Nodes;

var tree = new Tree("Folder");


var folder = tree.GetRoot();

for (int i = 0; i < 1; i++)
{
    folder.AddFolder($"Folder {i}");
}

foreach (var child in folder.Children)
{
    if (child is FolderNode node)
    {
        node.AddFolder(new FolderNode("New folder - 0",node));
        node.AddFolder(new FolderNode("New folder - 1",node));
        node.AddFolder(new FolderNode("New folder - 2",node));
        node.AddFile(new FileNode("File.exe",node));
    }
}

foreach (var item in tree)
{
    Console.WriteLine(item.Name);
    Console.WriteLine(item.Path);
    if (item is FileNode node)
    {
        Console.WriteLine(node.Extension);
    }
    Console.WriteLine("-------------------------------");
}

var foundNode = tree.Find("../Folder");
Console.WriteLine(foundNode.Type);

Console.WriteLine("Total folder(s): {0}",tree.GetTotalFolderCount());
Console.WriteLine("Total file(s): {0}",tree.GetTotalFileCount());
Console.WriteLine("Total item(s): {0}",tree.Count);

tree.Print();

Console.WriteLine("Finish");