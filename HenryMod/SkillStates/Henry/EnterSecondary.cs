using System;
using System.Collections.Generic;
using System.Text;
using RoR2;
using EntityStates;

namespace YassoMod.SkillStates.Henry
{
    public class EnterSecondary : BaseSkillState
    {
        public override void OnEnter()
        {
            base.OnEnter();

            bool empowered = base.characterBody.GetBuffCount(Modules.Buffs.qStackBuff) >= 2;

            if(empowered)
                base.characterBody.ClearTimedBuffs(Modules.Buffs.qStackBuff);

            EntityStateMachine b = null;
            EntityStateMachine[] components = base.gameObject.GetComponents<EntityStateMachine>();
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i].customName == "Body")
                {
                    b = components[i];
                    
                    break;
                }
            }

            if (b && b.state is Dash)
            {
                b.SetNextStateToMain();
                this.outer.SetNextState(new DashSpin { empowered = empowered } );
                return;
            }

            if (empowered)
            {
                this.outer.SetNextState(new Secondary3());
                return;
            }               
            else
                this.outer.SetNextState(new Secondary1());
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
