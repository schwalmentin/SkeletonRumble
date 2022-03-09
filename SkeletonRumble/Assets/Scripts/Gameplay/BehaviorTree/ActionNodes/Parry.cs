using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parry : Node
{
    public CharacterController characterController;

    public override NodeStates Evaluate()
    {
        switch (this.characterController.parryState)
        {
            case WeaponState.Idle:

                //Switch to Anticipation
                this.characterController.parryDuration = this.characterController.currentMeleeWeapon.parryAnticipationTime;
                this.characterController.parryState = WeaponState.Anticipation;

                //Set Animation
                this.characterController.animator.SetBool("Parry", true);
                this.characterController.animator.SetInteger("ParryState", 1);
                this.characterController.animator.SetFloat("ParryAnimationSpeed", 0);
                break;

            case WeaponState.Anticipation:

                //Anticipate
                this.characterController.parryDuration -= Time.deltaTime;
                this.characterController.animator.SetFloat("ParryAnimationSpeed", 1 - this.characterController.parryDuration / this.characterController.currentMeleeWeapon.parryAnticipationTime);

                //Switch to Strike
                if (this.characterController.parryDuration <= 0)
                {
                    this.characterController.parryDuration = this.characterController.currentMeleeWeapon.parryStrikeTime;
                    this.characterController.parryState = WeaponState.Strike;

                    //Set Animation
                    this.characterController.animator.SetInteger("ParryState", 2);
                    this.characterController.animator.SetFloat("ParryAnimationSpeed", 0);
                }

                break;

            case WeaponState.Strike:

                //Strike
                this.characterController.parryDuration -= Time.deltaTime;
                this.PerformParry();
                this.characterController.animator.SetFloat("ParryAnimationSpeed", 1 - this.characterController.parryDuration / this.characterController.currentMeleeWeapon.parryStrikeTime);

                //Switch to Strike
                if (this.characterController.parryDuration <= 0)
                {
                    this.characterController.parryDuration = this.characterController.currentMeleeWeapon.parryRecoveryTime;
                    this.characterController.parryState = WeaponState.Recovery;

                    //Set Animation
                    this.characterController.animator.SetInteger("ParryState", 3);
                    this.characterController.animator.SetFloat("ParryAnimationSpeed", 0);
                }

                break;
            case WeaponState.Recovery:

                //Recover
                this.characterController.parryDuration -= Time.deltaTime;
                this.characterController.animator.SetFloat("ParryAnimationSpeed", 1 - this.characterController.parryDuration / this.characterController.currentMeleeWeapon.parryRecoveryTime);

                if (this.characterController.parryDuration <= this.characterController.currentMeleeWeapon.parryRecoveryTime * this.characterController.currentMeleeWeapon.inputQueueParryPercentage && this.characterController.canParryCombo)
                {
                    this.SwitchToIdle();
                    this.characterController.heavyHit = true;
                    this.characterController.currentComboAmount = this.characterController.currentMeleeWeapon.comboAmount - 1;
                    print("//////////////////////////////ParryDuration " + this.characterController.parryDuration + "<=" + this.characterController.currentMeleeWeapon.parryRecoveryTime * this.characterController.currentMeleeWeapon.inputQueueParryPercentage);
                    return NodeStates.SUCCESS;
                }

                //Switch to Idle
                if (this.characterController.parryDuration <= 0)
                {
                    this.SwitchToIdle();
                    return NodeStates.SUCCESS;
                }

                break;
        }

        return NodeStates.RUNNING;
    }

    private void PerformParry()
    {
        Collider[] weapons = Physics.OverlapSphere(this.characterController.parryTransform.position, this.characterController.currentMeleeWeapon.parryRadius, this.characterController.weaponMask);

        foreach (Collider weapon in weapons)
        {
            CharacterController cc = weapon.GetComponentInParent<CharacterController>();

            if (cc == null)
            {
                continue;
            }

            if (cc.weaponState != WeaponState.Strike)
            {
                continue;
            }

            IDamage damage = weapon.GetComponentInParent<IDamage>();

            if (damage == null)
            {
                continue;
            }

            damage.GetStunned(this.characterController.currentMeleeWeapon.stunTime);
            this.characterController.parried = true;
        }
    }

    private void SwitchToIdle()
    {
        this.characterController.parryState = WeaponState.Idle;
        this.characterController.parried = false;

        //Set Animation
        this.characterController.animator.SetInteger("ParryState", 0);
        this.characterController.animator.SetBool("Parry", false);
    }

    public void ResetParry()
    {
        this.SwitchToIdle();
    }
}
