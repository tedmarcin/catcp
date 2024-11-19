using System;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;

class Program
{
    static bool ShouldExclude(string fileName, string[] excludePatterns)
    {
        foreach (var pattern in excludePatterns)
        {
            if (fileName.Contains(pattern))
            {
                return true;
            }
        }
        return false;
    }

    static void WriteUtf8File(string filePath, string content)
    {
        try
        {
            File.WriteAllText(filePath, content, System.Text.Encoding.UTF8);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to open file for writing: {ex.Message}");
        }
    }

    static void ProcessFile(string filePath, ref string contentToWrite, int maxLines)
    {
        try
        {
            var lines = File.ReadLines(filePath).Take(maxLines);
            contentToWrite += $"===== File: {filePath} =====\n";
            foreach (var line in lines)
            {
                contentToWrite += line + "\n";
            }
            contentToWrite += $"===== End of {filePath} =====\n\n";
        }
        catch (FileNotFoundException ex)
        {
            Console.WriteLine($"File not found: {filePath}, Error: {ex.Message}");
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine($"Access denied to file: {filePath}, Error: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to process file: {filePath}, Error: {ex.Message}");
        }
    }

    static void ShowHelp()
    {
        Console.WriteLine("Usage: catcp.exe [options] [file1] [file2] ...");
        Console.WriteLine("Options:");
        Console.WriteLine("  -exclude=<pattern1,pattern2,...>   Exclude files matching these patterns");
        Console.WriteLine("  -max=<number>                      Maximum number of lines to read from each file (default is 10000)");
        Console.WriteLine("\nExample:");
        Console.WriteLine("  catcp.exe -exclude=log,tmp -max=500 file1.txt file2.txt");
        Console.WriteLine("  catcp.exe file1.txt file2.txt file3.txt -exclude=log,tmp");
        Console.WriteLine("  catcp.exe *.txt *.cs -exclude=log,tmp");
        Console.WriteLine("  catcp.exe text*.* text.* te*t.*");
    }

    static List<string> ExpandWildcards(string pattern)
    {
        List<string> matchedFiles = new List<string>();
        string directory = Path.GetDirectoryName(pattern);
        if (string.IsNullOrEmpty(directory))
        {
            directory = Directory.GetCurrentDirectory();
        }
        string searchPattern = Path.GetFileName(pattern);
        if (string.IsNullOrEmpty(searchPattern))
        {
            searchPattern = "*";
        }

        try
        {
            // Überprüfen, ob das Verzeichnis existiert
            if (Directory.Exists(directory))
            {
                matchedFiles.AddRange(Directory.GetFiles(directory, searchPattern));
            }
            else
            {
                Console.WriteLine($"Directory does not exist: {directory}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error expanding pattern '{pattern}': {ex.Message}");
        }

        return matchedFiles;
    }

    static void Main(string[] args)
    {
        if (args.Length == 0 || args.Contains("--help"))
        {
            Console.WriteLine("Error: Missing parameters.");
            ShowHelp();
            return;
        }

        // Separate option arguments and file patterns
        var optionArgs = args.Where(arg => arg.StartsWith("-")).ToArray();
        var filePatterns = args.Where(arg => !arg.StartsWith("-")).ToArray();

        // Parse exclude patterns
        var excludePatterns = optionArgs
            .Where(arg => arg.StartsWith("-exclude="))
            .SelectMany(arg => arg.Substring(9).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            .ToArray();

        // Parse max lines
        int maxLines = 10000; // Default value
        var maxArg = optionArgs.FirstOrDefault(arg => arg.StartsWith("-max="));
        if (maxArg != null)
        {
            if (int.TryParse(maxArg.Substring(5), out int max))
            {
                maxLines = max;
            }
            else
            {
                Console.WriteLine("Invalid value for -max parameter. Using default value of 10000.");
            }
        }

        // Expand file patterns to actual file paths
        List<string> includeFiles = new List<string>();
        foreach (var pattern in filePatterns)
        {
            if (pattern.Contains("*") || pattern.Contains("?"))
            {
                includeFiles.AddRange(ExpandWildcards(pattern));
            }
            else
            {
                try
                {
                    var fullPath = Path.GetFullPath(pattern);
                    if (File.Exists(fullPath))
                    {
                        includeFiles.Add(fullPath);
                    }
                    else
                    {
                        Console.WriteLine($"File does not exist: {fullPath}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Invalid file path '{pattern}': {ex.Message}");
                }
            }
        }

        // Remove duplicates
        includeFiles = includeFiles.Distinct().ToList();

        if (includeFiles.Count == 0)
        {
            Console.WriteLine("Error: No files specified or no files matched the patterns.");
            ShowHelp();
            return;
        }

        var tempFilePath = Path.Combine(Path.GetTempPath(), "output_utf8.txt");
        var contentToWrite = string.Empty;

        foreach (var file in includeFiles)
        {
            var fullPath = Path.GetFullPath(file);
            var fileName = Path.GetFileName(fullPath);

            if (!ShouldExclude(fileName, excludePatterns))
            {
                ProcessFile(fullPath, ref contentToWrite, maxLines);
            }
        }

        WriteUtf8File(tempFilePath, contentToWrite);

        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = tempFilePath,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to open the output file: {ex.Message}");
        }
    }
}
