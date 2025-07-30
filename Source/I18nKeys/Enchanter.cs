using Localyssation.Util;
using System.Linq;

namespace Localyssation
{
    internal static partial class I18nKeys
    {
        internal static class Enchanter
        {
            internal static void Init() { }
            private static TranslationKey create(string key, string value = "")
            {
                return I18nKeys.Create($"ENCHANTER_GUI_{key.ToUpper()}", value);
            }

            public static readonly string HEADER
                = create("HEADER", "- Item Enchant -");
            public static readonly string BUTTON_CLEAR_SELECTION
                = create("BUTTON_CLEAR_SELECTION", "Clear Selection");


            public static string TransmuteButtonKey(DamageType type, bool free)
            {
                return $"BUTTON_TRANSMUTE_{type.ToString().ToUpper()}_{(free ? "FREE" : "FORMAT")}";
            }
            private static string generateTransmuteButton(DamageType type, bool free)
            {
                return create(
                    TransmuteButtonKey(type, free),
                    (type == DamageType.Mind && free ? "Apply Flux Stone (Free)" : $"Transmute {type.ToString()} " + (free ? "(Free)" : "(x{0})"))
                );
            }
            public static readonly string[] BUTTON_TRANSMUTE = new[] { DamageType.Dexterity, DamageType.Strength, DamageType.Mind }
                .SelectMany(
                    type => new[] { true, false }
                        .Select(free => generateTransmuteButton(type, free))
                ).ToArray();

            public static readonly string BUTTON_ENCHANT_REROLL
                = create("BUTTON_ENCHANT_REROLL", "Re-roll Enchant");
            public static readonly string BUTTON_ENCHANT_ENCHANT
                = create("BUTTON_ENCHANT_ENCHANT", "Enchant Item");
            public static readonly string BUTTON_ENCHANT_UNABLE
                = create("BUTTON_ENCHANT_UNABLE", "Cannot enchant");
            public static readonly string STATUS_NO_ENCHANT
                = create("STATUS_NO_ENCHANT", "No enchantment applied on this item");
            public static readonly string STATUS_UNABLE_TO_ENCHANT
                = create("STATUS_UNABLE_TO_ENCHANT", "Item cannot be enchanted");
            public static readonly string BUTTON_ENCHANT_INSERT_ITEM
                = create(nameof(BUTTON_ENCHANT_INSERT_ITEM), "Insert item to enchant");
            public static readonly string STATUS_CURRENT_ENCHANTMENT
                = create(nameof(STATUS_CURRENT_ENCHANTMENT), "Current enchantment: ");

            public static readonly TranslationKey GET_NEW_ENCHANTMENT_FORMAT
                = create("GET_NEW_ENCHANTMENT_FORMAT", "You got the {0} enchantment!");
            public static readonly TranslationKey TRANSMUTE_TO_STRENGTH_FORMAT
                = create("TRANSMUTE_TO_STRENGTH_FORMAT", "Your {0} now scales off Strength!");
            public static readonly TranslationKey TRANSMUTE_TO_DEXTERITY_FORMAT
                = create("TRANSMUTE_TO_DEXTERITY_FORMAT", "Your {0} now scales off Dexterity!");
            public static readonly TranslationKey TRANSMUTE_TO_MIND_FORMAT
                = create("TRANSMUTE_TO_MIND_FORMAT", "Your {0} now scales off Mind!");

            public static readonly TranslationKey NOT_ENOUGH_TRANSMUTE_STONES_STRENGTH
                = create("NOT_ENOUGH_TRANSMUTE_STONES_STRENGTH", "Not enough Might Stones");
            public static readonly TranslationKey NOT_ENOUGH_TRANSMUTE_STONES_DEXTERITY
                = create("NOT_ENOUGH_TRANSMUTE_STONES_DEXTERITY", "Not enough Agility Stones");
            public static readonly TranslationKey NOT_ENOUGH_TRANSMUTE_STONES_MIND
                = create("NOT_ENOUGH_TRANSMUTE_STONES_MIND", "Not enough Flux Stones");

            public static readonly TranslationKey CANNOT_TRANSMUTE_WEAPON
                = create("CANNOT_TRANSMUTE_WEAPON", "Cannot Transmute Weapon");
        }
    }
}