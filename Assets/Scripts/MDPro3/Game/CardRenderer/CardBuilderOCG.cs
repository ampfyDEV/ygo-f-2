using MDPro3.Duel.YGOSharp;
using MDPro3.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MDPro3
{
    public class CardBuilderOCG : CardBuilder
    {
        [Header("OCG")]

        [SerializeField] private RawImage imageArt;
        [SerializeField] private RawImage imageArtPendulum;
        [SerializeField] private RawImage imageArtPendulumSquare;
        [SerializeField] private RawImage imageArtPendulumWide;
        [SerializeField] private TextMeshProUGUI tmpCardType2;

        [SerializeField] private GameObject levels;
        [SerializeField] private GameObject ranks;
        [SerializeField] private GameObject ranks13;
        [SerializeField] private GameObject levelMasks;
        [SerializeField] private GameObject rankMasks;
        [SerializeField] private GameObject rank13Masks;
        [SerializeField] private GameObject linkMarkers;
        [SerializeField] private GameObject line;
        [SerializeField] private GameObject atk;
        [SerializeField] private GameObject def;
        [SerializeField] private Text numAtk;
        [SerializeField] private Text numDef;
        [SerializeField] private Image imageLinkCount;

        [SerializeField] private RectTransform rectMonster;
        [SerializeField] private RectTransform rectSpell;

        protected override void SwitchLanguage(string language = null)
        {
            base.SwitchLanguage(language);

            if (Language.UseLatin(language))
            {
                tmpCardType2.fontSize = 28f;
                tmpCardType2.transform.localScale = new Vector3(1.2f, 1f, 1f);
                tmpCardType2.fontStyle = (FontStyles)((int)FontStyles.Bold + (int)FontStyles.SmallCaps);
            }
            else
            {
                tmpCardType2.fontSize = 26f;
                tmpCardType2.transform.localScale = Vector3.one;
                tmpCardType2.fontStyle = FontStyles.Normal;
            }
        }

        protected override void SetFont(TMP_FontAsset tmpFont)
        {
            base.SetFont(tmpFont);
            tmpCardType2.font = tmpFont;
        }

        protected override void ShowNameOnlyParts()
        {
            base.ShowNameOnlyParts();

            tmpCardType2.text = string.Empty;
            imageArt.gameObject.SetActive(false);
            imageArtPendulum.gameObject.SetActive(false);
            imageArtPendulumSquare.gameObject.SetActive(false);
            imageArtPendulumWide.gameObject.SetActive(false);
            levels.SetActive(false);
            ranks.SetActive(false);
            ranks13.SetActive(false);
            levelMasks.SetActive(false);
            rankMasks.SetActive(false);
            rank13Masks.SetActive(false);
            linkMarkers.SetActive(false);
            tmpCardType.text = string.Empty;

            line.SetActive(false);
            atk.SetActive(false);
            def.SetActive(false);
            numAtk.text = string.Empty;
            numDef.text = string.Empty;
            imageLinkCount.gameObject.SetActive(false);
            tmpCardType.text = string.Empty;
        }

        public override void SetCardName(Card data, string language)
        {
            base.SetCardName(data, language);

            if (data.IsLevelZeroMonster())
                data.Level = 0;
            if (data.HasType(CardType.Xyz))
            {
                if (data.Level == 13)
                    rank13Masks.SetActive(true);
                else
                {
                    rankMasks.SetActive(true);
                    for (int i = 0; i < 12; i++)
                        rankMasks.transform.GetChild(i).gameObject.SetActive(i < data.Level);
                }
            }
            else if (data.HasType(CardType.Monster)
                && !data.HasType(CardType.Link))
            {
                levelMasks.SetActive(true);
                for (int i = 0; i < 12; i++)
                    levelMasks.transform.GetChild(i).gameObject.SetActive(i < data.Level);
            }
        }

        public override void SetCard(Card data, string language, Texture art, Texture2D overFrame = null)
        {
            base.SetCard(data, language, art, overFrame);

            levelMasks.SetActive(false);
            rankMasks.SetActive(false);
            rank13Masks.SetActive(false);

            line.SetActive(true);
            atk.SetActive(true);
            def.SetActive(true);
            numAtk.text = data.GetAttackString();
            numDef.text = data.GetDefenseString();
            tmpDescription.rectTransform.sizeDelta = rectMonster.sizeDelta;
            tmpDescription.rectTransform.anchoredPosition = rectMonster.anchoredPosition;
            imageAttr.sprite = TextureManager.container.GetCardAttributeIcon(data, true);

            if (data.HasType(CardType.Pendulum))
            {
                if(art.width == art.height)
                {
                    imageArtPendulumSquare.gameObject.SetActive(true);
                    imageArtPendulumSquare.texture = art;
                }
                else if(art.width > art.height)
                {
                    imageArtPendulumWide.gameObject.SetActive(true);
                    imageArtPendulumWide.texture = art;
                }
                else
                {
                    imageArtPendulum.gameObject.SetActive(true);
                    imageArtPendulum.texture = art;
                }

                var pendulumDesc = data.GetDescriptionSplit(true);
                tmpCardType2.text = data.GetTypeForRushDuelRender();
                tmpDescriptionPendulum.text = TextForRender(pendulumDesc[0], data);

                var authorSplit = GetAuthorFromDescription(pendulumDesc[1]);
                tmpDescription.text = TextForRender(authorSplit[0], data);
                tmpAuther.text = authorSplit[1];

                textLScale.text = data.LScale.ToString();
                textRScale.text = data.RScale.ToString();
            }
            else
            {
                imageArt.gameObject.SetActive(true);
                imageArt.texture = art;

                if(!data.HasAnyType(CardType.Spell, CardType.Trap))
                    tmpCardType2.text = data.GetTypeForRushDuelRender();
                var authorSplit = GetAuthorFromDescription(data.Desc);
                tmpDescription.text = TextForRender(authorSplit[0], data);
                tmpAuther.text = authorSplit[1];
            }

            if (data.IsLevelZeroMonster())
                data.Level = 0;
            if (data.HasType(CardType.Link))
            {
                tmpCardName.color = Color.white;
                linkMarkers.SetActive(true);
                def.SetActive(false);
                numDef.text = string.Empty;
                imageLinkCount.gameObject.SetActive(true);
                imageLinkCount.sprite = TextureManager.container.GetOcgLinkCount(data.GetLinkCount());
                for (int i = 0; i < 8; i++)
                {
                    int bitIndex = i < 4 ? i : i + 1;
                    linkMarkers.transform.GetChild(i).gameObject.SetActive((data.LinkMarker & (1 << bitIndex)) != 0);
                }
            }
            else if (data.HasType(CardType.Xyz))
            {
                tmpCardName.color = Color.white;
                if (!data.HasType(CardType.Pendulum))
                {
                    textPassword.color = Color.white;
                    tmpAuther.color = Color.white;
                }

                if(data.Level == 13)
                    ranks13.SetActive(true);
                else
                {
                    ranks.SetActive(true);
                    for (int i = 0; i < 12; i++)
                        ranks.transform.GetChild(i).gameObject.SetActive(i < data.Level);
                }
            }
            else if (data.HasType(CardType.Monster))
            {
                levels.SetActive(true);
                for (int i = 0; i < 12; i++)
                    levels.transform.GetChild(i).gameObject.SetActive(i < data.Level);
            }
            else if (data.HasAnyType(CardType.Spell, CardType.Trap))
            {
                tmpDescription.rectTransform.sizeDelta = rectSpell.sizeDelta;
                tmpDescription.rectTransform.anchoredPosition = rectSpell.anchoredPosition;
                tmpCardName.color = Color.white;
                line.SetActive(false);
                atk.SetActive(false);
                def.SetActive(false);
                numAtk.text = string.Empty;
                numDef.text = string.Empty;
                tmpCardType.text = data.GetSpellTypeForOCGRender();
            }
        }

        public override RawImage GetArtPartForVideo(bool isPendulum)
        {
            return isPendulum ? imageArtPendulum : imageArt;
        }

        public override void SetAllArtPartsOff()
        {
            imageArt.gameObject.SetActive(false);
            imageArtPendulum.gameObject.SetActive(false);
            imageArtPendulumSquare.gameObject.SetActive(false);
            imageArtPendulumWide.gameObject.SetActive(false);
        }
    }
}