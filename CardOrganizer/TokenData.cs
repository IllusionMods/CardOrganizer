using System.IO;

namespace CardOrganizer
{
    public class TokenData
    {
        public CardType CardType { get; set; }
        public string Folder { get; set; }
        public string Token { get; set; }

        public TokenData(string token, CardType cardType, string gameFolder, string gameCategory, string cardCategory)
        {
            Token = token;
            CardType = cardType;
            Folder = Config.Default.UseCommonFolder ? Path.Combine(Config.Default.CommonFolder, gameCategory, cardCategory) : Path.Combine(gameFolder, cardCategory);
        }

        private static Trie<TokenData> cardData;
        public static Trie<TokenData> CardData
        {
            get
            {
                if(cardData == null)
                {
                    cardData = new Trie<TokenData>();

                    cardData.Add(CardConstants.KoikatuSceneToken, new TokenData(CardConstants.KoikatuSceneToken, CardType.Resolved, Config.Default.KoikatuFolder, CardConstants.KoikatuCategory, CardConstants.SceneCategory));
                    cardData.Add(CardConstants.KoikatuCharaToken, new TokenData(CardConstants.KoikatuCharaToken, CardType.UnknownSex, Config.Default.KoikatuFolder, CardConstants.KoikatuCategory, ""));
                    cardData.Add(CardConstants.KoikatuOutfitToken, new TokenData(CardConstants.KoikatuOutfitToken, CardType.Resolved, Config.Default.KoikatuFolder, CardConstants.KoikatuCategory, CardConstants.OutfitCategory));

                    cardData.Add(CardConstants.PlayHomeSceneToken, new TokenData(CardConstants.PlayHomeSceneToken, CardType.Resolved, Config.Default.PlayHomeFolder, CardConstants.PlayHomeCategory, CardConstants.SceneCategory));
                    cardData.Add(CardConstants.PlayHomeFemaleToken, new TokenData(CardConstants.PlayHomeFemaleToken, CardType.Resolved, Config.Default.PlayHomeFolder, CardConstants.PlayHomeCategory, CardConstants.FemaleCategory));
                    cardData.Add(CardConstants.PlayHomeMaleToken, new TokenData(CardConstants.PlayHomeMaleToken, CardType.Resolved, Config.Default.PlayHomeFolder, CardConstants.PlayHomeCategory, CardConstants.MaleCategory));

                    cardData.Add(CardConstants.HoneySelectSceneToken, new TokenData(CardConstants.HoneySelectSceneToken, CardType.Resolved, Config.Default.HoneySelectFolder, CardConstants.HoneySelectCategory, CardConstants.SceneCategory));
                    cardData.Add(CardConstants.HoneySelectFemaleToken, new TokenData(CardConstants.HoneySelectFemaleToken, CardType.Resolved, Config.Default.HoneySelectFolder, CardConstants.HoneySelectCategory, CardConstants.FemaleCategory));
                    cardData.Add(CardConstants.HoneySelectMaleToken, new TokenData(CardConstants.HoneySelectMaleToken, CardType.Resolved, Config.Default.HoneySelectFolder, CardConstants.HoneySelectCategory, CardConstants.MaleCategory));

                    cardData.Add(CardConstants.AICharaToken, new TokenData(CardConstants.AICharaToken, CardType.UnknownSex, Config.Default.AISyoujyoFolder, CardConstants.AISyoujyoCategory, ""));
                    cardData.Add(CardConstants.AIOutfitToken, new TokenData(CardConstants.AIOutfitToken, CardType.Resolved, Config.Default.AISyoujyoFolder, CardConstants.AISyoujyoCategory, CardConstants.OutfitCategory));
                    cardData.Add(CardConstants.AISceneToken, new TokenData(CardConstants.AISceneToken, CardType.Resolved, Config.Default.AISyoujyoFolder, CardConstants.AISyoujyoCategory, CardConstants.SceneCategory));
                    cardData.Add(CardConstants.AIHousingToken, new TokenData(CardConstants.AIHousingToken, CardType.Resolved, Config.Default.AISyoujyoFolder, CardConstants.AISyoujyoCategory, CardConstants.HousingCategory));

                    cardData.Add(CardConstants.SexyBeachPremiumFemaleToken, new TokenData(CardConstants.SexyBeachPremiumFemaleToken, CardType.Resolved, Config.Default.SexyBeachPremiumFolder, CardConstants.SexyBeachPremiumCategory, CardConstants.FemaleCategory));
                    cardData.Add(CardConstants.SexyBeachPremiumMaleToken, new TokenData(CardConstants.SexyBeachPremiumMaleToken, CardType.Resolved, Config.Default.SexyBeachPremiumFolder, CardConstants.SexyBeachPremiumCategory, CardConstants.MaleCategory));

                    cardData.Add(CardConstants.EmotionCreatorsCharaToken, new TokenData(CardConstants.EmotionCreatorsCharaToken, CardType.UnknownSex, Config.Default.EmotionCreatorsFolder, CardConstants.EmotionCreatorsCategory, ""));
                    cardData.Add(CardConstants.EmotionCreatorsHSceneToken, new TokenData(CardConstants.EmotionCreatorsHSceneToken, CardType.Resolved, Config.Default.EmotionCreatorsFolder, CardConstants.EmotionCreatorsCategory, CardConstants.Uncategorized));
                    cardData.Add(CardConstants.EmotionCreatorsMapToken, new TokenData(CardConstants.EmotionCreatorsMapToken, CardType.Resolved, Config.Default.EmotionCreatorsFolder, CardConstants.EmotionCreatorsCategory, CardConstants.Uncategorized));
                    cardData.Add(CardConstants.EmotionCreatorsPoseToken, new TokenData(CardConstants.EmotionCreatorsPoseToken, CardType.Resolved, Config.Default.EmotionCreatorsFolder, CardConstants.EmotionCreatorsCategory, CardConstants.Uncategorized));

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
