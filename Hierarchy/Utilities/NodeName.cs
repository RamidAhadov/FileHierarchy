using System.Text.RegularExpressions;

namespace Hierarchy.Utilities;

public static class NodeName
{
    public static void ValidateFileName(string name)
    {
        if (!Regex.IsMatch(name,"^[^*:><\\|]*\\.(?!.*:)[a-zA-Z0-9]+$"))
        {
            throw new ArgumentException();
        }
    }

    public static void ValidateFolderName(string name)
    {
        if (!Regex.IsMatch(name,"^([a-zA-Z0-9][^*/><?\\\\|:.]*)(?:\\\\.)?$"))
        {
            throw new ArgumentException();
        }
    }
}