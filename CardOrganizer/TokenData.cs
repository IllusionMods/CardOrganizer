using System;
using System.IO;

namespace CardOrganizer
{
    public class TokenData
    {
        public string Token { get; set; }
        public CardType CardType { get; set; }
        public Func<byte, string> GetFolder { get; set; }

        public static TokenData CreateResolvedToken(string token, string folder, string gameCategory, string cardCategory)
        {
            return new TokenData
            {
                Token = token,
                CardType = CardType.Resolved,
                GetFolder = (x) => Config.Default.UseCommonFolder || string.IsNullOrWhiteSpace(folder) ? Path.Combine(Config.Default.CommonFolder, gameCategory, cardCategory) : folder
            };
        }

        public static TokenData CreateUnknownSexToken(string token, string gameCategory, string femaleFolder, string maleFolder)
        {
            return new TokenData
            {
                Token = token,
                CardType = CardType.UnknownSex,
                GetFolder = (x) => Config.Default.UseCommonFolder || string.IsNullOrWhiteSpace(femaleFolder) || string.IsNullOrWhiteSpace(maleFolder)
                    ? Path.Combine(Config.Default.CommonFolder, gameCategory, x == 0 ? CardConstants.MaleCategory : CardConstants.FemaleCategory) : x == 0 ? maleFolder : femaleFolder
            };
        }

        private static Trie<TokenData> cardData;
        public static Trie<TokenData> CardData
        {
            get
            {
                if(cardData == null)
                {
                    cardData = new Trie<TokenData>();

                    cardData.Add(CardConstants.KoikatuSceneToken, CreateResolvedToken(CardConstants.KoikatuSceneToken, Config.Default.KoikatuSceneFolder, CardConstants.KoikatuCategory, CardConstants.SceneCategory));
                    cardData.Add(CardConstants.KoikatuCharaToken, CreateUnknownSexToken(CardConstants.KoikatuCharaToken, CardConstants.KoikatuCategory, Config.Default.KoikatuFemaleFolder, Config.Default.KoikatuMaleFolder));
                    cardData.Add(CardConstants.KoikatuOutfitToken, CreateResolvedToken(CardConstants.KoikatuOutfitToken, Config.Default.KoikatuOutfitFolder, CardConstants.KoikatuCategory, CardConstants.OutfitCategory));

                    cardData.Add(CardConstants.PlayHomeSceneToken, CreateResolvedToken(CardConstants.PlayHomeSceneToken, Config.Default.PlayHomeSceneFolder, CardConstants.PlayHomeCategory, CardConstants.SceneCategory));
                    cardData.Add(CardConstants.PlayHomeFemaleToken, CreateResolvedToken(CardConstants.PlayHomeFemaleToken, Config.Default.PlayHomeFemaleFolder, CardConstants.PlayHomeCategory, CardConstants.FemaleCategory));
                    cardData.Add(CardConstants.PlayHomeMaleToken, CreateResolvedToken(CardConstants.PlayHomeMaleToken, Config.Default.PlayHomeMaleFolder, CardConstants.PlayHomeCategory, CardConstants.MaleCategory));

                    cardData.Add(CardConstants.HoneySelectSceneToken, CreateResolvedToken(CardConstants.HoneySelectSceneToken, Config.Default.HoneySelectSceneFolder, CardConstants.HoneySelectCategory, CardConstants.SceneCategory));
                    cardData.Add(CardConstants.HoneySelectFemaleToken, CreateResolvedToken(CardConstants.HoneySelectFemaleToken, Config.Default.HoneySelectFemaleFolder, CardConstants.HoneySelectCategory, CardConstants.FemaleCategory));
                    cardData.Add(CardConstants.HoneySelectMaleToken, CreateResolvedToken(CardConstants.HoneySelectMaleToken, Config.Default.HoneySelectMaleFolder, CardConstants.HoneySelectCategory, CardConstants.MaleCategory));

                    cardData.Add(CardConstants.AICharaToken, CreateUnknownSexToken(CardConstants.AICharaToken, CardConstants.AISyoujyoCategory, Config.Default.AIShoujoFemaleFolder, Config.Default.AIShoujoMaleFolder));
                    cardData.Add(CardConstants.AIOutfitToken, CreateResolvedToken(CardConstants.AIOutfitToken, Config.Default.AIShoujoOutfitFolder, CardConstants.AISyoujyoCategory, CardConstants.OutfitCategory));
                    cardData.Add(CardConstants.AISceneToken, CreateResolvedToken(CardConstants.AISceneToken, Config.Default.AIShoujoSceneFolder, CardConstants.AISyoujyoCategory, CardConstants.SceneCategory));
                    cardData.Add(CardConstants.AIHousingToken, CreateResolvedToken(CardConstants.AIHousingToken, Config.Default.AIShoujoHousingFolder, CardConstants.AISyoujyoCategory, CardConstants.HousingCategory));

                    cardData.Add(CardConstants.SexyBeachPremiumFemaleToken, CreateResolvedToken(CardConstants.SexyBeachPremiumFemaleToken, Config.Default.SexyBeachPremiumFemaleFolder, CardConstants.SexyBeachPremiumCategory, CardConstants.FemaleCategory));
                    cardData.Add(CardConstants.SexyBeachPremiumMaleToken, CreateResolvedToken(CardConstants.SexyBeachPremiumMaleToken, Config.Default.SexyBeachPremiumMaleFolder, CardConstants.SexyBeachPremiumCategory, CardConstants.MaleCategory));

                    cardData.Add(CardConstants.EmotionCreatorsCharaToken, CreateUnknownSexToken(CardConstants.EmotionCreatorsCharaToken, CardConstants.EmotionCreatorsCategory, Config.Default.EmotionCreatorsFemaleFolder, Config.Default.EmotionCreatorsMaleFolder));
                    cardData.Add(CardConstants.EmotionCreatorsHSceneToken, CreateResolvedToken(CardConstants.EmotionCreatorsHSceneToken, Config.Default.EmotionCreatorsHSceneFolder, CardConstants.EmotionCreatorsCategory, CardConstants.Uncategorized));
                    cardData.Add(CardConstants.EmotionCreatorsMapToken, CreateResolvedToken(CardConstants.EmotionCreatorsMapToken, Config.Default.EmotionCreatorsMapFolder, CardConstants.EmotionCreatorsCategory, CardConstants.Uncategorized));
                    cardData.Add(CardConstants.EmotionCreatorsPoseToken, CreateResolvedToken(CardConstants.EmotionCreatorsPoseToken, Config.Default.EmotionCreatorsPoseFolder, CardConstants.EmotionCreatorsCategory, CardConstants.Uncategorized));

                    cardData.Build();
                }

                return cardData;
            }
        }

        private static Trie sexData;
        public static Trie SexData
        {
            get
            {
                if(sexData == null)
                {
                    sexData = new Trie();
                    sexData.Add(CardConstants.SexToken);
                    sexData.Build();
                }

                return sexData;
            }
        }
    }

    public enum CardType
    {
        Resolved,
        UnknownSex,
    }
}
