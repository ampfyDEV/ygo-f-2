using MDPro3.Duel.YGOSharp;
using MDPro3.Utility;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MDPro3
{
    public abstract class CardBuilder : MonoBehaviour
    {
        [Header("Common")]
        [SerializeField] protected TextMeshProUGUI tmpDescription;
        [SerializeField] protected TextMeshProUGUI tmpDescriptionPendulum;
        [SerializeField] protected TextMeshProUGUI tmpCardName;
        [SerializeField] protected TextMeshProUGUI tmpCardType;
        [SerializeField] protected TextMeshProUGUI tmpAttrRuby;
        [SerializeField] protected TextMeshProUGUI tmpAuther;

        [SerializeField] protected Image imageFrame;
        [SerializeField] protected RawImage imageOverFrame;
        [SerializeField] protected Image imageAttr;
        [SerializeField] protected Text textLScale;
        [SerializeField] protected Text textRScale;
        [SerializeField] protected Text textPassword;

        protected string currentFontLanguage;

        protected virtual CardRenderer.CardStyle CardStyle => CardRenderer.CardStyle.OCG_TCG;
        protected virtual float CardNameLabelMaxWidth => 520f;
        protected virtual float[] FontSizeSimplifiedChinese => new float[] { 50f, 40f };
        protected virtual float[] FontSizeTraditionalChinese => new float[] { 55f, 40f };
        protected virtual float[] FontSizeKorean => new float[] { 50f, 40f };
        protected virtual float[] FontSizeJapanese => new float[] { 55f, 40f };
        protected virtual float[] FontSizeEnglish => new float[] { 63f, 43f };

        private RectTransform _tmpCardNameRT;
        private RectTransform TmpCardNameRT =>
            _tmpCardNameRT = _tmpCardNameRT != null ? _tmpCardNameRT : tmpCardName.GetComponent<RectTransform>();
        private ContentSizeFitter _tmpCardNameCSF;
        private ContentSizeFitter TmpCardNameCSF =>
            _tmpCardNameCSF = _tmpCardNameCSF != null ? _tmpCardNameCSF : tmpCardName.GetComponent<ContentSizeFitter>();

        protected virtual void SwitchLanguage(string language = null)
        {
            language ??= Language.GetCardConfig();

            if (currentFontLanguage == language)
                return;
            currentFontLanguage = language;

            if(language == Language.SimplifiedChinese)
            {
                tmpCardName.fontSize = FontSizeSimplifiedChinese[0];
                tmpCardType.fontSize = FontSizeSimplifiedChinese[1];
                SetFont(CardRenderer.tmpFontChineseSimplified);
            }
            else if(language == Language.TraditionalChinese)
            {
                tmpCardName.fontSize = FontSizeTraditionalChinese[0];
                tmpCardType.fontSize = FontSizeTraditionalChinese[1];
                SetFont(CardRenderer.tmpFontChineseTraditional);
            }
            else if(language == Language.Korean)
            {
                tmpCardName.fontSize = FontSizeKorean[0];
                tmpCardType.fontSize = FontSizeKorean[1];
                SetFont(CardRenderer.tmpFontKorean);
            }
            else if(language == Language.Japanese)
            {
                tmpCardName.fontSize = FontSizeJapanese[0];
                tmpCardType.fontSize = FontSizeJapanese[1];
                SetFont(CardRenderer.tmpFontJapanese);
            }
            else
            {
                tmpCardName.fontSize = FontSizeEnglish[0];
                tmpCardType.fontSize = FontSizeEnglish[1];
                SetFont(CardRenderer.tmpFontEnglish);
            }

            if (Language.UseLatin(language))
                tmpCardName.fontStyle = FontStyles.SmallCaps;
            else
                tmpCardName.fontStyle = FontStyles.Normal;
        }

        protected virtual void SetFont(TMP_FontAsset tmpFont)
        {
            tmpDescription.font = tmpFont;
            tmpDescriptionPendulum.font = tmpFont;
            tmpCardName.font = tmpFont;
            tmpCardType.font = tmpFont;
            tmpAttrRuby.font = tmpFont;
            tmpAuther.font = tmpFont;
        }

        protected virtual void ShowNameOnlyParts()
        {
            imageFrame.gameObject.SetActive(false);
            imageOverFrame.gameObject.SetActive(false);
            imageAttr.gameObject.SetActive(false);

            tmpDescription.text = string.Empty;
            tmpDescriptionPendulum.text = string.Empty;
            tmpCardType.text = string.Empty;
            textLScale.text = string.Empty;
            textRScale.text = string.Empty;
        }

        public virtual void SetCardName(Card data, string language)
        {
            SwitchLanguage(language);

            tmpCardName.text = data.Name;
            tmpCardName.color = Color.white;
            TmpCardNameRT.localScale = Vector3.one;
            TmpCardNameCSF.SetLayoutHorizontal();
            var nameWidth = TmpCardNameRT.rect.width;
            if(nameWidth > CardNameLabelMaxWidth)
                TmpCardNameRT.localScale = new Vector3(CardNameLabelMaxWidth / nameWidth, 1f, 1f);
            tmpAttrRuby.text = IDS.GetAttributeRubyText(data, language);

            ShowNameOnlyParts();
        }

        public virtual void SetCard(Card data, string language, Texture art, Texture2D overFrame = null)
        {
            SetCardName(data, language);
            SetAllArtPartsOff();
            SetPassword(data);
            tmpCardName.color = Color.black;
            tmpCardType.color = Color.black;
            tmpAuther.color = Color.black;

            imageFrame.gameObject.SetActive(true);
            imageAttr.gameObject.SetActive(true);
            imageFrame.sprite = TextureManager.container.GetCardFrame(data, CardStyle);
            if(overFrame != null)
            {
                imageOverFrame.gameObject.SetActive(true);
                var descMask = TextureManager.container.GetDescMask(CardStyle, data.HasType(CardType.Pendulum), data.Id, overFrame);
                var maskedOF = TextureProcessor.ApplyMaskToAlpha(overFrame, descMask, invertMask: true);
#if UNITY_EDITOR
                maskedOF.alphaIsTransparency = true;
#endif
                imageOverFrame.texture = maskedOF;
            }
        }

        protected virtual void SetPassword(Card data)
        {
            textPassword.color = Color.black;
            if (Settings.Data.CardRenderPassword)
                textPassword.text = data.Id.ToString("D8");
            else
                textPassword.text = string.Empty;
        }

        protected Texture2D GetDescMask(bool isPendulum)
        {
            if (isPendulum)
                return TextureManager.container.rd_CardDescColumnMask;
            else
                return TextureManager.container.CardDescColumnMask;
        }

        public virtual RawImage GetArtPartForVideo(bool isPendulum)
        {
            return null;
        }

        public virtual void SetAllArtPartsOff()
        {
        }

        #region Text Process

        private const string BIG_SLASH = "／";
        private const string SMALL_SLASH = " / ";

        protected static string TextForRender(string description, Card data)
        {
            if (string.IsNullOrEmpty(description))
                return string.Empty;

            if (data.IsRushDuelCard())
                description = data.GetRushDescriptionForRender(description);

            var language = data.isPre ? Language.GetPrereleaseConfig() : Language.GetCardConfig();
            description = description.Replace("\r", string.Empty);

            if (!data.IsRushDuelCard())
            {
                description = description.Replace("\t\n", "\f\f");
                description = description.Replace("\n●", "●●●");
                description = description.Replace("\n", string.Empty);
                description = description.Replace("\f\f", Program.STRING_LINE_BREAK);
                description = description.Replace("●●●", $"{Program.STRING_LINE_BREAK}●");
            }

            if (!Language.UseLatin(language))
                description = description.Replace(Program.STRING_SLASH, BIG_SLASH);
            else
                description = description.Replace(Program.STRING_SLASH, SMALL_SLASH);

            //if (!Language.UseLatin(language))
            //    description = description.Replace(" ", "\u00A0");
            description = description.Replace($"{Program.STRING_LINE_BREAK}{Program.STRING_LINE_BREAK}", Program.STRING_LINE_BREAK);
            return description;
        }

        protected static List<string> GetAuthorFromDescription(string description)
        {
            var lines = description.Replace("\r", string.Empty).Split("\n");
            var returnValue = new List<string>();

            StringBuilder beforeDiySymbol = new();
            bool foundDIY = false;

            foreach (var line in lines)
            {
                if (!foundDIY && line.StartsWith(Settings.Data.DiySymbol))
                {
                    var beforeDiySymbolText = beforeDiySymbol.ToString();
                    returnValue.Add(beforeDiySymbolText);
                    returnValue.Add(line);
                    foundDIY = true;
                }
                else if (!foundDIY && !string.IsNullOrEmpty(line))
                {
                    beforeDiySymbol.Append(line);
                }

                if (foundDIY)
                    break;
            }

            if (!foundDIY)
            {
                returnValue.Add(description);
                returnValue.Add(string.Empty);
            }

            return returnValue;
        }

        #endregion

    }
}
