using System;
using JetBrains.Annotations;
using UnityEngine;
using RoR2.Skills;
using RoR2;

namespace YassoMod.Modules.Components
{
	public class DashTrackerSkillDef : SkillDef
	{
		// Token: 0x06004566 RID: 17766 RVA: 0x0011F902 File Offset: 0x0011DB02
		public override SkillDef.BaseSkillInstanceData OnAssigned([NotNull] GenericSkill skillSlot)
		{
			return new DashTrackerSkillDef.InstanceData
			{
				huntressTracker = skillSlot.GetComponent<YassoTracker>()
			};
		}

		// Token: 0x06004567 RID: 17767 RVA: 0x0011F915 File Offset: 0x0011DB15
		private static bool HasTarget([NotNull] GenericSkill skillSlot)
		{
			YassoTracker huntressTracker = ((DashTrackerSkillDef.InstanceData)skillSlot.skillInstanceData).huntressTracker;
			return (huntressTracker != null) ? huntressTracker.GetTrackingTarget() : null;
		}

		// Token: 0x06004568 RID: 17768 RVA: 0x0011F93D File Offset: 0x0011DB3D
		public override bool CanExecute([NotNull] GenericSkill skillSlot)
		{
			return DashTrackerSkillDef.HasTarget(skillSlot) && base.CanExecute(skillSlot);
		}

		// Token: 0x06004569 RID: 17769 RVA: 0x0011F950 File Offset: 0x0011DB50
		public override bool IsReady([NotNull] GenericSkill skillSlot)
		{
			return base.IsReady(skillSlot) && DashTrackerSkillDef.HasTarget(skillSlot);
		}

		// Token: 0x02000BFE RID: 3070
		protected class InstanceData : SkillDef.BaseSkillInstanceData
		{
			// Token: 0x04004392 RID: 17298
			public YassoTracker huntressTracker;
		}
	}
}
