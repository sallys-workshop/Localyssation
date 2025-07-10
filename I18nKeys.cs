using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Localyssation
{
    // Symbolized key strings, for better referring
    // Less string constants, less typos
    internal static class I18nKeys
    {

        internal static readonly Dictionary<string, string> TR_KEYS = new Dictionary<string, string>();
        private static string create(string key, string defaultString = "")
        {
            if (string.IsNullOrEmpty(key)) { throw new ArgumentNullException("key"); }
            if (string.IsNullOrEmpty(defaultString))
            {
                defaultString = key;
            }
            return key;
        }
        internal static class Item
        {
            public static readonly string FORMAT_ITEM_RARITY
                = create("FORMAT_ITEM_RARITY", "[{0}]");
            public static readonly string FORMAT_ITEM_TOOLTIP_VENDOR_VALUE_COUNTER
                = create("FORMAT_ITEM_TOOLTIP_VENDOR_VALUE_COUNTER", "{0}");
            public static readonly string FORMAT_ITEM_TOOLTIP_VENDOR_VALUE_COUNTER_MULTIPLE
                = create("FORMAT_ITEM_TOOLTIP_VENDOR_VALUE_COUNTER_MULTIPLE", "<color=grey>(x{0} each)</color> {1}");
            // Gamble
            public static readonly string TOOLTIP_GAMBLE_ITEM_NAME 
                = create("ITEM_TOOLTIP_GAMBLE_ITEM_NAME", "Mystery Item");
            public static readonly string TOOLTIP_GAMBLE_ITEM_RARITY 
                = create("ITEM_TOOLTIP_GAMBLE_ITEM_RARITY", "[Unknown]");
            public static readonly string TOOLTIP_GAMBLE_ITEM_DESC 
                = create("ITEM_TOOLTIP_GAMBLE_ITEM_DESCRIPTION", "You can't really see what this is until you buy it.");

            // Consumable
            public static readonly string TOOLTIP_CONSUMABLE_DESCRIPTION_HEALTH_APPLY
                = create("ITEM_TOOLTIP_CONSUMABLE_DESCRIPTION_HEALTH_APPLY", "Recovers {0} Health.");
            public static readonly string TOOLTIP_CONSUMABLE_DESCRIPTION_MANA_APPLY
                = create("ITEM_TOOLTIP_CONSUMABLE_DESCRIPTION_MANA_APPLY", "Recovers {0} Mana.");
            public static readonly string TOOLTIP_CONSUMABLE_DESCRIPTION_STAMINA_APPLY
                = create("ITEM_TOOLTIP_CONSUMABLE_DESCRIPTION_STAMINA_APPLY", "Recovers {0} Stamina.");
            public static readonly string TOOLTIP_CONSUMABLE_DESCRIPTION_EXP_GAIN
                = create("ITEM_TOOLTIP_CONSUMABLE_DESCRIPTION_EXP_GAIN", "Gain {0} Experience on use.");
        }
        
        internal static class Lore
        {
            public static readonly string CROWN = create("CROWN", "crown");
            public static readonly string CROWN_PLURAL = create("CROWN_PLURAL", "crowns");
        }
    }
}
