using CommandLine;

namespace CardOrganizer
{
    public class Arguments
    {
        [Option("targetfolder", Required = false, HelpText = "The cards in this folder will organized")]
        public string TargetFolder { get; set; }

        [Option("subfolders", Default = null, Required = false, HelpText = "Seach subfolders for cards")]
        public bool? SearchSubfolders { get; set; }

        [Option("testrun", Default = false, Required = false, HelpText = "Test the program without moving files")]
        public bool TestRun { get; set; }
        
        [Option("config", Default = false, Required = false, HelpText = "Open config in default text editor")]
        public bool OpenConfig { get; set; }
    }
}