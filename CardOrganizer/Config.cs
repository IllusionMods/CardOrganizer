using Nett;
using System;
using System.IO;

namespace CardOrganizer
{
    internal class Config
    {
        private const string ConfigFile = "config.toml";
        private const string DefaultFolder = "cardorganizer";

        private static Config _default;
        public static Config Default
        {
            get
            {
                Init();
                return _default;
            }
        }

        public static void Init()
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
        }

        [TomlMember, TomlComment("Search this folder for cards to organize. Arguments override this setting.")]
        public string TargetFolder { get; set; } = "";

        [TomlMember, TomlComment("Seach target folder subfolders for cards. Arguments override this setting.")]
        public bool SearchSubfolders { get; set; } = false;

        [TomlMember, TomlComment("Use a common output folder for all the games. If this setting is true all the other folders are ignored.")]
        public bool UseCommonFolder { get; set; } = true;

        [TomlMember, TomlComment("The folder used if UseCommonFolder is true.")]
        public string CommonFolderPath { get; set; } = DefaultFolder;

        [TomlMember, TomlComment("Use current working directory for relative paths.")]
        public bool UseWorkingDir { get; set; } = false;

        [TomlMember, TomlComment("Koikatu folders")]
        public string KoikatuFemaleFolder { get; set; } = "";
        [TomlMember]
        public string KoikatuMaleFolder { get; set; } = "";
        [TomlMember]
        public string KoikatuSceneFolder { get; set; } = "";
        [TomlMember]
        public string KoikatuOutfitFolder { get; set; } = "";

        [TomlMember, TomlComment("PlayHome folders")]
        public string PlayHomeFemaleFolder { get; set; } = "";
        [TomlMember]
        public string PlayHomeMaleFolder { get; set; } = "";
        [TomlMember]
        public string PlayHomeSceneFolder { get; set; } = "";

        [TomlMember, TomlComment("Honey Select folders")]
        public string HoneySelectSceneFolder { get; set; } = "";
        [TomlMember]
        public string HoneySelectFemaleFolder { get; set; } = "";
        [TomlMember]
        public string HoneySelectMaleFolder { get; set; } = "";

        [TomlMember, TomlComment("PlayHome folders")]
        public string EmotionCreatorsFemaleFolder { get; set; } = "";
        [TomlMember]
        public string EmotionCreatorsMaleFolder { get; set; } = "";
        [TomlMember]
        public string EmotionCreatorsMapFolder { get; set; } = "";
        [TomlMember]
        public string EmotionCreatorsPoseFolder { get; set; } = "";
        [TomlMember]
        public string EmotionCreatorsHSceneFolder { get; set; } = "";

        [TomlMember, TomlComment("Sexy Beach Premium Resort folders")]
        public string SexyBeachPremiumFemaleFolder { get; set; } = "";
        [TomlMember]
        public string SexyBeachPremiumMaleFolder { get; set; } = "";

        [TomlMember, TomlComment("AI Shoujo folders")]
        public string AIShoujoFemaleFolder { get; set; } = "";
        [TomlMember]
        public string AIShoujoMaleFolder { get; set; } = "";
        [TomlMember]
        public string AIShoujoSceneFolder { get; set; } = "";
        [TomlMember]
        public string AIShoujoHousingFolder { get; set; } = "";
        [TomlMember]
        public string AIShoujoOutfitFolder { get; set; } = "";
    }
}
