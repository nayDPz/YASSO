using System;
using JetBrains.Annotations;
using UnityEngine;
using RoR2.Skills;
using RoR2;

namespace YassoMod.Modules.Components
{
	public class ASpdCdSkillDef : SkillDef
	{
        public override void OnExecute([NotNull] GenericSkill skillSlot)
        {
            base.OnExecute(skillSlot);
        }
        public override float GetRechargeInterval([NotNull] GenericSkill skillSlot)
        {
            skillSlot.finalRechargeInterval = this.baseRechargeInterval / skillSlot.characterBody.attackSpeed;
            return this.baseRechargeInterval / skillSlot.characterBody.attackSpeed;
        }

	}
}
