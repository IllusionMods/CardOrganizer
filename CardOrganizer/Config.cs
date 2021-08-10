using Nett;
using System;
using System.Diagnostics;
using System.IO;

namespace CardOrganizer
{
    public class Config
    {
        private const string CONFIG_FILENAME = "cardorganizer.toml";
        private const string DEFAULT_TARGET_DIR = "cardorganizer";

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
                if(File.Exists(CONFIG_FILENAME))
                {
                    _default = Toml.ReadFile<Config>(CONFIG_FILENAME);
                    _default.ConfigPath = CONFIG_FILENAME;
                }
                else
                {
                    var configPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), CONFIG_FILENAME);
                    
                    if(File.Exists(configPath))
                    {
                        _default = Toml.ReadFile<Config>(configPath);
                        _default.ConfigPath = configPath;
                    }
                    else
                    {
                        _default = new Config();
                        Toml.WriteFile(_default, configPath);
                        _default.ConfigPath = configPath;

                        Console.WriteLine("A new config file has been created.");
                        Console.WriteLine("This program uses a TOML config file, any text editor should be able to edit it.");
                        Console.WriteLine("Open config file in default text editor? (y/n)");
                        if(Console.ReadLine() is "y" or "yes")
                            _default.ShellOpen();
                        
                        Program.Exit();
                    }
                }
            }
        }

        public void ShellOpen()
        {
            new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    UseShellExecute = true,
                    FileName = ConfigPath
                }
            }.Start();
            
            Program.Exit();
        }

        public string ConfigPath;

        [TomlMember, TomlComment("Search this folder for cards to organize. Arguments override this setting.")]
        public string TargetFolder { get; set; } = "";

        [TomlMember, TomlComment("Seach target folder subfolders for cards. Arguments override this setting.")]
        public bool SearchSubfolders { get; set; } = false;

        [TomlMember, TomlComment("Use a common output folder for all the games. If true all other folders are ignored.")]
        public bool UseCommonFolder { get; set; } = true;

        [TomlMember, TomlComment("The folder used if UseCommonFolder is true.")]
        public string CommonFolderPath { get; set; } = DEFAULT_TARGET_DIR;

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
