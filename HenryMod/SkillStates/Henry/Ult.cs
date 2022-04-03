using System;
using YassoMod.Modules.Components;
using EntityStates;
using EntityStates.Commando.CommandoWeapon;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

namespace YassoMod.SkillStates
{
	// Token: 0x02000009 RID: 9
	public class Ult : BaseSkillState
	{
		// Token: 0x06000022 RID: 34 RVA: 0x00003760 File Offset: 0x00001960
		public override void OnEnter()
		{
			base.OnEnter();

			if(NetworkServer.active)
            {
				base.characterBody.AddBuff(RoR2Content.Buffs.HiddenInvincibility);
            }
			base.characterBody.SetAimTimer(1.5f);

			Util.PlaySound("YasuoSpecialStart", base.gameObject);
			
			this.roundsCompleted = 0;
			this.roundStopwatch = this.roundDuration;
			float num = (this.attackSpeedStat - 1f) * 0.5f;
			this.roundDuration /= num + 1f;
			this.crit = base.RollCrit();

			base.PlayAnimation("FullBody, Override", "Special", "ThrowBomb.playbackRate", (float)this.numRounds * this.roundDuration);
		}

		public override void OnExit()
		{
			if (NetworkServer.active)
			{
				base.characterBody.RemoveBuff(RoR2Content.Buffs.HiddenInvincibility);
			}
			base.OnExit();
		}

		private void Fire()
		{
			bool final = this.roundsCompleted == this.numRounds - 1;

			float d = final ? finalDamageCoefficient : damageCoefficient;
			
			new BlastAttack
			{
				attacker = base.gameObject,
				procChainMask = default(ProcChainMask),
				impactEffect = EffectCatalog.FindEffectIndexFromPrefab(Modules.Assets.tornadoImpactEffect),
				losType = BlastAttack.LoSType.NearestHit,
				damageColorIndex = DamageColorIndex.Default,
				damageType = DamageType.FruitOnHit,
				procCoefficient = Ult.procCoefficient,
				bonusForce = Vector3.zero,
				baseForce = 0,
				baseDamage = d * this.damageStat,
				falloffModel = BlastAttack.FalloffModel.Linear,
				radius = this.maxRange,
				position = base.transform.position,
				attackerFiltering = AttackerFiltering.NeverHitSelf,
				teamIndex = base.GetTeam(),
				inflictor = base.gameObject,
				crit = this.crit
			}.Fire();

			if (NetworkServer.active)
			{
				Vector3 direction = Vector3.down;
				if (!final)
					direction = UnityEngine.Random.insideUnitSphere;
				direction.y = Mathf.Max(-0.2f, direction.y);
				direction = direction.normalized;
				List<HealthComponent> hits = new List<HealthComponent>();
				Collider[] hit = Physics.OverlapSphere(base.transform.position, this.maxRange, LayerIndex.entityPrecise.mask, QueryTriggerInteraction.UseGlobal);
				for (int i = 0; i < hit.Length; i++)
				{
					HurtBox hurtBox = hit[i].GetComponent<HurtBox>();
					if (hurtBox)
					{
						HealthComponent healthComponent = hurtBox.healthComponent;
						if (healthComponent)
						{
							TeamComponent team = healthComponent.GetComponent<TeamComponent>();
							bool enemy = team.teamIndex != base.teamComponent.teamIndex;
							if (enemy)
							{
								if (!hits.Contains(healthComponent))
								{
									hits.Add(healthComponent);
									if(healthComponent.body)
                                    {
                                        if (healthComponent.body.characterMotor)
                                            healthComponent.body.characterMotor.velocity = direction * force;
										else if(healthComponent.body.rigidbody)
											healthComponent.body.rigidbody.velocity = direction * force;
									}
								}
							}
						}
					}

				}
				
			}
		}

		// Token: 0x06000025 RID: 37 RVA: 0x00003A58 File Offset: 0x00001C58
		public override void FixedUpdate()
		{
			base.FixedUpdate();

			base.characterMotor.velocity = Vector3.zero;
			

			this.roundStopwatch += Time.fixedDeltaTime;
			bool flag3 = this.roundStopwatch >= this.roundDuration;
			if (flag3)
			{
				this.Fire();
				this.roundsCompleted++;
				this.roundStopwatch = 0f;
			}
			bool flag4 = this.roundsCompleted >= this.numRounds;
			if (flag4)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x06000026 RID: 38 RVA: 0x00003B90 File Offset: 0x00001D90
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x04000068 RID: 104
		public static float blastRadius = 11f;

		// Token: 0x04000069 RID: 105
		public static float damageCoefficient = 3.75f;

		public static float finalDamageCoefficient = 12f;

		// Token: 0x0400006A RID: 106
		public static float procCoefficient = 1f;

		// Token: 0x0400006B RID: 107
		public static float baseDuration = 1f;

		// Token: 0x0400006C RID: 108
		public static float force = 12f;

		// Token: 0x04000070 RID: 112
		public bool crit;

		// Token: 0x04000071 RID: 113
		private string muzzleString;

		// Token: 0x04000072 RID: 114
		private float maxRange = 12f;

		// Token: 0x04000073 RID: 115
		private float roundDuration = 0.45f;

		// Token: 0x04000074 RID: 116
		private float roundStopwatch;

		// Token: 0x04000075 RID: 117
		private int numRounds = 3;

		// Token: 0x04000076 RID: 118
		private int roundsCompleted = 0;

		// Token: 0x04000077 RID: 119
		private EntityStateMachine body;

		// Token: 0x04000078 RID: 120
		private DamageType damageType = DamageType.Generic;

		// Token: 0x04000079 RID: 121
		private bool animSwitch;

		// Token: 0x0400007A RID: 122
		private CharacterBody fireTarget;
	}
}
