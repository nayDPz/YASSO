using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System;

namespace YassoMod.SkillStates
{
    public class Dash : BaseSkillState
    {
        public int targetIndex = 0;
        public CharacterBody target;
        private Transform modelTransform;

        private Modules.Components.YassoTracker tracker;

        private HurtBoxGroup hurtboxGroup;

        private Vector3 lastKnownPosition;
        private Vector3 direction;
        private float distance;
        private float duration;
        private float speed;
        private bool hasFired;

        private float stopwatch;
        private float damageCoefficient = 1.5f;

        public static float hitRange = 4.5f;
        public static float stopTrackTime = 0.05f;
        private float baseDuration = 0.35f;
        public static float extraDuration = 0.17f;
        private float extraD;
        private float minDistance = 7f;
        public static float extraDistance = 3.25f;
        public static float exitExtraDistance = 3.25f;

        public static float baseChainPrepDuration = 0.067f;
        public static float basePrepDuration = 0.067f;
        private float prepDuration;
        private float prepStopwatch;

        public override void OnEnter()
        {
            base.OnEnter();

            if (base.characterBody && NetworkServer.active) base.characterBody.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;


            this.tracker = base.GetComponent<Modules.Components.YassoTracker>();
            if(this.tracker)
            {
                HurtBox h = this.tracker.GetTrackingTarget();
                if(h && h.healthComponent && h.healthComponent.body)
                    this.target = this.tracker.GetTrackingTarget().healthComponent.body;
            }

            if(!this.target)
            {
                this.outer.SetNextStateToMain();
                this.activatorSkillSlot.AddOneStock();
                return;
            }

            this.target.AddTimedBuff(Modules.Buffs.dashCooldownBuff, 8f);
            base.GetComponent<KinematicCharacterController.KinematicCharacterMotor>().ForceUnground();

            Vector3 corePosition = Util.GetCorePosition(target);
            this.distance = Mathf.Max(this.minDistance, (base.transform.position - corePosition).magnitude);
            this.direction = (corePosition - base.transform.position).normalized;
            this.duration = this.baseDuration / this.attackSpeedStat;
            this.extraD = Dash.extraDuration / this.attackSpeedStat;
            this.speed = this.distance / this.duration;

            this.lastKnownPosition = this.target.corePosition;

            this.prepDuration = Dash.baseChainPrepDuration / this.attackSpeedStat;

            this.modelTransform = base.GetModelTransform();
            if (this.modelTransform)
            {
                this.hurtboxGroup = this.modelTransform.GetComponent<HurtBoxGroup>();
            }
            if (this.hurtboxGroup)
            {
                HurtBoxGroup hurtBoxGroup = this.hurtboxGroup;
                int hurtBoxesDeactivatorCounter = hurtBoxGroup.hurtBoxesDeactivatorCounter + 1;
                hurtBoxGroup.hurtBoxesDeactivatorCounter = hurtBoxesDeactivatorCounter;
            }

            base.PlayCrossfade("FullBody, Override", "Dash", 0.1f);
            Util.PlaySound("YasuoDashStart", base.gameObject);
        }

        public override void OnExit()
        {

            base.gameObject.layer = LayerIndex.defaultLayer.intVal;
            base.characterMotor.Motor.RebuildCollidableLayers();
            base.characterMotor.velocity = Vector3.zero;

            if (this.hurtboxGroup)
            {
                HurtBoxGroup hurtBoxGroup = this.hurtboxGroup;
                int hurtBoxesDeactivatorCounter = hurtBoxGroup.hurtBoxesDeactivatorCounter - 1;
                hurtBoxGroup.hurtBoxesDeactivatorCounter = hurtBoxesDeactivatorCounter;
            }
            if (NetworkServer.active)
            {
                base.characterBody.bodyFlags &= ~CharacterBody.BodyFlags.IgnoreFallDamage;
            }
            base.OnExit();
        }

        private void Fire()
        {
            if (!this.hasFired)
            {
                this.hasFired = true;
                if (Util.HasEffectiveAuthority(base.gameObject))
                {
                    DamageInfo damageInfo = new DamageInfo
                    {
                        position = this.target.transform.position,
                        attacker = base.gameObject,
                        inflictor = base.gameObject,
                        damage = this.damageCoefficient * base.damageStat,
                        damageColorIndex = DamageColorIndex.Default,
                        damageType = DamageType.Generic,
                        crit = base.RollCrit(),
                        force = Vector3.zero,
                        procChainMask = default(ProcChainMask),
                        procCoefficient = 1f
                    };
                   
                    this.target.healthComponent.TakeDamage(damageInfo);
                    GlobalEventManager.instance.OnHitEnemy(damageInfo, this.target.gameObject);
                    GlobalEventManager.instance.OnHitAll(damageInfo, this.target.gameObject);

                    

                    EffectManager.SpawnEffect(Modules.Assets.autoHitImpactEffect, new EffectData
                    {
                        origin = this.target.transform.position,
                        rotation = Quaternion.identity,
                        networkSoundEventIndex = Modules.Assets.dashHitSoundEvent.index
                    }, true);
                }
            }




        }

        private bool bufferedSecondary;
        private void ReadInputs()
        {
            if (this.inputBank.skill2.down)
                this.bufferedSecondary = true;
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();

            this.ReadInputs();

            if(base.isGrounded)
            {
                base.GetComponent<KinematicCharacterController.KinematicCharacterMotor>().ForceUnground();
            }

            if (this.prepStopwatch >= this.prepDuration)
            {

                this.stopwatch += Time.fixedDeltaTime;


                Vector3 target = this.lastKnownPosition;

                if (this.stopwatch >= this.duration && this.extraD != 0)
                    this.speed = Dash.extraDistance / this.extraD;

                base.characterDirection.forward = this.direction;
                base.characterMotor.rootMotion += this.direction * this.speed * Time.fixedDeltaTime;
                base.characterMotor.velocity = Vector3.zero;

                if(this.target)
                {
                    float dis = (Util.GetCorePosition(this.target) - base.transform.position).magnitude;
                    if (dis <= hitRange)
                    {
                        this.Fire();
                    }
                }
                

                base.gameObject.layer = LayerIndex.fakeActor.intVal;
                base.characterMotor.Motor.RebuildCollidableLayers();

                if (this.stopwatch >= this.duration)
                {
                    if(this.skillLocator.secondary.CanExecute() && this.bufferedSecondary)
                    {
                        this.skillLocator.secondary.ExecuteIfReady();                       
                    }
                }
                
                if (this.stopwatch >= this.duration + Dash.extraDuration)
                {
                    this.outer.SetNextStateToMain();
                }
                
            }
            else
                this.prepStopwatch += Time.fixedDeltaTime;

        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

    }
}