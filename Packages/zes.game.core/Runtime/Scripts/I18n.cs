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

        [Tooltip("Language ID")]
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
            var uiText = GetComponent<TMP_Text>();
            if (uiText != null)
            {
                uiText.text = value;
                return;
            }

            logger.Error($"Cannot find TMPro_Text on the game object {gameObject.name}");
        }
    }
}
