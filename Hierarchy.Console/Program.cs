using System.Diagnostics;
using Hierarchy.HierarchyTree;
using Hierarchy.HierarchyTree.Nodes;
using Hierarchy.Stream;
using Path = Hierarchy.Utilities.Path;

var tree = new Tree("Folder");

var input = Console.ReadLine();
while (input != "Start")
{
    Console.WriteLine($"{input} is not correct input. Please try again.");
    input = Console.ReadLine();
}
string name = "RamidAhadov";
string path2 = "/Users/macbook/Desktop/RamidNew/";
string path3 = "/Users/macbook/Desktop/RamidNew/Folders/Nothing";
var removedPath = Path.RemoveLastSection(path2);
string pathA = "/Ramid/";
string pathB = "/Ahadov/";
string pathC = "/Baku/";

var firstSectionRemoved = Path.RemoveFirstSection(path2);

//var stream = new Streaming(path2).GetTree().GetRoot();

var relatedPath = Path.FindRelation(path2, path3);
int retry = 1;
string[] addresses = new string[retry];
for (int i = 0; i < retry; i++)
{
    if (i%2 == 0)
    {
        addresses[i] = "ra:mid/";
    }
    else
    {
        addresses[i] = "aha/dov/";
    }
}
string name1 = name.Substring(0, name.Length - 1);

string[] testPath = new[] {"Ramid","/ram","ahadoc/", "aha:s" };

var st = new Stopwatch();
st.Start();
//var path1 = Path.MergePaths("Evv", addresses);
st.Stop();

var seconds = st.ElapsedMilliseconds;
Console.WriteLine(seconds);

//Thread.Sleep(100000);

var readDirectory = new Streaming("/Users/macbook/Desktop/RamidNew/");
var directoryRoot = readDirectory.GetTree().GetRoot();
// var baseFolder = new FolderNode("Folder");
// var disposedNode = new FileNode("Program.cs");
// baseFolder.AddFile(disposedNode);
// disposedNode.Delete();

var folder = tree.GetRoot();


//((FolderNode)folder.Children[0]).Children[0].MoveNode("../Folder");

//((FolderNode)((FolderNode)folder.Children[0]).Children[0]).AddFolder("Yeni folder");
//((FolderNode)((FolderNode)folder.Children[0]).Children[0]).AddFolder("Ikinci folder");


//ar notNewedFolder = (FolderNode)((FolderNode)folder.Children[0]).Children[0];

//((FolderNode)folder.Children[0]).Children[0].MoveNode("../Folder");

//var movedNode = ((FolderNode)folder.Children[0]).Children[0];
//var movedNode1 = (FolderNode)folder.Children[^1];

//var newedFolder = (FolderNode)((FolderNode)folder.Children[0]).Children[0];
//folder.Children[0].Rename("Renamed1");

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


string basePath = "/Users/user/macbook/desktop/ramid/ahadov";
string localPath = "/Users/user/macbook/desktop/ramid/ahadov/Folder/foll/doll";
var list = new List<int>();



//tree.Print();

st.Restart();
Streaming streaming = new Streaming("/Users/macbook/Desktop/RamidNew/");
var newTree = streaming.GetTree();
var treeRoot = newTree.GetRoot();
var movedNode = ((FolderNode)treeRoot.Children[0]);

// st.Restart();
// for (int i = 0; i < 100000; i++)
// {
//     _ = newTree.Find("../RamidNew/Aha:/Ramm/rrr.exe/untitled folder/Friend.html");
// }
//
// st.Stop();
// Console.WriteLine(st.ElapsedMilliseconds);
//Thread.Sleep(1000000);

//var info = new DirectoryInfo("/Users/macbook/Desktop/RamidNew/");
//bool directoryExists = info.Exists;
//Works, but moves wrong directory and creates wrong folder name
//streaming.MoveDirectory(movedNode,"/Users/macbook/Desktop/Folder2/");
//streaming.MoveDirectory("/Users/macbook/Desktop/RamidNew/ramid.m4ab/","/Users/macbook/Desktop/Folder2/");
var fileNode = (FileNode)movedNode.Children[0];
var folderNode = movedNode.Children[1];
//ACCESS ISSUE ABOUT FILEINFOSYSTEM
var info = new DirectoryInfo("/Users/macbook/Desktop/Folder2/");
bool exists = info.Exists;
streaming.MoveFile(fileNode,"/Users/macbook/Desktop/Folder2/");
//streaming.MoveFile("/Users/macbook/Desktop/RamidNew/Aha:/powershelltcp copy111.txt","/Users/macbook/Desktop/Folder2/");
//streaming.Remove("/Users/macbook/Desktop/RamidNew/Aha:/Ramm/rrr.exe/untitled folder/Friend.html");
//streaming.MoveNode(fileNode,"/Users/macbook/Desktop/Folder2/",default);

//bool nodeExists = newTree.Exists(fileNode);

try
{
    //Directory.Move("/Users/macbook/Desktop/RamidNew/ramid.m4ab/","/Users/macbook/Desktop/Folder2/");
}
catch (Exception e)
{
    Console.WriteLine(e.Message);
}

st.Stop();

Console.WriteLine(st.ElapsedMilliseconds);
//newTree.Print();

//Directory.Move("/Users/macbook/Desktop/Folder2/VV/","/Users/macbook/Desktop/RamidNew");

//((FolderNode)treeRoot.Children[3]).Children[1].MoveNode("../Ramid/");

Console.WriteLine("Total folder(s): {0}",newTree.GetTotalFolderCount());
Console.WriteLine("Total file(s): {0}",newTree.GetTotalFileCount());
Console.WriteLine("Total item(s): {0}",newTree.Count);


Console.WriteLine("Finish");