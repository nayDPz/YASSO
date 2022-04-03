using R2API;
using System;

namespace YassoMod.Modules
{
    internal static class Tokens
    {
        internal static void AddTokens()
        {
            #region Henry
            string prefix = YassoPlugin.DEVELOPER_PREFIX + "_HENRY_BODY_";

            string desc = "yasou is fucking bullshit" + Environment.NewLine + Environment.NewLine;

            string outro = "..broken nerf please gg jg diff.";
            string outroFailure = ".. jg diff ff15.";

            LanguageAPI.Add(prefix + "NAME", "Yasuo");
            LanguageAPI.Add(prefix + "DESCRIPTION", desc);
            LanguageAPI.Add(prefix + "SUBTITLE", "THE LEAAGUE OF LEGEND");
            LanguageAPI.Add(prefix + "LORE", "sample lore");
            LanguageAPI.Add(prefix + "OUTRO_FLAVOR", outro);
            LanguageAPI.Add(prefix + "OUTRO_FAILURE", outroFailure);

            #region Skins
            LanguageAPI.Add(prefix + "DEFAULT_SKIN_NAME", "yesou");
            #endregion


            #region Primary
            LanguageAPI.Add(prefix + "PRIMARY_SLASH_NAME", "auto attack idk");
            LanguageAPI.Add(prefix + "PRIMARY_SLASH_DESCRIPTION", Helpers.agilePrefix + $"Melee attack");
            #endregion

            #region Secondary
            LanguageAPI.Add(prefix + "SECONDARY_GUN_NAME", "Steel Tempest");
            LanguageAPI.Add(prefix + "SECONDARY_GUN_DESCRIPTION", Helpers.agilePrefix + $" Stab in a direction. Landing the attack twice turns the next cast into a tornado that exposes enemies.");
            #endregion

            #region Utility
            LanguageAPI.Add(prefix + "UTILITY_ROLL_NAME", "Sweeping Blade");
            LanguageAPI.Add(prefix + "UTILITY_ROLL_DESCRIPTION", "Dash to the targeted enemy, with a cooldown per target. Steel Tempest can be used during the dash.");
            #endregion

            #region Special
            LanguageAPI.Add(prefix + "SPECIAL_BOMB_NAME", "Last Breath");
            LanguageAPI.Add(prefix + "SPECIAL_BOMB_DESCRIPTION", $"Dash to an exposed enemy to deal damage in an area.");
            #endregion

            #endregion
        }
    }
}