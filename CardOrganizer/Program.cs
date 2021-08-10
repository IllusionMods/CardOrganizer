using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using CommandLine;

namespace CardOrganizer
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            if(!Debugger.IsAttached)
            {
                AppDomain.CurrentDomain.UnhandledException += (_, e) =>
                {
                    Console.WriteLine(e.ExceptionObject);
                    Exit();
                };
            }

            Parser.Default.ParseArguments<Arguments>(args).WithParsed(RunWithOptions).WithNotParsed(x => Exit());
        }

        private static void RunWithOptions(Arguments args)
        {
            Config.Init();
            
            if(args.OpenConfig)
                Config.Default.ShellOpen();

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
                    var tokenSearch = TokenData.CardData.Find(fileString).LastOrDefault(); // TODO: last has to be taken because scene token is last, reverse read order?

                    if(tokenSearch != null)
                    {
                        var (tokenData, startIndex) = tokenSearch;
                        var destination = Path.Combine(tokenData.GetFolder(fileString, startIndex), Path.GetFileName(filepath));

                        if(!IsFullPath(destination))
                            destination = Path.Combine(Config.Default.UseWorkingDir ? Directory.GetCurrentDirectory() : targetFolder, destination);

                        filesToMove.Add(Tuple.Create(filepath, destination));
                        Console.WriteLine($"{Path.GetFileName(filepath)} = {tokenData.Token}");
                    }
                }

                if(!args.TestRun && filesToMove.Count > 0)
                {
                    if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        WinAPI.MoveFiles(filesToMove);
                    }
                    else
                    {
                        foreach(var (source, dest) in filesToMove)
                        {
                            var srcInfo = new FileInfo(source);
                            var destInfo = new FileInfo(dest);
                            
                            if(destInfo.Exists)
                            {
                                // skip identical files
                                if(srcInfo.Name == destInfo.Name && srcInfo.Length == destInfo.Length)
                                    continue;
                                
                                Console.WriteLine();
                                Console.WriteLine($"{srcInfo.Name} | {BytesToString(srcInfo.Length)} | {srcInfo.LastWriteTime}");
                                Console.WriteLine($"{destInfo.Name} | {BytesToString(destInfo.Length)} | {destInfo.LastWriteTime}");
                                Console.WriteLine("Overwrite file? (y/n)");
                                if(Console.ReadLine() is "y" or "yes")
                                    Move(true);
                            }
                            else
                            {
                                Move(false);
                            }

                            void Move(bool overwrite)
                            {
                                Console.WriteLine($"{source} -> {dest}");
                                destInfo.Directory.Create();
                                srcInfo.MoveTo(dest, overwrite);
                            }
                        }
                    }
                }
            }

            if(filesToMove.Count == 0)
                Console.WriteLine("No cards were found in the target folder.");

            Console.WriteLine();
            Exit();
        }

        public static void Exit()
        {
            if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
            
            Environment.Exit(0);
        }

        private static bool IsFullPath(string path)
        {
            return !string.IsNullOrWhiteSpace(path)
                && path.IndexOfAny(Path.GetInvalidPathChars().ToArray()) == -1
                && Path.IsPathRooted(path)
                && !Path.GetPathRoot(path).Equals(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal);
        }
        
        private static string BytesToString(long byteCount)
        {
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
            if (byteCount == 0)
                return "0" + suf[0];
            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (Math.Sign(byteCount) * num).ToString(CultureInfo.InvariantCulture) + suf[place];
        }
    }
}
