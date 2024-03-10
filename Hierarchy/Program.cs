using Hierarchy.HierarchyTree;

var tree = new Tree("Folder");


var folder = tree.GetRoot();

for (int i = 0; i < 10; i++)
{
    folder.AddFolder($"Folder {i}");
}

foreach (var child in folder.Children)
{
    int num = 0;
    if (child is FolderNode node)
    {
        node.AddFolder(new FolderNode($"New folder - {num}", child.Path));
        num++;
    }
}

foreach (var items in tree)
{
    Console.WriteLine(items.Name);
}


Console.WriteLine("Finish");