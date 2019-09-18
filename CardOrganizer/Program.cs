using CommandLine;
using Nett;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CardOrganizer
{
    internal class Program
    {
        private const string ConfigFile = "config.toml";
        private static Config Config;
        private static Arguments Arguments;

        private static void Main(string[] args)
        {
            if(File.Exists(ConfigFile))
            {
                Config = Toml.ReadFile<Config>(ConfigFile);
            }
            else
            {
                Config = new Config();
                Toml.WriteFile(Config, ConfigFile);

                Console.WriteLine("A new config file has been created.");
                Console.WriteLine("This program uses a TOML config file, any text editor should be able to edit it.");
                Console.WriteLine("Press ESC to quit and edit the config or press any other key to continue with the default config.");

                if(Console.ReadKey().Key == ConsoleKey.Escape)
                    Environment.Exit(0);

                Console.WriteLine();
            }

            Parser.Default.ParseArguments<Arguments>(args)
                .WithParsed(RunWithOptions).WithNotParsed(RunWithError);
        }

        private static void RunWithOptions(Arguments args)
        {
            Arguments = args;

            if(!Directory.Exists(Arguments.TargetFolder))
            {
                Console.WriteLine("TargetFolder does not exist");
                Exit();
            }

            Console.WriteLine($"Organizing cards found in the specified folder. ({Arguments.TargetFolder})");
            Console.WriteLine($"Subfolders are {(Arguments.SearchSubfolders ? "searched as well" : "ignored")}.");
            Console.WriteLine();

            var searchOption = Arguments.SearchSubfolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            var files = Directory.EnumerateFiles(Arguments.TargetFolder, "*", searchOption).Where(x => x.EndsWith(".png", StringComparison.OrdinalIgnoreCase)).ToList();

            if(files.Count > 0)
            {
                foreach(var file in files)
                {
                    var bytes = File.ReadAllBytes(file);

                    if(BoyerMoore.ContainsSequence(bytes, CardConstants.KoikatuStudioToken))
                    {
                        var newPath = Path.Combine(Config.KoikatuFolder, CardConstants.SceneCategory, Path.GetFileName(file));
                        FileMove(file, newPath);
                        Console.WriteLine(newPath);
                    }
                    else if(BoyerMoore.ContainsSequence(bytes, CardConstants.KoikatuCharaToken))
                    {
                        var index = new BoyerMoore(CardConstants.SexToken).Search(bytes).First();
                        var sex = bytes[index + CardConstants.SexToken.Length];
                        var folder = sex == 0 ? CardConstants.MaleCategory : CardConstants.FemaleCategory;
                        var newPath = Path.Combine(Config.KoikatuFolder, folder, Path.GetFileName(file));
                        FileMove(file, newPath);
                        Console.WriteLine(newPath);
                    }
                    else if(BoyerMoore.ContainsSequence(bytes, CardConstants.KoikatuOutfitToken))
                    {
                        var newPath = Path.Combine(Config.KoikatuFolder, CardConstants.OutfitCategory, Path.GetFileName(file));
                        FileMove(file, newPath);
                        Console.WriteLine(newPath);
                    }
                }
            }
            else
            {
                Console.WriteLine("No cards were found in the target folder");
            }

            Console.WriteLine();
            Exit();
        }

        private static void RunWithError(IEnumerable<Error> errors)
        {

        }

        private static void FileMove(string srcFileName, string destFileName)
        {
            if(!Arguments.TestRun)
            {
                var fullPath = Path.GetFullPath(destFileName);
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
                File.Move(srcFileName, fullPath);
            }
        }

        private static void Exit()
        {
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
            Environment.Exit(0);
        }
    }
}
