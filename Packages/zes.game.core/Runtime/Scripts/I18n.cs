using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Zes
{
    public class I18n : MonoBehaviour
    {
        public static Func<int, string> translation;

        private static Logger logger = Logger.GetLogger<I18n>();
        public MaskableGraphic uiText;
        public int languageId;

        private void Start()
        {
            SetText(GetText());
        }

        private string GetText()
        {
            return translation(languageId);
        }

        private void SetText(string value)
        {
            TMP_Text txtpro = uiText as TMP_Text;
            if (txtpro != null)
            {
                txtpro.text = value;
                return;
            }


            Text txt = uiText as Text;
            if (txt != null)
            {
                txt.text = value;
                return;
            }

            var uitype = uiText == null ? "null" : uiText.GetType().ToString();
            logger.Error($"unknown type for uiText (${uitype})");

        }
    }
}
