using YassoMod.SkillStates.BaseStates;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace YassoMod.SkillStates
{
    public class DashSpin : BaseMeleeAttack
    {

        public bool empowered;
        public override void OnEnter()
        {
            this.hitboxName = "Spin";

            this.damageType = this.empowered ? DamageType.ApplyMercExpose : DamageType.Generic;
            this.damageCoefficient = empowered ? 7f : 3f;
            this.procCoefficient = 1f;
            this.pushForce = 300f;
            this.bonusForce = empowered ? Vector3.up * 3000f : Vector3.zero;
            this.baseDuration = 0.67f;
            this.attackStartTime = 0.0f;
            this.attackEndTime = 0.2f;
            this.baseEarlyExitTime = 0.2f;
            this.hitStopDuration = 0.04f;
            this.attackRecoil = 0.5f;
            this.hitHopVelocity = 9f;

            this.swingSoundString = "YasuoQAttack";
            this.hitSoundString = "";
            this.swingEffectPrefab = null;
            this.hitEffectPrefab = empowered ? Modules.Assets.tornadoImpactEffect : Modules.Assets.autoHitImpactEffect;

            this.impactSound = Modules.Assets.eqHitSoundEvent.index;
            base.OnEnter();
        }

        protected override void PlayAttackAnimation()
        {
            base.PlayCrossfade("FullBody, Override", "SecondaryDash", "Slash.playbackRate", this.duration, 0.05f);
        }

        protected override void PlaySwingEffect()
        {
            base.PlaySwingEffect();
        }

        bool s = false;
        protected override void OnHitEnemyAuthority()
        {
            base.OnHitEnemyAuthority();
            if (NetworkServer.active && !this.empowered)
            {
                int sta = base.characterBody.GetBuffCount(Modules.Buffs.qStackBuff);
                base.characterBody.ClearTimedBuffs(Modules.Buffs.qStackBuff);
                for (int i = 0; i < sta + 1; i++)
                    base.characterBody.AddTimedBuff(Modules.Buffs.qStackBuff, 8f, sta + 1);
            }
            if (base.characterBody.GetBuffCount(Modules.Buffs.qStackBuff) >= 2 && !s)
            {
                s = true;
                Util.PlaySound("YasuoQBuff", base.gameObject);
            }
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