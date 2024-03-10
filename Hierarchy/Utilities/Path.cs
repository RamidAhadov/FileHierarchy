namespace Hierarchy.Utilities;

public class Path
{
    public static string SetPath(string parentPath, string parentName)
    {
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
}