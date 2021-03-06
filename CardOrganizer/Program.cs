using CommandLine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CardOrganizer
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                Console.WriteLine(e.ExceptionObject);
                Exit();
            };

            Config.Init();
            Parser.Default.ParseArguments<Arguments>(args).WithParsed(RunWithOptions).WithNotParsed(x => Exit());
        }

        private static void RunWithOptions(Arguments args)
        {
            var targetFolder = string.IsNullOrWhiteSpace(args.TargetFolder) ? Config.Default.TargetFolder : args.TargetFolder;
            var searchSub = args.SearchSubfolders ?? Config.Default.SearchSubfolders;
            
            if(!Directory.Exists(targetFolder))
            {
                Console.WriteLine("Target folder does not exist");
                Exit();
            }

            Console.WriteLine($"Organizing cards found in the specified folder. ({targetFolder})");
            Console.WriteLine($"Subfolders will be {(searchSub ? "searched as well" : "ignored")}.");
            if(args.TestRun) Console.WriteLine("No files will be moved because test mode is enabled.");
            Console.WriteLine();

            var searchOption = searchSub ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            var pngFiles = Directory.EnumerateFiles(targetFolder, "*", searchOption).Where(x => x.EndsWith(".png", StringComparison.OrdinalIgnoreCase)).ToList();
            var filesToMove = new List<Tuple<string, string>>();

            if(pngFiles.Count > 0)
            {
                foreach(var filepath in pngFiles)
                {
                    var fileString = File.ReadAllText(filepath, Encoding.UTF8);
                    var tokenData = TokenData.CardData.Find(fileString).LastOrDefault(); // Last has to be taken because scene token is last, reverse read order?

                    if(tokenData != null)
                    {
                        var destination = Path.Combine(tokenData.Item1.GetFolder(fileString, tokenData.Item2), Path.GetFileName(filepath));

                        if(!IsFullPath(destination))
                            destination = Path.Combine(Config.Default.UseWorkingDir ? Directory.GetCurrentDirectory() : targetFolder, destination);

                        filesToMove.Add(Tuple.Create(filepath, destination));
                        Console.WriteLine($"{Path.GetFileName(filepath)} = {tokenData.Item1.Token}");
                    }
                }

                if(!args.TestRun && filesToMove.Count > 0)
                    FileOperation.Move(filesToMove);
            }

            if(filesToMove.Count == 0)
                Console.WriteLine("No cards were found in the target folder.");

            Console.WriteLine();
            Exit();
        }

        private static void Exit()
        {
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
            Environment.Exit(0);
        }

        private static bool IsFullPath(string path)
        {
            return !string.IsNullOrWhiteSpace(path)
                && path.IndexOfAny(Path.GetInvalidPathChars().ToArray()) == -1
                && Path.IsPathRooted(path)
                && !Path.GetPathRoot(path).Equals(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal);
        }
    }
}
