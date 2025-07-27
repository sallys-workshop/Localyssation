
namespace Localyssation
{
    internal static partial class I18nKeys
    {

        internal static class SkillMenu
        {
            internal static void Init() { }

            public static readonly string RANK_SOULBOUND
                = Create("SKILL_RANK_SOULBOUND", "Soulbound Skill");
            public static readonly string RANK
                = Create("FORMAT_SKILL_RANK", "[Rank {0} / {1}]");
            public static readonly string SKILL_POINT_COST_FORMAT
                = Create("SKILL_POINT_COST_FORMAT", "Point Cost: {0}");
            public static readonly string TOOLTIP_DAMAGE_TYPE
                = Create("FORMAT_SKILL_TOOLTIP_DAMAGE_TYPE", "{0} Skill");
            public static readonly string TOOLTIP_ITEM_COST
                = Create("FORMAT_SKILL_TOOLTIP_ITEM_COST", "{0} {1}");
            public static readonly string TOOLTIP_MANA_COST
                = Create("FORMAT_SKILL_TOOLTIP_MANA_COST", "{0} Mana");
            public static readonly string TOOLTIP_HEALTH_COST
                = Create("FORMAT_SKILL_TOOLTIP_HEALTH_COST", "{0} Health");
            public static readonly string TOOLTIP_STAMINA_COST
                = Create("FORMAT_SKILL_TOOLTIP_STAMINA_COST", "{0} Stamina");
            public static readonly string TOOLTIP_CAST_TIME_INSTANT
                = Create("SKILL_TOOLTIP_CAST_TIME_INSTANT", "Instant Cast");
            public static readonly string TOOLTIP_CAST_TIME
                = Create("FORMAT_SKILL_TOOLTIP_CAST_TIME", "{0} sec Cast");
            public static readonly string TOOLTIP_COOLDOWN
                = Create("FORMAT_SKILL_TOOLTIP_COOLDOWN", "{0} sec Cooldown");
            public static readonly string TOOLTIP_PASSIVE
                = Create("SKILL_TOOLTIP_PASSIVE", "Passive Skill");

            //public static readonly string TOOLTIP_RANK_DESCRIPTOR_NEXT_RANK
            //    = create("SKILL_TOOLTIP_RANK_DESCRIPTOR_NEXT_RANK", "\n<color=white><i>[Next Rank]</i></color>");
            //public static readonly string TOOLTIP_RANK_DESCRIPTOR_CURRENT_RANK
            //    = create("FORMAT_SKILL_TOOLTIP_RANK_DESCRIPTOR_CURRENT_RANK", "\n<color=white><i>[Rank {0}]</i></color>");
            //public static readonly string TOOLTIP_RANK_DESCRIPTOR_REQUIRED_LEVEL
            //    = create("FORMAT_SKILL_TOOLTIP_RANK_DESCRIPTOR_REQUIRED_LEVEL", "<color=red>\n(Requires Lv. {0})</color>");

            public static readonly string TOOLTIP_DESCRIPTOR_MANACOST
                = Create("FORMAT_SKILL_TOOLTIP_DESCRIPTOR_MANACOST", "<color=yellow>Costs {0} Mana.</color>");
            public static readonly string TOOLTIP_DESCRIPTOR_HEALTHCOST
                = Create("FORMAT_SKILL_TOOLTIP_DESCRIPTOR_HEALTHCOST", "<color=yellow>Costs {0} Health.</color>");
            public static readonly string TOOLTIP_DESCRIPTOR_STAMINACOST
                = Create("FORMAT_SKILL_TOOLTIP_DESCRIPTOR_STAMINACOST", "<color=yellow>Costs {0} Stamina.</color>");
            public static readonly string TOOLTIP_DESCRIPTOR_COOLDOWN
                = Create("FORMAT_SKILL_TOOLTIP_DESCRIPTOR_COOLDOWN", "<color=yellow>{0} sec cooldown.</color>");
            public static readonly string TOOLTIP_DESCRIPTOR_CAST_TIME
                = Create("FORMAT_SKILL_TOOLTIP_DESCRIPTOR_CAST_TIME", "<color=yellow>{0} sec cast time.</color>");
            public static readonly string TOOLTIP_DESCRIPTOR_CAST_TIME_INSTANT
                = Create("SKILL_TOOLTIP_DESCRIPTOR_CAST_TIME_INSTANT", "<color=yellow>instant cast time.</color>");

            public static readonly string TOOLTIP_REQUIEMENT_FORMAT
                = Create("FORMAT_SKILL_TOOLTIP_REQUIREMENT", " <color=yellow>Requirement a {0} weapon.</color>");
            public static readonly string TOOLTIP_REQUIRE_SHIELD
                = Create("SKILL_TOOLTIP_REQUIREMENT_SHIELD", " <color=yellow>Requires a shield.</color>");

            public static readonly string TOOLTIP_DESCRIPTOR_CONDITION_CANCEL_ON_HIT
                = Create("SKILL_TOOLTIP_DESCRIPTOR_CONDITION_CANCEL_ON_HIT", " <color=yellow>Cancels if hit.</color>");
            public static readonly string TOOLTIP_DESCRIPTOR_CONDITION_IS_STACKABLE
                = Create("SKILL_TOOLTIO_DESCRIPTOR_CONDITION_IS_STACKABLE", " <color=yellow>Stackable.</color>");
            public static readonly string TOOLTIP_DESCRIPTOR_CONDITION_CHANCE
                = Create("SKILL_TOOLTIP_DESCRIPTOR_CONDITION_CHANCE", "({0}% Chance)");
        }

    }
}