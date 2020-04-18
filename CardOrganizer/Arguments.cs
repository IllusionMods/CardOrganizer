using CommandLine;

namespace CardOrganizer
{
    internal class Arguments
    {
        [Value(0, MetaName = "targetfolder", Required = true, HelpText = "The cards in this folder will organized")]
        public string TargetFolder { get; set; }

        [Option("subfolders", Default = false, Required = false, HelpText = "Seach subfolders for cards")]
        public bool SearchSubfolders { get; set; }

        [Option("testrun", Default = false, Required = false, HelpText = "Test the program without moving files")]
        public bool TestRun { get; set; }
    }
}
