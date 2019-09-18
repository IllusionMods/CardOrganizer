using System.Text;

namespace CardOrganizer
{
    internal static class CardConstants
    {
        public const string SceneCategory = "scene";
        public const string FemaleCategory = "female";
        public const string MaleCategory = "male";
        public const string OutfitCategory = "outfit";

        public static readonly byte[] KoikatuStudioToken = Encoding.UTF8.GetBytes("【KStudio】");
        public static readonly byte[] KoikatuCharaToken = Encoding.UTF8.GetBytes("【KoiKatuChara");
        public static readonly byte[] KoikatuOutfitToken = Encoding.UTF8.GetBytes("【KoiKatuClothes】");

        public static readonly byte[] SexToken = Encoding.UTF8.GetBytes("sex");
    }
}
