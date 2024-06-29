using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class Program
{
    static void Main(string[] args)
    {
        string sourcePath = @"E:\";
        string destinationPath = @"Z:\SVB\SDs\19";

        // Ensure the source and destination directories exist
        if (!Directory.Exists(sourcePath))
        {
            Console.WriteLine("Source directory does not exist.");
            return;
        }

        if (!Directory.Exists(destinationPath))
        {
            Directory.CreateDirectory(destinationPath);
        }

        // Get all files from the source directory, skipping restricted paths
        var files = GetFiles(sourcePath);

        // Calculate the total size of files to be copied
        long totalSize = files.Sum(file => new FileInfo(file).Length);
        Console.WriteLine($"Total size of files to be copied: {FormatSize(totalSize)}");

        // Prompt user to continue
        Console.WriteLine("Do you want to proceed with copying? (Y/N)");
        if (Console.ReadKey().Key != ConsoleKey.Y)
        {
            Console.WriteLine("\nCopy operation canceled.");
            return;
        }

        Console.WriteLine("\nCopying files...");

        for(var i = 0; i < files.Count; i++)
        {
            var file = files[i];
            try
            {
                string relativePath = file.Substring(sourcePath.Length + 1);
                string destinationFile = Path.Combine(destinationPath, relativePath);

                // Ensure the destination directory exists
                Directory.CreateDirectory(Path.GetDirectoryName(destinationFile));

                if (File.Exists(destinationFile))
                {
                    if (new FileInfo(file).Length == new FileInfo(destinationFile).Length)
                    {
                        // File already exists and is the same size, skip copying
                        Console.WriteLine($"{i}/{files.Count}. Skipping file (exists and same size): {relativePath}");
                        continue;
                    }
                }

                Console.WriteLine($"{i}/{files.Count}. Copying file: {relativePath}");

                // Copy the file
                File.Copy(file, destinationFile, true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error copying file: {file}. Error: {ex.Message}");
            }
        }

        Console.WriteLine("Copy operation completed.");
    }

    static List<string> GetFiles(string rootPath)
    {
        var files = new List<string>();
        try
        {
            files.AddRange(Directory.GetFiles(rootPath, "*.*", SearchOption.TopDirectoryOnly));

            foreach (var directory in Directory.GetDirectories(rootPath))
            {
                try
                {
                    files.AddRange(GetFiles(directory));
                }
                catch (UnauthorizedAccessException)
                {
                    Console.WriteLine($"Access denied to directory: {directory}");
                }
                catch (PathTooLongException)
                {
                    Console.WriteLine($"Path too long: {directory}");
                }
            }
        }
        catch (UnauthorizedAccessException)
        {
            Console.WriteLine($"Access denied to directory: {rootPath}");
        }
        catch (PathTooLongException)
        {
            Console.WriteLine($"Path too long: {rootPath}");
        }

        return files;
    }

    static string FormatSize(long size)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        double len = size;
        int order = 0;
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len /= 1024;
        }
        return $"{len:0.##} {sizes[order]}";
    }
}
