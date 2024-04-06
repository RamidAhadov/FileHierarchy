using Path = Hierarchy.Utilities.Path;

namespace Hierarchy.Console;

public static class Command
{
    private static List<string> _commands;
    public static bool IsCommand(string input)
    {
        if (_commands == null)
        {
            LoadCommands();
        }
        input = input.Trim();
        var command = Path.GetCommand(input);
        if (command == null)
        {
            return false;
        }
        
        if (_commands.Contains(command.ToLower()))
        {
            return true;
        }

        return false;
    }

    private static void LoadCommands()
    {
        _commands =
        [
            "help",
            "tree",
            "move",
            "remove",
            "rename",
            "getcount",
            "getfoldercount",
            "getfilecount",
            "print",
            "up",
            "exit"
        ];
    }
}