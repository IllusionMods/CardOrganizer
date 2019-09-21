using Nett;
using System;
using System.IO;

namespace CardOrganizer
{
    internal class Config
    {
        private const string ConfigFile = "config.toml";
        private const string MainFolder = "cardorganizer";

        private static Config _default;
        public static Config Default
        {
            get
            {
                if(_default == null)
                {
                    if(File.Exists(ConfigFile))
                    {
                        _default = Toml.ReadFile<Config>(ConfigFile);
                    }
                    else
                    {
                        _default = new Config();
                        Toml.WriteFile(_default, ConfigFile);

                        Console.WriteLine("A new config file has been created.");
                        Console.WriteLine("This program uses a TOML config file, any text editor should be able to edit it.");
                        Console.WriteLine("Press ESC to quit and edit the config or press any other key to continue with the default config.");

                        if(Console.ReadKey().Key == ConsoleKey.Escape)
                            Environment.Exit(0);

                        Console.WriteLine();
                    } 
                }

                return _default;
            }
        }

        [TomlMember]
        [TomlComment("Use a common folder for all the games. If this setting is true all the other folders are ignored.")]
        public bool UseCommonFolder { get; set; } = true;

        [TomlMember]
        [TomlComment("The folder used if UseCommonFolder is true.")]
        public string CommonFolder { get; set; } = MainFolder;

        [TomlMember]
        [TomlComment("Koikatu cards will be moved to this folder. Supports relative paths.")]
        public string KoikatuFolder { get; set; } = Path.Combine(MainFolder, CardConstants.KoikatuCategory);

        [TomlMember]
        [TomlComment("Honey Select cards will be moved to this folder. Supports relative paths.")]
        public string HoneySelectFolder { get; set; } = Path.Combine(MainFolder, CardConstants.HoneySelectCategory);

        [TomlMember]
        [TomlComment("PlayHome cards will be moved to this folder. Supports relative paths.")]
        public string PlayHomeFolder { get; set; } = Path.Combine(MainFolder, CardConstants.PlayHomeCategory);

        [TomlMember]
        [TomlComment("AI Syoujyo cards will be moved to this folder. Supports relative paths.")]
        public string AISyoujyoFolder { get; set; } = Path.Combine(MainFolder, CardConstants.AISyoujyoCategory);

        [TomlMember]
        [TomlComment("Sexy Beach Premium Resort cards will be moved to this folder. Supports relative paths.")]
        public string SexyBeachPremiumFolder { get; set; } = Path.Combine(MainFolder, CardConstants.SexyBeachPremiumCategory);

        [TomlMember]
        [TomlComment("Emotion Creators cards will be moved to this folder. Supports relative paths.")]
        public string EmotionCreatorsFolder { get; set; } = Path.Combine(MainFolder, CardConstants.EmotionCreatorsCategory);
    }
}
