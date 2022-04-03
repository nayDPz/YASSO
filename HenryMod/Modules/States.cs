using YassoMod.SkillStates;
using YassoMod.SkillStates.BaseStates;
using System.Collections.Generic;
using System;

namespace YassoMod.Modules
{
    public static class States
    {
        internal static void RegisterStates()
        {
            Modules.Content.AddEntityState(typeof(BaseMeleeAttack));
            Modules.Content.AddEntityState(typeof(SlashCombo));


            Modules.Content.AddEntityState(typeof(SkillStates.Henry.EnterSecondary));
            Modules.Content.AddEntityState(typeof(Secondary1));

            Modules.Content.AddEntityState(typeof(Dash));

            Modules.Content.AddEntityState(typeof(Secondary3));
        }
    }
}