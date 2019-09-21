using CommandLine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualBasic.FileIO;

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
            var files = Directory.EnumerateFiles(args.TargetFolder, "*", searchOption).Where(x => x.EndsWith(".png", StringComparison.OrdinalIgnoreCase)).ToList();

            if(files.Count > 0)
            {
                foreach(var file in files)
                {
                    var fileString = File.ReadAllText(file, Encoding.UTF8);
                    var tokenData = TokenData.CardData.Find(fileString).LastOrDefault();

                    if(tokenData != null)
                    {
                        if(tokenData.Item1.CardType == CardType.UnknownSex)
                        {
                            var substring = fileString.Substring(tokenData.Item2);
                            var index = TokenData.SexData.Find(substring).First().Item2;
                            var testsub = substring.Substring(index);
                            var sexChar = substring[index + 1];
                            var sex = BitConverter.GetBytes(sexChar)[0];
                            var folder = sex == 0 ? CardConstants.MaleCategory : CardConstants.FemaleCategory;

                            var newPath = Path.Combine(tokenData.Item1.Folder, folder, Path.GetFileName(file));
                            FileMove(file, newPath, args, tokenData.Item1);
                        }
                        else
                        {
                            var newPath = Path.Combine(tokenData.Item1.Folder, Path.GetFileName(file));
                            FileMove(file, newPath, args, tokenData.Item1);
                        }
                    }
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

        private static void FileMove(string srcFileName, string destFileName, Arguments args, TokenData tokenData)
        {
            if(!args.TestRun)
            {
                var fullPath = IsFullPath(destFileName) ? destFileName : Path.Combine(args.TargetFolder, destFileName);
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

                if(File.Exists(fullPath))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"{fullPath} already exists.");
                    Console.ResetColor();
                }
                else
                {
                    File.Move(srcFileName, fullPath);
                    //FileSystem.MoveFile(srcFileName, destFileName, false);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"{Path.GetFileName(srcFileName)} = {tokenData.Token}");
                    Console.ResetColor();
                }
            }
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
