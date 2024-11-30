using System.Collections.Generic;
using UnityEngine;

namespace Localyssation
{
    // "adjustables" are in-game objects that must be automatically adjusted with
    // language-specific variants (strings, textures, etc.) whenever language is changed in-game
    internal static class LangAdjustables
    {
        public static List<ILangAdjustable> nonMonoBehaviourAdjustables = new List<ILangAdjustable>();

        public static void Init()
        {
            Localyssation.instance.onLanguageChanged += (newLanguage) =>
            {
                // copy the list to avoid the loop breaking if an entry self-destructs mid-loop
                var safeAdjustables = new List<ILangAdjustable>(nonMonoBehaviourAdjustables);
                foreach (var replaceable in safeAdjustables)
                {
                    replaceable.AdjustToLanguage(newLanguage);
                }
            };
        }

        // handy function to slot into the newTextFunc param when you need a simple key->string replacement
        public static System.Func<string> GetStringFunc(string key)
        {
            return () => Localyssation.GetString(key);
        }

        public static void RegisterText(UnityEngine.UI.Text text, System.Func<string> newTextFunc)
        {
            if (text.GetComponent<LangAdjustableUIText>()) return;

            var replaceable = text.gameObject.AddComponent<LangAdjustableUIText>();
            replaceable.newTextFunc = newTextFunc;
        }

        public static void RegisterDropdown(UnityEngine.UI.Dropdown dropdown, List<System.Func<string>> newTextFuncs)
        {
            if (dropdown.GetComponent<LangAdjustableUIDropdown>()) return;

            var replaceable = dropdown.gameObject.AddComponent<LangAdjustableUIDropdown>();
            replaceable.newTextFuncs = newTextFuncs;
        }

        public static void RegisterTextSetterEventTrigger(UnityEngine.EventSystems.EventTrigger.TriggerEvent triggerEvent, UnityEngine.Events.ArgumentCache argumentCache, System.Func<string> newTextFunc)
        {
            var replaceable = new LangAdjustableTextSetterEventTrigger { triggerEvent = triggerEvent, argumentCache = argumentCache, newTextFunc = newTextFunc };
            replaceable.AdjustToLanguage(Localyssation.currentLanguage);
            nonMonoBehaviourAdjustables.Add(replaceable);
        }

        public interface ILangAdjustable
        {
            void AdjustToLanguage(Language newLanguage);
        }

        public class LangAdjustableUIText : MonoBehaviour, ILangAdjustable
        {
            public UnityEngine.UI.Text text;
            public System.Func<string> newTextFunc;

            public bool orig_resizeTextForBestFit = false;
            public int orig_resizeTextMaxSize;

            public void Awake()
            {
                text = GetComponent<UnityEngine.UI.Text>();
                orig_resizeTextForBestFit = text.resizeTextForBestFit;
                orig_resizeTextMaxSize = text.resizeTextMaxSize;
                Localyssation.instance.onLanguageChanged += onLanguageChanged;
            }

            public void Start()
            {
                AdjustToLanguage(Localyssation.currentLanguage);
            }

            private void onLanguageChanged(Language newLanguage)
            {
                AdjustToLanguage(newLanguage);
            }

            public void AdjustToLanguage(Language newLanguage)
            {
                if (newTextFunc != null)
                {
                    text.text = newTextFunc();
                }

                if (newLanguage.shrinkOverflowingText != text.resizeTextForBestFit)
                {
                    if (newLanguage.shrinkOverflowingText)
                    {
                        text.resizeTextMaxSize = text.fontSize;
                        text.resizeTextForBestFit = true;
                    }
                    else
                    {
                        text.resizeTextForBestFit = orig_resizeTextForBestFit;
                        text.resizeTextMaxSize = orig_resizeTextMaxSize;
                    }
                }
            }

            public void OnDestroy()
            {
                Localyssation.instance.onLanguageChanged -= onLanguageChanged;
            }
        }

        public class LangAdjustableUIDropdown : MonoBehaviour, ILangAdjustable
        {
            public UnityEngine.UI.Dropdown dropdown;
            public List<System.Func<string>> newTextFuncs;

            public void Awake()
            {
                dropdown = GetComponent<UnityEngine.UI.Dropdown>();
                Localyssation.instance.onLanguageChanged += onLanguageChanged;

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
                AdjustToLanguage(Localyssation.currentLanguage);
            }

            private void onLanguageChanged(Language newLanguage)
            {
                AdjustToLanguage(newLanguage);
            }

            public void AdjustToLanguage(Language newLanguage)
            {
                if (newTextFuncs.Count == dropdown.options.Count)
                {
                    for (var i = 0; i < dropdown.options.Count; i++)
                    {
                        var option = dropdown.options[i];
                        option.text = newTextFuncs[i]();
                    }
                    dropdown.RefreshShownValue();
                }
            }

            public void OnDestroy()
            {
                Localyssation.instance.onLanguageChanged -= onLanguageChanged;
            }
        }

        public class LangAdjustableTextSetterEventTrigger : ILangAdjustable
        {
            public UnityEngine.EventSystems.EventTrigger.TriggerEvent triggerEvent;
            public UnityEngine.Events.ArgumentCache argumentCache;
            public System.Func<string> newTextFunc;

            public void AdjustToLanguage(Language newLanguage)
            {
                if (newTextFunc != null)
                {
                    argumentCache.stringArgument = newTextFunc();
                    triggerEvent.DirtyPersistentCalls();
                }
            }
        }
    }
}
