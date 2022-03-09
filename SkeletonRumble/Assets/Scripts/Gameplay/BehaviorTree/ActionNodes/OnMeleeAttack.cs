using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnMeleeAttack : Node
{
    public CharacterController characterController;

    public override NodeStates Evaluate()
    {
        //Check if attack up
        if (this.characterController.inputControls.inputY > this.characterController.attackUpInput && this.characterController.canAttackUp)
        {
            this.characterController.canAttackUp = false;

            //Set Parry Combo
            if (!this.characterController.canParryCombo &&
                this.characterController.parried &&
                this.characterController.currentComboAmount != this.characterController.currentMeleeWeapon.comboAmount &&
                (this.characterController.parryState == WeaponState.Strike ||
                (this.characterController.parryState == WeaponState.Recovery &&
                this.characterController.currentMeleeWeapon.parryRecoveryTime * this.characterController.currentMeleeWeapon.inputQueueParryPercentage < this.characterController.parryDuration)))
            {
                this.characterController.canParryCombo = true;
                this.characterController.attackDirection = AttackDirection.Up;
                return NodeStates.SUCCESS;
            }

            if (this.characterController.parryState != WeaponState.Idle)
            {
                return NodeStates.FAILURE;
            }

            //Set Combo
            if ((this.characterController.weaponState == WeaponState.Anticipation && this.characterController.inputqueueAnticipationTime > this.characterController.attackTimer) ||
            this.characterController.weaponState == WeaponState.Strike ||
            (this.characterController.weaponState == WeaponState.Recovery && this.characterController.inputqueueRecoveryTime < this.characterController.attackTimer))
            {
                //Hit To Combo
                if (!this.characterController.hitTarget && this.characterController.useHitToCombo)
                {
                    return NodeStates.FAILURE;
                }
                else
                {
                    this.characterController.hitTarget = false;
                }

                if (this.characterController.attackDirection == AttackDirection.Front && !this.characterController.performingCombo && this.characterController.currentComboAmount == this.characterController.currentMeleeWeapon.comboAmount - 1)
                {
                    this.characterController.canPerformCombo = true;
                    this.characterController.attackDirection = AttackDirection.Up;
                    return NodeStates.SUCCESS;
                }
            }

            //Return when weaponstate is equals idle
            if (this.characterController.weaponState != WeaponState.Idle)
            {
                return NodeStates.FAILURE;
            }

            //Set Attack Up
            this.characterController.attackDirection = AttackDirection.Up;

            return NodeStates.SUCCESS;
        }

        //Check if attack down
        if (this.characterController.inputControls.inputY < -this.characterController.attackUpInput)
        {
            //Return when weaponstate is equals idle
            if (this.characterController.weaponState != WeaponState.Idle || this.characterController.parryState != WeaponState.Idle)
            {
                return NodeStates.FAILURE;
            }

            //Set Attack Down
            this.characterController.attackDirection = AttackDirection.Down;

            return NodeStates.SUCCESS;
        }

        //Set Parry Combo
        if (!this.characterController.canParryCombo &&
            this.characterController.parried &&
            this.characterController.currentComboAmount != this.characterController.currentMeleeWeapon.comboAmount &&
            (this.characterController.parryState == WeaponState.Strike ||
            (this.characterController.parryState == WeaponState.Recovery &&
            this.characterController.currentMeleeWeapon.parryRecoveryTime * this.characterController.currentMeleeWeapon.inputQueueParryPercentage < this.characterController.parryDuration)))
        {
            this.characterController.canParryCombo = true;
            this.characterController.attackDirection = AttackDirection.Front;

            return NodeStates.SUCCESS;
        }

        if (this.characterController.parryState != WeaponState.Idle)
        {
            return NodeStates.FAILURE;
        }

        //Set Combo
        if ((this.characterController.weaponState == WeaponState.Anticipation && this.characterController.inputqueueAnticipationTime > this.characterController.attackTimer) ||
            this.characterController.weaponState == WeaponState.Strike ||
            (this.characterController.weaponState == WeaponState.Recovery && this.characterController.inputqueueRecoveryTime < this.characterController.attackTimer))
        {
            //Check if attack front
            if (this.characterController.attackDirection == AttackDirection.Down)
            {
                return NodeStates.FAILURE;
            }

            //Hit To Combo
            if (!this.characterController.hitTarget && this.characterController.useHitToCombo)
            {
                return NodeStates.FAILURE;
            }
            else
            {
                this.characterController.hitTarget = false;
            }

            //Set Up-Front Combo
            if (this.characterController.attackDirection == AttackDirection.Up)
            {
                this.characterController.attackDirection = AttackDirection.Front;

                this.characterController.canPerformCombo = true;
                return NodeStates.SUCCESS;
            }

            this.characterController.canPerformCombo = true;
        }

        this.characterController.attackDirection = AttackDirection.Front;

        return NodeStates.SUCCESS;
    }
}
