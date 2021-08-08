using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace CardOrganizer
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
#if DEBUG
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                Console.WriteLine(e.ExceptionObject);
                Exit();
            };
#endif

            Config.Init();

            var testRun = args.Length >= 1 && args[0] == "--testrun";
            var targetFolder = Config.Default.TargetFolder;
            var searchSub = Config.Default.SearchSubfolders;
            
            if(!Directory.Exists(targetFolder))
            {
                Console.WriteLine("Target folder does not exist");
                Exit();
            }

            Console.WriteLine($"Organizing cards found in the specified folder. ({targetFolder})");
            Console.WriteLine($"Subfolders will be {(searchSub ? "searched as well" : "ignored")}.");
            if(testRun) Console.WriteLine("No files will be moved because test mode is enabled.");
            Console.WriteLine();

            var searchOption = searchSub ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            var pngFiles = Directory.EnumerateFiles(targetFolder, "*", searchOption).Where(x => x.EndsWith(".png", StringComparison.OrdinalIgnoreCase)).ToList();
            var filesToMove = new List<Tuple<string, string>>();

            if(pngFiles.Count > 0)
            {
                foreach(var filepath in pngFiles)
                {
                    var fileString = File.ReadAllText(filepath, Encoding.UTF8);
                    var tokenData = TokenData.CardData.Find(fileString).LastOrDefault(); // TODO: last has to be taken because scene token is last, reverse read order?

                    if(tokenData != null)
                    {
                        var destination = Path.Combine(tokenData.Item1.GetFolder(fileString, tokenData.Item2), Path.GetFileName(filepath));

                        if(!IsFullPath(destination))
                            destination = Path.Combine(Config.Default.UseWorkingDir ? Directory.GetCurrentDirectory() : targetFolder, destination);

                        filesToMove.Add(Tuple.Create(filepath, destination));
                        Console.WriteLine($"{Path.GetFileName(filepath)} = {tokenData.Item1.Token}");
                    }
                }

                if(!testRun && filesToMove.Count > 0)
                {
                    if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        WinAPI.MoveFiles(filesToMove);
                    }
                    else
                    {
                        foreach(var (source, dest) in filesToMove)
                        {
                            var destInfo = new FileInfo(dest);
                            var srcInfo = new FileInfo(source);
                            
                            if(destInfo.Exists && srcInfo.Name != destInfo.Name && srcInfo.Length != destInfo.Length)
                            {
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
