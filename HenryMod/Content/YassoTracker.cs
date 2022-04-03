using System;
using System.Linq;
using UnityEngine;
using RoR2;

namespace YassoMod.Modules.Components
{
	// Token: 0x02000739 RID: 1849
	[RequireComponent(typeof(CharacterBody))]
	[RequireComponent(typeof(TeamComponent))]
	[RequireComponent(typeof(InputBankTest))]
	public class YassoTracker : MonoBehaviour
	{
		// Token: 0x06002662 RID: 9826 RVA: 0x000A7035 File Offset: 0x000A5235
		private void Awake()
		{
			this.indicator = new Indicator(base.gameObject, LegacyResourcesAPI.Load<GameObject>("Prefabs/HuntressTrackingIndicator"));
		}

		// Token: 0x06002663 RID: 9827 RVA: 0x000A7052 File Offset: 0x000A5252
		private void Start()
		{
			this.characterBody = base.GetComponent<CharacterBody>();
			this.inputBank = base.GetComponent<InputBankTest>();
			this.teamComponent = base.GetComponent<TeamComponent>();
		}

		// Token: 0x06002664 RID: 9828 RVA: 0x000A7078 File Offset: 0x000A5278
		public HurtBox GetTrackingTarget()
		{
			return this.trackingTarget;
		}

		// Token: 0x06002665 RID: 9829 RVA: 0x000A7080 File Offset: 0x000A5280
		private void OnEnable()
		{
			this.indicator.active = true;
		}

		// Token: 0x06002666 RID: 9830 RVA: 0x000A708E File Offset: 0x000A528E
		private void OnDisable()
		{
			this.indicator.active = false;
		}

		// Token: 0x06002667 RID: 9831 RVA: 0x000A709C File Offset: 0x000A529C
		private void FixedUpdate()
		{
			this.trackerUpdateStopwatch += Time.fixedDeltaTime;
			if (this.trackerUpdateStopwatch >= 1f / this.trackerUpdateFrequency)
			{
				this.trackerUpdateStopwatch -= 1f / this.trackerUpdateFrequency;
				HurtBox hurtBox = this.trackingTarget;
				Ray aimRay = new Ray(this.inputBank.aimOrigin, this.inputBank.aimDirection);
				this.SearchForTarget(aimRay);
				this.indicator.targetTransform = (this.trackingTarget ? this.trackingTarget.transform : null);
			}
		}

		// Token: 0x06002668 RID: 9832 RVA: 0x000A713C File Offset: 0x000A533C
		private void SearchForTarget(Ray aimRay)
		{
			this.search.teamMaskFilter = TeamMask.GetUnprotectedTeams(this.teamComponent.teamIndex);
			this.search.filterByLoS = true;
			this.search.searchOrigin = aimRay.origin;
			this.search.searchDirection = aimRay.direction;
			this.search.sortMode = BullseyeSearch.SortMode.Distance;
			this.search.maxDistanceFilter = this.maxTrackingDistance;
			this.search.maxAngleFilter = this.maxTrackingAngle;
			this.search.RefreshCandidates();
			this.search.FilterOutGameObject(base.gameObject);
			foreach(HurtBox h in this.search.GetResults())
            {
				if(h && h.healthComponent && h.healthComponent.body)
                {
					if (h.healthComponent.body.HasBuff(Modules.Buffs.dashCooldownBuff))
                    {
						this.search.FilterOutGameObject(h.gameObject); // idk which one lol
						this.search.FilterOutGameObject(h.healthComponent.gameObject);
					}
						
                }
				this.search.FilterOutGameObject(h.gameObject);
			}
			this.trackingTarget = this.search.GetResults().FirstOrDefault<HurtBox>();
		}

		// Token: 0x04002A49 RID: 10825
		public float maxTrackingDistance = 27f;

		// Token: 0x04002A4A RID: 10826
		public float maxTrackingAngle = 20f;

		// Token: 0x04002A4B RID: 10827
		public float trackerUpdateFrequency = 10f;

		// Token: 0x04002A4C RID: 10828
		private HurtBox trackingTarget;

		// Token: 0x04002A4D RID: 10829
		private CharacterBody characterBody;

		// Token: 0x04002A4E RID: 10830
		private TeamComponent teamComponent;

		// Token: 0x04002A4F RID: 10831
		private InputBankTest inputBank;

		// Token: 0x04002A50 RID: 10832
		private float trackerUpdateStopwatch;

		// Token: 0x04002A51 RID: 10833
		private Indicator indicator;

		// Token: 0x04002A52 RID: 10834
		private readonly BullseyeSearch search = new BullseyeSearch();
	}
}
