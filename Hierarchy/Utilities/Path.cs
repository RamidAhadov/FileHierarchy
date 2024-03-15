using System.Text.RegularExpressions;

namespace Hierarchy.Utilities;

public class Path
{
    public static string SetPath(string parentPath, string parentName)
    {
        if (parentName.Contains("/"))
        {
            parentName = parentName.Replace('/',':');
        }
        
        return parentPath + parentName + "/";
    }

    public static string[] SplitPath(string path)
    {
        if (path.EndsWith("/"))
        {
            path = path.Substring(0, path.Length - 1);
        }
        var substring = path.Substring(3);
        
        return substring.Split('/').Skip(1).ToArray();
    }

    public static bool IsFolderPath(string[] addresses)
    {
        if (addresses.Length == 0)
        {
            return true;
        }
        
        if (Regex.IsMatch(addresses[^1], @"\..+"))
        {
            return false;
        }

        return true;
    }

    public static string GetFileName(string path)
    {
        return path.Split('/').ToArray()[^1];
    }

    public static string ReplaceIfExists(string nodeName, char oldChar, char newChar)
    {
        if (nodeName.Contains(oldChar))
        {
            return nodeName.Replace(oldChar, newChar);
        }

        return nodeName;
    }
}