using System.Text.RegularExpressions;
#pragma warning disable CS8603 // Possible null reference return.

namespace Hierarchy.Utilities;

public class Path
{
    public static string SetPath(string parentPath, string parentName)
    {
        if (parentName.Contains("/"))
        {
            parentName = parentName.Replace('/',':');
        }

        if (!parentPath.EndsWith('/'))
        {
            return parentPath + "/" + parentName + "/";
        }
        return parentPath + parentName + "/";
    }

    public static string[] SplitPath(string path)
    {
        if (path.EndsWith("/"))
        {
            path = path.Substring(0, path.Length - 1);
        }

        string substring;
        if (path.StartsWith("../"))
        {
            substring = path.Substring(3);
        }
        else
        {
            substring = path.Substring(1);
        }
        
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

    public static bool IsFolderPath(string path)
    {
        if (string.IsNullOrWhiteSpace(path) || path.Length < 2)
        {
            return false;
        }

        if (!path.StartsWith('/') || path.Contains(':'))
        {
            return false;
        }

        if (path.Contains(':'))
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

    public static string RemoveLastSection(string path)
    {
        path = path.TrimEnd('/');
        var lastSlashIndex = path.LastIndexOf('/');
        
        return path[..(lastSlashIndex + 1)];
    }

    public static string MergeTwoPaths(string rootPath, string nodePath)
    {
        if (rootPath == null || nodePath == null)
        {
            throw new ArgumentNullException();
        }

        if (!rootPath.EndsWith('/'))
        {
            rootPath += "/";
        }

        if (nodePath.StartsWith("../"))
        {
            //Range indexer instead of Substring method.
            nodePath = nodePath[3..];
        }

        int slashIndex = nodePath.IndexOf('/');

        nodePath = nodePath[(slashIndex + 1)..];

        return rootPath + nodePath;
    }

    public static string MergePaths(string basePath, params string[] paths)
    {
        if (paths.Length == 0)
        {
            return basePath;
        }

        if (!basePath.EndsWith("/"))
        {
            basePath += "/";
        }

        foreach (var path in CheckAndSetAddresses(paths))
        {
            basePath += path;
        }

        return basePath;
    }

    private static IEnumerable<string> CheckAndSetAddresses(string[] addresses)
    {
        for (int i = 0; i < addresses.Length; i++)
        {
            yield return FilterAddress(addresses[i]);
        }
    }

    private static string FilterAddress(string address)
    {
        if (Regex.IsMatch(address,"^[^/]+/$"))
        {
            return address;
        }
        
        if (address.StartsWith("../"))
        {
            address = address[3..];
        }
        
        address = address.TrimStart('/');
        
        if (!address.EndsWith('/'))
        {
            address += "/";
        }
        
        if (address.Length < 2)
        {
            throw new ArgumentException();
        }
        
        if (address[..^1].Contains('/'))
        {
            address = address.Replace('/', ':');
        }

        return address;
    }
}