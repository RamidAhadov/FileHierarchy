using Hierarchy.HierarchyTree;
using Hierarchy.Stream;
using Path = Hierarchy.Utilities.Path;

namespace Hierarchy.Console;

public class Action
{
    private static Streaming _streaming;
    
    public static void StartAction(string input)
    {
        var command = Path.GetCommand(input);
        input = Path.DeleteCommand(input);
        switch (command)
        {
            case "help":
                ShowInfo();
                break;
            case "tree":
                CreateTree(input);
                break;
            case "move":
                Move(input);
                break;
            case "remove":
                Remove(input);
                break;
            case "rename":
                Rename(input);
                break;
            case "getcount":
                ShowTotalCount(input, default);
                break;
            case "getfoldercount":
                ShowTotalCount(input, NodeType.Folder);
                break;
            case "getfilecount":
                ShowTotalCount(input, NodeType.File);
                break;
            case "print":
                Print();
                break;
            case "up":
                GoUp();
                break;
            case "exit":
                Close();
                break;
        }
    }


    private static void ShowInfo()
    {
        System.Console.WriteLine(_info);
    }

    private static void CreateTree(string command)
    {
        if (string.IsNullOrEmpty(command))
        {
            System.Console.WriteLine("Command is not in the right format. Path not exists.");
            return;
        }
        
        if (_streaming != null)
        {
            System.Console.WriteLine("Tree was already created.");
            return;
        }
        
        //var path = Path.DeleteCommand(command);
        var splitPath = command.Split(' ');
        if (command.Contains(' ') || splitPath.Length == 0)
        {
            System.Console.WriteLine("Command is not in the right format. Path is wrong.");
            return;
        }

        try
        {
            _streaming = new Streaming(command);
        }
        catch (Exception e)
        {
            System.Console.WriteLine(e.Message);
        }
    }

    private static void Move(string input)
    {
        try
        {
            CheckTree();
        }
        catch (Exception e)
        {
            System.Console.WriteLine(e.Message);
            return;
        }

        if (input == null)
        {
            System.Console.WriteLine("Paths not exist.");
            return;
        }
        
        var splitPath = input.Split(' ');
        if (splitPath == null || splitPath.Length != 2)
        {
            System.Console.WriteLine("Path is not right.");
            return;
        }

        var destPath = splitPath[0];
        var newPath = splitPath[1];
        var destType = Path.GetTypeByPath(destPath);
        try
        {
            switch (destType)
            {
                case NodeType.Folder:
                    _streaming.MoveDirectory(destPath, newPath);
                    break;
                case NodeType.File:
                    _streaming.MoveFile(destPath, newPath);
                    break;
            }
        }
        catch (Exception e)
        {
            System.Console.WriteLine(e.Message);
        }
    }
    
    private static void Remove(string input)
    {
        try
        {
            CheckTree();
        }
        catch (Exception e)
        {
            System.Console.WriteLine(e.Message);
            return;
        }
        
        var splitPath = input.Split(' ');
        if (splitPath == null || splitPath.Length != 1)
        {
            System.Console.WriteLine("Path is not right.");
            return;
        }

        try
        {
            _streaming.Remove(splitPath[0]);
        }
        catch (Exception e)
        {
            System.Console.WriteLine(e.Message);
        }
    }
    
    private static void Rename(string input)
    {
        try
        {
            CheckTree();
        }
        catch (Exception e)
        {
            System.Console.WriteLine(e.Message);
            return;
        }

        if (input == null)
        {
            System.Console.WriteLine("Paths not exist.");
            return;
        }
        
        var splitPath = input.Split(' ');
        if (splitPath == null || splitPath.Length != 2)
        {
            System.Console.WriteLine("Path is not right.");
            return;
        }
        
        try
        {
            _streaming.Rename(splitPath[0], splitPath[1]);
        }
        catch (Exception e)
        {
            System.Console.WriteLine(e.Message);
        }
    }

    private static void ShowTotalCount(string input, NodeType? type)
    {
        try
        {
            CheckTree();
        }
        catch (Exception e)
        {
            System.Console.WriteLine(e.Message);
            return;
        }
        
        string[] splitPath = input == null ? Array.Empty<string>() : input.Split(' ');
        
        if (splitPath == null || splitPath.Length > 1)
        {
            System.Console.WriteLine("Path is not right.");
            return;
        }

        switch (splitPath.Length)
        {
            case 0:
                switch (type)
                {
                    case NodeType.Folder:
                        System.Console.WriteLine($"Total folders count: {_streaming.TotalFolderCount()}");
                        break;
                    case NodeType.File:
                        System.Console.WriteLine($"Total files count: {_streaming.TotalFileCount()}");
                        break;
                    case null:
                        System.Console.WriteLine($"Total items count: {_streaming.TotalCount()}");
                        break;
                }
                break;
            case 1:
                switch (type)
                {
                    case NodeType.Folder:
                        System.Console.WriteLine($"Total folders count: {_streaming.TotalFolderCount(splitPath[0])}");
                        break;
                    case NodeType.File:
                        System.Console.WriteLine($"Total files count: {_streaming.TotalFileCount(splitPath[0])}");
                        break;
                    case null:
                        System.Console.WriteLine($"Total items count: {_streaming.TotalCount(splitPath[0])}");
                        break;
                }
                break;
        }
    }
    
    private static void Print()
    {
        try
        {
            CheckTree();
        }
        catch (Exception e)
        {
            System.Console.WriteLine(e.Message);
            return;
        }
        
        foreach (var data in _streaming.GetTree().Print())
        {
            System.Console.WriteLine(data);
        }
    }

    private static void GoUp()
    {
        try
        {
            CheckTree();
        }
        catch (Exception e)
        {
            System.Console.WriteLine(e.Message);
            return;
        }

        _ = _streaming.GoUp();
    }

    private static void Close()
    {
        System.Console.WriteLine("Console is closing...");
        Environment.Exit(0);
    }

    private static void CheckTree()
    {
        if (_streaming == null)
        {
            throw new NullReferenceException("Tree was not created.");
        }
    }

    private const string _info = "You can use below commands: " +
                                 "\nhelp\ntree\nmove\nremove\nrename\ngetcount\ngetfilecount" +
                                 "\ngetfoldercount\nprint\nup\nexit";
}