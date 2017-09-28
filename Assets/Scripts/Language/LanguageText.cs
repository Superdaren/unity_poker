using UnityEngine;
using UnityEngine.UI;

namespace Language
{
	[RequireComponent(typeof (Text))]
	[AddComponentMenu("Language/LanguageText")]

	public class LanguageText : MonoBehaviour {
        [HideInInspector] public string Language;
        [HideInInspector] public string File;
        [HideInInspector] public string Key;
        [HideInInspector] public string Value;

        public LanguageService Localization;
        // Use this for initialization
        private Text label;

        void Start()
		{
			Localization = LanguageService.Instance;
            label = GetComponent<Text>();
            label.text = Localization.GetFromFile(File, Key, label.text);
		}

        public void UpdateText(string Key)
        {
            this.Key = Key;
            label.text = Localization.GetFromFile(File, Key, label.text);
        }
	}
}

