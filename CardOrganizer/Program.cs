using CommandLine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CardOrganizer
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Arguments>(args).WithParsed(RunWithOptions);
        }

        private static void RunWithOptions(Arguments args)
        {
            if(!Directory.Exists(args.TargetFolder))
            {
                Console.WriteLine("Target folder does not exist");
                Exit();
            }

            Console.WriteLine($"Organizing cards found in the specified folder. ({args.TargetFolder})");
            Console.WriteLine($"Subfolders will be {(args.SearchSubfolders ? "searched as well" : "ignored")}.");
            if(args.TestRun) Console.WriteLine("No files will be moved because test mode is enabled.");
            Console.WriteLine();

            var searchOption = args.SearchSubfolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            var pngFiles = Directory.EnumerateFiles(args.TargetFolder, "*", searchOption).Where(x => x.EndsWith(".png", StringComparison.OrdinalIgnoreCase)).ToList();
            var filesToMove = new List<Tuple<string, string>>();

            if(pngFiles.Count > 0)
            {
                foreach(var filepath in pngFiles)
                {
                    var fileString = File.ReadAllText(filepath, Encoding.UTF8);
                    var tokenData = TokenData.CardData.Find(fileString).LastOrDefault(); // Last has to be taken because scene token is last

                    if(tokenData != null)
                    {
                        string destination;

                        if(tokenData.Item1.CardType == CardType.UnknownSex)
                        {
                            var substring = fileString.Substring(tokenData.Item2);
                            var index = TokenData.SexData.Find(substring).First().Item2;
                            var sexChar = substring[index + 1];
                            var sex = BitConverter.GetBytes(sexChar)[0];
                            destination = Path.Combine(tokenData.Item1.GetFolder(sex), Path.GetFileName(filepath));
                        }
                        else
                        {
                            destination = Path.Combine(tokenData.Item1.GetFolder(0), Path.GetFileName(filepath));
                        }

                        if(!IsFullPath(destination))
                            destination = Path.Combine(args.TargetFolder, destination);
                        
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
