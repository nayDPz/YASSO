using YassoMod.SkillStates.BaseStates;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using RoR2.Skills;
using EntityStates;

namespace YassoMod.SkillStates
{
    public class Secondary1 : BaseMeleeAttack
    {
        public override void OnEnter()
        {
            this.hitboxName = "Sword";

            this.damageType = DamageType.Generic;
            this.damageCoefficient = Modules.StaticValues.swordDamageCoefficient;
            this.procCoefficient = 1f;
            this.pushForce = 300f;
            this.bonusForce = Vector3.zero;
            this.baseDuration = 1f;
            this.attackStartTime = 0.3f;
            this.attackEndTime = 0.4f;
            this.baseEarlyExitTime = 0.45f;
            this.hitStopDuration = 0.012f;
            this.attackRecoil = 0.5f;
            this.hitHopVelocity = 4f;
            this.bufferExit = true;

            this.swingSoundString = "YasuoQAttack";
            this.hitSoundString = "";
            this.hitEffectPrefab = Modules.Assets.autoHitImpactEffect;

            this.impactSound = Modules.Assets.qHitSoundEvent.index;

            base.OnEnter();
        }

        protected override void PlayAttackAnimation()
        {
            base.PlayCrossfade("FullBody, Override", "Secondary1", "Slash.playbackRate", this.duration, 0.05f);
        }

        protected override void PlaySwingEffect()
        {
            base.PlaySwingEffect();
        }

        bool s = false;
        protected override void OnHitEnemyAuthority()
        {
            base.OnHitEnemyAuthority();
            if(NetworkServer.active)
            {
                int sta = base.characterBody.GetBuffCount(Modules.Buffs.qStackBuff);
                base.characterBody.ClearTimedBuffs(Modules.Buffs.qStackBuff);
                for(int i = 0; i < sta + 1; i++)
                    base.characterBody.AddTimedBuff(Modules.Buffs.qStackBuff, 8f, sta + 1);
            }
            if(base.characterBody.GetBuffCount(Modules.Buffs.qStackBuff) >= 2 && !s)
            {
                s = true;
                Util.PlaySound("YasuoQBuff", base.gameObject);
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

        protected override void SetNextState()
        {
            this.outer.SetNextStateToMain();
        }

        public override void OnExit()
        {
            base.OnExit();
        }
    }
}