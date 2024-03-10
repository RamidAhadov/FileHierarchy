namespace Hierarchy.Utilities;

public class Path
{
    public static string SetPath(string parentPath, string parentName)
    {
        return parentPath + parentName + "/";
    }

    public static string[] SplitPath(string path)
    {
        var substring = path.Substring(3);
        return substring.Split('/');
    }
}