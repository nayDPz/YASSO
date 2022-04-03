using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace YassoMod.Modules
{
    public static class Buffs
    {
        // armor buff gained during roll
        internal static BuffDef qStackBuff;
        internal static BuffDef dashCooldownBuff;
        internal static void RegisterBuffs()
        {
            qStackBuff = AddNewBuff("YasuoQStack", RoR2.LegacyResourcesAPI.Load<Sprite>("Textures/BuffIcons/texBuffGenericShield"), Color.white, true, false);
            dashCooldownBuff = AddNewBuff("DashCooldown", RoR2.LegacyResourcesAPI.Load<Sprite>("Textures/BuffIcons/texBuffGenericShield"), Color.white, false, false);
        }

        // simple helper method
        internal static BuffDef AddNewBuff(string buffName, Sprite buffIcon, Color buffColor, bool canStack, bool isDebuff)
        {
            BuffDef buffDef = ScriptableObject.CreateInstance<BuffDef>();
            buffDef.name = buffName;
            buffDef.buffColor = buffColor;
            buffDef.canStack = canStack;
            buffDef.isDebuff = isDebuff;
            buffDef.eliteDef = null;
            buffDef.iconSprite = buffIcon;

            Modules.Content.AddBuffDef(buffDef);

            return buffDef;
        }
    }
}