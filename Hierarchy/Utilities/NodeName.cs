using System.Text.RegularExpressions;

namespace Hierarchy.Utilities;

public static class NodeName
{
    [Obsolete($"Please use {nameof(ValidateFileName)}")]
    public static void ValidateFileNameManual(string name)
    {
        if (!name.Contains('.') || name.Contains(':'))
        {
            throw new ArgumentException($"{name} is not correct file name.");
        }

        int dotIndex = name.IndexOf('.');
        string afterDot = name.Substring(dotIndex + 1);
        if (afterDot.Length <= 0)
        {
            throw new ArgumentException($"{name} is not correct file name.");
        }

        string beforeDot = name.Substring(0, dotIndex - 1);
        if (beforeDot.Length <= 0)
        {
            throw new ArgumentException($"{name} is not correct file name.");
        }
    }

    [Obsolete($"Please use {nameof(ValidateFolderName)}")]
    public static void ValidateFolderNameManual(string name)
    {
        if (name.Contains(':'))
        {
            throw new ArgumentException($"{name} is not correct folder name.");
        }

        if (name.Contains('.'))
        {
            int dotIndex = name.IndexOf('.');
            string afterDot = name.Substring(dotIndex + 1);
            if (afterDot.Length > 0)
            {
                throw new ArgumentException($"{name} is not correct folder name.");
            }
            
            string beforeDot = name.Substring(0, dotIndex - 1);
            if (beforeDot.Length <= 0)
            {
                throw new ArgumentException($"{name} is not correct folder name.");
            }
        }
    }
    
    public static void ValidateFileName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("File name cannot be empty or whitespace.");
        }
        
        if (Regex.IsMatch(name,"^\\."))
        {
            throw new ArgumentException($"{name} is not a valid file name. File body is missing.");
        }

        if (name.Contains(':') || name.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) != -1)
        {
            throw new ArgumentException($"{name} is not a valid file name.");
        }

        if (!name.Contains('.'))
        {
            throw new ArgumentException($"{name} is not a valid file name. File extension is missing.");
        }

        string[] parts = name.Split('.');
        string extension = parts[^1];
        if (string.IsNullOrWhiteSpace(extension))
        {
            throw new ArgumentException($"{name} is not a valid file name. File extension is missing.");
        }
    }

    public static void ValidateFolderName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Folder name cannot be empty or whitespace.");
        }

        if (name.Contains(':') || name.IndexOfAny(System.IO.Path.GetInvalidPathChars()) != -1)
        {
            throw new ArgumentException($"{name} is not a valid folder name.");
        }
    }
}