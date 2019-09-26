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
            Parser.Default.ParseArguments<Arguments>(args)
                .WithParsed(RunWithOptions).WithNotParsed(RunWithError);
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

            var searchOption = args.SearchSubfolders ? System.IO.SearchOption.AllDirectories : System.IO.SearchOption.TopDirectoryOnly;
            var pngFiles = Directory.EnumerateFiles(args.TargetFolder, "*", searchOption).Where(x => x.EndsWith(".png", StringComparison.OrdinalIgnoreCase)).ToList();

            if(pngFiles.Count > 0)
            {
                var filesToMove = Enumerable.Empty<object>().Select(x => new { source = "", destination = "" }).ToList();

                foreach(var filepath in pngFiles)
                {
                    var fileString = File.ReadAllText(filepath, Encoding.UTF8);
                    var tokenData = TokenData.CardData.Find(fileString).LastOrDefault();

                    if(tokenData != null)
                    {
                        if(tokenData.Item1.CardType == CardType.UnknownSex)
                        {
                            var substring = fileString.Substring(tokenData.Item2);
                            var index = TokenData.SexData.Find(substring).First().Item2;
                            var sexChar = substring[index + 1];
                            var sex = BitConverter.GetBytes(sexChar)[0];
                            var folder = sex == 0 ? CardConstants.MaleCategory : CardConstants.FemaleCategory;

                            var dest = Path.Combine(tokenData.Item1.Folder, folder, Path.GetFileName(filepath));
                            filesToMove.Add(new { source = filepath, destination = dest });
                        }
                        else
                        {
                            var dest = Path.Combine(tokenData.Item1.Folder, Path.GetFileName(filepath));
                            filesToMove.Add(new { source = filepath, destination = dest });
                        }

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"{Path.GetFileName(filepath)} = {tokenData.Item1.Token}");
                        Console.ResetColor();
                    }
                }

                if(!args.TestRun && filesToMove.Count > 0)
                {
                    var sources = filesToMove.Select(x => x.source).ToList();
                    var destinations = filesToMove.Select(x => IsFullPath(x.destination) ? x.destination : Path.Combine(args.TargetFolder, x.destination)).ToList();
                    FileOperation.Move(sources, destinations);
                }
            }
            else
            {
                Console.WriteLine("No cards were found in the target folder.");
            }

            Console.WriteLine();
            Exit();
        }

        private static void RunWithError(IEnumerable<Error> errors)
        {

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
