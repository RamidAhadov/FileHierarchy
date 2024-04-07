using Hierarchy.Console;
using Action = Hierarchy.Console.Action;


GetCommand();

return;

static void GetCommand()
{
    var command = GetInput();
    
    Action.StartAction(command);
    
    GetCommand();
}

static string GetInput()
{
    var input = Console.ReadLine();
    while (!Command.IsCommand(input))
    {
        Console.WriteLine($"{input} is not correct command.");
        input = Console.ReadLine();
    }

    return input;
}