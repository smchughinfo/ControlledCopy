using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlledCopy
{
    internal class Program
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

            // Get all files from the source directory
            var files = Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories);

            // Calculate the total size of files to be copied
            long totalSize = files.Sum(file => new FileInfo(file).Length);
            Console.WriteLine($"Total size of files to be copied: {totalSize / (1024 * 1024)} MB");

            // Prompt user to continue
            Console.WriteLine("Do you want to proceed with copying? (Y/N)");
            if (Console.ReadKey().Key != ConsoleKey.Y)
            {
                Console.WriteLine("\nCopy operation canceled.");
                return;
            }

            Console.WriteLine("\nCopying files...");

            foreach (var file in files)
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
                        Console.WriteLine($"Skipping file (exists and same size): {relativePath}");
                        continue;
                    }
                }

                // Copy the file
                File.Copy(file, destinationFile, true);
                Console.WriteLine($"Copied file: {relativePath}");
            }

            Console.WriteLine("Copy operation completed.");
        }
    }
}
