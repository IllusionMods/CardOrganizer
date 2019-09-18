using Nett;

namespace CardOrganizer
{
    internal class Config
    {
        [TomlMember]
        [TomlComment("Koikatu cards will be moved to this folder. Supports relative paths.")]
        public string KoikatuFolder { get; set; } = "koikatu";

        [TomlMember]
        [TomlComment("Honey Select cards will be moved to this folder. Supports relative paths.")]
        public string HoneySelectFolder { get; set; } = "honeyselect";

        [TomlMember]
        [TomlComment("PlayHome cards will be moved to this folder. Supports relative paths.")]
        public string PlayHomeFolder { get; set; } = "playhome";

        [TomlMember]
        [TomlComment("AI Syoujyo cards will be moved to this folder. Supports relative paths.")]
        public string AISyoujyoFolder { get; set; } = "aisyoujyo";
    }
}
