using Hierarchy.HierarchyTree;
using Hierarchy.HierarchyTree.Nodes;
using Hierarchy.Stream;

var tree = new Tree("Folder");


var folder = tree.GetRoot();

folder.AddFile(new FileNode("   .exe"));

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

((FolderNode)folder.Children[1]).Children[0].MoveNode("../Folder");

//((FolderNode)((FolderNode)folder.Children[0]).Children[0]).AddFolder("Yeni folder");
//((FolderNode)((FolderNode)folder.Children[0]).Children[0]).AddFolder("Ikinci folder");


//ar notNewedFolder = (FolderNode)((FolderNode)folder.Children[0]).Children[0];

//((FolderNode)folder.Children[0]).Children[0].MoveNode("../Folder");

//var movedNode = ((FolderNode)folder.Children[0]).Children[0];
//var movedNode1 = (FolderNode)folder.Children[^1];

//var newedFolder = (FolderNode)((FolderNode)folder.Children[0]).Children[0];
folder.Children[1].Rename("Renamed1");

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



//tree.Print();

var streaming = new Streaming();
var newTree = streaming.ReadDirectory("/Users/macbook/Desktop/Ramid");
var treeRoot = newTree.GetRoot();
newTree.Print();

((FolderNode)treeRoot.Children[3]).Children[1].MoveNode("../Ramid/");

Console.WriteLine("Total folder(s): {0}",newTree.GetTotalFolderCount());
Console.WriteLine("Total file(s): {0}",newTree.GetTotalFileCount());
Console.WriteLine("Total item(s): {0}",newTree.Count);


Console.WriteLine("Finish");