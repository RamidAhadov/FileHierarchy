using System.Text.RegularExpressions;

namespace Hierarchy.Utilities;

public static class NodeName
{
    public static void ValidateFileName(string name)
    {
        if (!Regex.IsMatch(name,"^[^:]+\\.[^:]+$"))
        {
            throw new ArgumentException($"{name} is not correct file name.");
        }
    }

    public static void ValidateFolderName(string name)
    {
        if (!Regex.IsMatch(name,"^[^:.]+$"))
        {
            throw new ArgumentException($"{name} is not correct file name.");
        }
    }
}