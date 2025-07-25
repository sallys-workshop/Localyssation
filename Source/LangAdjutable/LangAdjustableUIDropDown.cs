﻿using System.Collections.Generic;
using UnityEngine;
using static Localyssation.LangAdjutable.LangAdjustables;
using Localyssation.LanguageModule;

#pragma warning disable IDE0130
namespace Localyssation.LangAdjutable
{
    public class LangAdjustableUIDropdown : MonoBehaviour, ILangAdjustable
    {
        public UnityEngine.UI.Dropdown dropdown;
        public List<System.Func<int, string>> newTextFuncs;

        public void Awake()
        {
            dropdown = GetComponent<UnityEngine.UI.Dropdown>();
            Localyssation.instance.OnLanguageChanged += onLanguageChanged;

            // auto-shrink text according to options
            if (dropdown.itemText)
            {
                dropdown.itemText.gameObject.AddComponent<LangAdjustableUIText>();
            }
            if (dropdown.captionText)
            {
                dropdown.captionText.gameObject.AddComponent<LangAdjustableUIText>();
            }
        }

        public void Start()
        {
            AdjustToLanguage(LanguageManager.CurrentLanguage);
        }

        private void onLanguageChanged(Language newLanguage)
        {
            AdjustToLanguage(newLanguage);
        }

        public void AdjustToLanguage(Language newLanguage)
        {
            if (newTextFuncs != null && newTextFuncs.Count == dropdown.options.Count)
            {
                for (var i = 0; i < dropdown.options.Count; i++)
                {
                    var option = dropdown.options[i];
                    option.text = newTextFuncs[i](-1);
                }
                dropdown.RefreshShownValue();
            }
        }

        public void OnDestroy()
        {
            Localyssation.instance.OnLanguageChanged -= onLanguageChanged;
            registeredDropdowns.Remove(dropdown);
        }
    }
}