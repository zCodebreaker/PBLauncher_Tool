
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;

namespace Launcher_Tool
{
    internal class Program
    {
        static List<(string FileName, string MD5)> FileList = new List<(string, string)>();
        static void Main()
        {
            Console.Title = "UserFileList Generator";
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("U S E R F I L E L I S T - G E N E R A T O R");
            Console.WriteLine();
            Console.WriteLine("by Codebreaker");
            Console.WriteLine();
            Console.WriteLine("##::::'##::'######::'########:'########::'########:'####:'##:::::::'########:'##:::::::'####::'######::'########:");
            Console.WriteLine("##:::: ##:'##... ##: ##.....:: ##.... ##: ##.....::. ##:: ##::::::: ##.....:: ##:::::::. ##::'##... ##:... ##..::");
            Console.WriteLine("##:::: ##: ##:::..:: ##::::::: ##:::: ##: ##:::::::: ##:: ##::::::: ##::::::: ##:::::::: ##:: ##:::..::::: ##::::");
            Console.WriteLine("##:::: ##:. ######:: ######::: ########:: ######:::: ##:: ##::::::: ######::: ##:::::::: ##::. ######::::: ##::::");
            Console.WriteLine("##:::: ##::..... ##: ##...:::: ##.. ##::: ##...::::: ##:: ##::::::: ##...:::: ##:::::::: ##:::..... ##:::: ##::::");
            Console.WriteLine("##:::: ##:'##::: ##: ##::::::: ##::. ##:: ##:::::::: ##:: ##::::::: ##::::::: ##:::::::: ##::'##::: ##:::: ##::::");
            Console.WriteLine(".########::.######:: ########: ##:::. ##: ##:::::::'####: ########: ########: ########:'####:. ######::::: ##::::");
            Console.WriteLine();
            Console.WriteLine("Drop Point Blank client here ");
            string sourcePath = Console.ReadLine().Trim('"');
            if (!Directory.Exists(sourcePath))
            {
                Console.WriteLine("Client not found.");
                Console.ReadKey();
                return;
            }
            string outputPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Client");

            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }
            ProcessDirectory(sourcePath, outputPath, sourcePath);

            XElement xml = new XElement("list",
                FileList.ConvertAll(f =>
                    new XElement("file",
                        new XAttribute("n", f.FileName),
                        new XAttribute("m", f.MD5)
                    )
                )
            );
            string xmlPath = Path.Combine(outputPath, "UserFileList.dat");
            xml.Save(xmlPath);

            Console.WriteLine("Process completed.\r\n");
            Console.ReadKey();
        }
        static void ProcessDirectory(string sourceDir, string targetDir, string baseDir)
        {
            Directory.CreateDirectory(targetDir);
            foreach (string filePath in Directory.GetFiles(sourceDir))
            {
                ProcessFile(filePath, targetDir, baseDir);
            }
            foreach (string subDir in Directory.GetDirectories(sourceDir))
            {
                string newTargetDir = Path.Combine(targetDir, Path.GetFileName(subDir));
                ProcessDirectory(subDir, newTargetDir, baseDir);
            }
        }
        static void ProcessFile(string filePath, string targetDir, string baseDir)
        {
            string relativePath = filePath.Substring(baseDir.Length).Replace(Path.DirectorySeparatorChar, '/');
            string md5Hash = CalculateMD5(filePath);
            FileList.Add((relativePath, md5Hash));
            string zipPath = Path.Combine(targetDir, Path.GetFileName(filePath) + ".zip");
            using (FileStream zipStream = new FileStream(zipPath, FileMode.Create))
            {
                using (ZipArchive archive = new ZipArchive(zipStream, ZipArchiveMode.Create))
                {
                    archive.CreateEntryFromFile(filePath, Path.GetFileName(filePath));
                }
            }
            Console.WriteLine($"File: {filePath}");
        }
        static string CalculateMD5(string filePath)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filePath))
                {
                    byte[] hash = md5.ComputeHash(stream);
                    StringBuilder sb = new StringBuilder();
                    foreach (byte b in hash)
                    {
                        sb.Append(b.ToString("x2"));
                    }
                    return sb.ToString();
                }
            }
        }
    }
}
