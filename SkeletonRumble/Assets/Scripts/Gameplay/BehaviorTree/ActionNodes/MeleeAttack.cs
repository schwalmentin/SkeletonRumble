using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MeleeAttack : Node
{
    public CharacterController characterController;
    [HideInInspector] public AttackStats attackStats;
    private Transform attackTransform;
    public float velocityYSmoothing;
    private bool goHorizontal;
    private AttackDirection currentAttackDirection;
    public float superArmour;

    public override NodeStates Evaluate()
    {
        this.characterController.attackTimer -= Time.deltaTime;

        if (this.characterController.inputControls.inputY > -0.2f)
        {
            this.characterController.controller.collisions.fallingThroughPlatform = false;
        }

        Vector2 vectorToTarget1 = Vector2.up;
        float angle1 = Mathf.Atan2(vectorToTarget1.y, vectorToTarget1.x) * Mathf.Rad2Deg;
        Quaternion q1 = Quaternion.AngleAxis(angle1 - 90, Vector3.forward);
        this.characterController.characterGraphic.transform.rotation = Quaternion.Slerp(this.characterController.characterGraphic.transform.rotation, q1, Time.deltaTime * 20);

        //Weapon State
        switch (this.characterController.weaponState)
        {   
            //Idle
            case WeaponState.Idle:

                //Reset Attack
                this.StartAnticipation();

                //Switch to Anticipation
                this.characterController.currentComboAmount = 1;
                this.CalculateAttackVelocity();

                //Set Animation
                this.characterController.animator.SetInteger("ComboAmount", 1);
                break;

            //Anticipation
            case WeaponState.Anticipation:

                //Anticipate
                this.characterController.animator.SetFloat("AttackAnimationSpeed", 1 - this.characterController.attackTimer / this.attackStats.anticipationTime);
                this.AnticipationMovement();

                //Turn
                if (this.attackStats.anticipationTime * this.attackStats.inputqueueTurn < this.characterController.attackTimer)
                {
                    this.characterController.characterGraphic.transform.localScale = new Vector3(Mathf.Abs(this.characterController.characterGraphic.transform.localScale.x) * this.characterController.inputControls.lookDirection, this.characterController.characterGraphic.transform.localScale.y, this.characterController.characterGraphic.transform.localScale.z);
                }

                //Switch to Strike
                if (this.characterController.attackTimer <= 0)
                {
                    if (this.characterController.gameObject.name == "PlayerController")
                    {
                        //print(this.characterController.attackDirection + " Aticipation " + this.timer);
                    }

                    if (this.goHorizontal)
                    {
                        this.characterController.velocity.x = this.characterController.moveSpeed * this.characterController.currentMeleeWeapon.attackUpXSpeedPercent * this.characterController.inputControls.lookDirection;
                    }

                    //Switch
                    this.characterController.attackTimer += this.attackStats.strikeTime;
                    this.characterController.weaponState = WeaponState.Strike;

                    //Set Animation
                    this.characterController.animator.SetInteger("WeaponState", 2);
                    this.characterController.animator.SetFloat("AttackAnimationSpeed", 0);
                    /////DELETE
                    this.characterController.animator.SetTrigger("Jump");
                    this.characterController.animator.SetBool("Grounded", false);
                    /////DELETE
                }
                break;
            
            //Strike
            case WeaponState.Strike:

                //Strike
                this.characterController.animator.SetFloat("AttackAnimationSpeed", 1 - this.characterController.attackTimer / this.attackStats.strikeTime);
                this.DoDamage(this.attackTransform, this.attackStats.size, this.attackStats.damage, this.attackStats.knockBack, this.attackStats.stunTime, this.attackStats.weightless);

                this.StrikeMovement();

                //Switch to Recovery
                if (this.characterController.attackTimer <= 0)
                {
                    if (this.characterController.gameObject.name == "PlayerController")
                    {
                        //print(this.characterController.attackDirection + " Strike " + this.timer);
                    }

                    //Switch
                    this.characterController.attackTimer += this.attackStats.recoveryTime;
                    this.characterController.weaponState = WeaponState.Recovery;

                    //Set Animation
                    this.characterController.animator.SetInteger("WeaponState", 3);
                    this.characterController.animator.SetFloat("AttackAnimationSpeed", 0);
                }
                break;
            
            //Recovery
            case WeaponState.Recovery:

                //Recover
                this.characterController.animator.SetFloat("AttackAnimationSpeed", 1 - this.characterController.attackTimer / this.attackStats.recoveryTime);
                this.RecoveryMovement();

                //Do Combo
                if (!this.characterController.canParryCombo && this.characterController.canPerformCombo && this.characterController.currentComboAmount < this.characterController.currentMeleeWeapon.comboAmount && this.characterController.attackTimer < this.attackStats.recoveryTime * this.characterController.currentMeleeWeapon.inputqueueRecoveryPercentage)
                {
                    //Set Combo
                    this.characterController.currentComboAmount++;

                    if (this.characterController.currentComboAmount == this.characterController.currentMeleeWeapon.comboAmount)
                    {
                        this.characterController.heavyHit = true;
                    }

                    //Reset Attack
                    this.StartAnticipation();

                    this.characterController.canPerformCombo = false;

                    this.characterController.animator.SetInteger("ComboAmount", this.characterController.currentComboAmount);
                    this.CalculateAttackVelocity();
                    break;
                }

                //Switch to Idle
                if (this.characterController.attackTimer <= 0)
                {
                    this.ResetAttack();

                    return NodeStates.SUCCESS;
                }

                break;
        }
        this.characterController.comboCount.text = this.characterController.currentComboAmount.ToString();
        
        this.characterController.inputqueueAnticipationTime = this.attackStats.anticipationTime * this.characterController.currentMeleeWeapon.inputqueueAnticipationPercentage;
        this.characterController.inputqueueRecoveryTime = this.attackStats.recoveryTime * this.characterController.currentMeleeWeapon.inputqueueRecoveryPercentage;

        this.characterController.controller.Move(this.characterController.velocity * Time.deltaTime, false);

        return NodeStates.RUNNING;
    }

    /// <summary>
    /// Deals damage to damageable objects.
    /// </summary>
    private void DoDamage(Transform attackTransform, Vector3 weaponSize, float damage, Vector3 knockBack, float stunTime, bool weightless)
    {
        weaponSize = this.characterController.currentMeleeWeapon.attackFront.size;
        Collider[] hittedObjects = Physics.OverlapBox(attackTransform.position, weaponSize / 2, attackTransform.rotation, this.characterController.damageMask);

        foreach (Collider item in hittedObjects)
        {
            IDamage objectToDamage = item.GetComponentInParent<IDamage>();

            if (objectToDamage == null)
            {
                continue;
            }

            if (objectToDamage == this.characterController.damagedObject || objectToDamage == this.characterController.gameObject.GetComponent<IDamage>())
            {
                continue;
            }

            if (this.currentAttackDirection == AttackDirection.Down)
            {
                int knockbackDirection = this.characterController.transform.position.x < item.transform.position.x ? 1 : -1;
                knockBack.x *= knockbackDirection;
            }

            this.characterController.hitTarget = true;
            objectToDamage.GetDamage(damage, knockBack, stunTime, weightless, this);

            this.characterController.damagedObject = objectToDamage;
        }
    }

    private void StartAnticipation()
    {
        //Reset Attack
        this.characterController.damagedObject = null;
        this.currentAttackDirection = this.characterController.attackDirection;
        this.superArmour = this.characterController.currentMeleeWeapon.superArmour;
        //print(this.characterController.currentMeleeWeapon.name + ": " + this.superArmour);
        this.characterController.audioManager.PlayAttackSound(this.characterController.audioID, this.characterController.weaponType);
        this.characterController.airCombo++;

        //Weapon direction
        switch (this.characterController.attackDirection)
        {
            //Up
            case AttackDirection.Up:

                //Set Up Attack
                this.attackTransform = this.characterController.attackPositions[0];
                this.attackStats = this.characterController.heavyHit ? this.characterController.currentMeleeWeapon.attackHeavyUp : this.characterController.currentMeleeWeapon.attackUp;
                this.characterController.attackDirectionVector = Vector2.up;

                //Set Attack Animation
                this.characterController.animator.SetInteger("AttackDirection", 1);
                break;

            //Front
            case AttackDirection.Front:

                //Set Front Attack
                this.attackTransform = this.characterController.attackPositions[1];
                this.attackStats = this.characterController.heavyHit ? this.characterController.currentMeleeWeapon.attackHeavyFront : this.characterController.currentMeleeWeapon.attackFront;
                this.characterController.attackDirectionVector = Vector2.right * this.characterController.inputControls.lookDirection;

                //Set Front Attack Animation
                this.characterController.animator.SetInteger("AttackDirection", 2);
                break;

            //Down
            case AttackDirection.Down:

                //Heavy
                if (this.characterController.controller.collisions.below)
                {
                    //Set Heavy Down Attack
                    this.attackStats = this.characterController.currentMeleeWeapon.attackHeavyDown;
                    this.characterController.heavyHit = true;
                    this.attackTransform = this.characterController.attackPositions[1];
                    this.characterController.attackDirectionVector = new Vector2(this.characterController.velocity.x, (Vector2.down * this.characterController.currentMeleeWeapon.attackDown.attackForce).y);

                    //Set Heavy Down Attack Animation
                    this.characterController.animator.SetInteger("AttackDirection", 4);

                    ///////////Delete///////////
                    foreach (SpriteRenderer renderer in this.characterController.attackSprites)
                    {
                        renderer.color = new Color(0.474f, 0.168f, 0.207f, 1);
                    }
                    ///////////Delete///////////
                    break;
                }

                //Set Down Attack
                this.characterController.controller.collisions.fallingThroughPlatform = true;
                this.attackStats = this.characterController.currentMeleeWeapon.attackDown;
                this.attackTransform = this.characterController.attackPositions[2];
                this.characterController.attackDirectionVector = this.characterController.heavyHit
                    ? Vector2.right * this.characterController.inputControls.lookDirection
                    : Vector2.down;

                //Set Down Attack Animation
                this.characterController.animator.SetInteger("AttackDirection", 3);
                break;
        }

        //Switch to Anticipation
        this.characterController.weaponState = WeaponState.Anticipation;
        this.characterController.attackTimer = this.attackStats.anticipationTime;

        //Set Animation
        this.characterController.animator.SetInteger("WeaponState", 1);
        this.characterController.animator.SetFloat("AttackAnimationSpeed", 0);

        if (this.characterController.heavyHit)
        {
            ///////////Delete///////////
            foreach (SpriteRenderer renderer in this.characterController.attackSprites)
            {
                renderer.color = new Color(0.474f, 0.168f, 0.207f, 1);
            }
            ///////////Delete///////////
        }
    }

    public void ResetAttack()
    {
        if (this.characterController.gameObject.name == "PlayerController")
        {
            //print(this.characterController.attackDirection + " Recovery " + this.timer);
        }

        //Switch
        this.characterController.weaponState = WeaponState.Idle;
        this.characterController.canPerformCombo = false;
        this.characterController.heavyHit = false;
        this.characterController.canParryCombo = false;
        this.characterController.controller.collisions.fallingThroughPlatform = false;
        this.superArmour = 0;

        ///////////Delete///////////
        foreach (SpriteRenderer renderer in this.characterController.attackSprites)
        {
            renderer.color = new Color(0.286f, 0.749f, 0.96f, 1);
        }
        ///////////Delete///////////

        //Set Animation
        this.characterController.animator.SetInteger("WeaponState", 0);
        this.characterController.animator.SetInteger("ComboAmount", 0);
        this.characterController.animator.SetInteger("AttackDirection", 0);
    }

    private void CalculateAttackVelocity()
    {
        this.characterController.attackVelocity = this.attackStats.attackLength / this.attackStats.strikeTime;

        if (this.characterController.gameObject.name == "PlayerController")
        {
            //print(this.characterController.attackVelocity);
        }

        switch (this.currentAttackDirection)
        {
            case AttackDirection.Front:
                this.goHorizontal = false;
                this.attackStats.knockBack.x *= this.characterController.inputControls.lookDirection;
                break;

            case AttackDirection.Up:
                if (this.characterController.heavyHit)
                {
                    this.attackStats.knockBack.x *= this.characterController.inputControls.lookDirection;
                    break;
                }
                this.goHorizontal = Mathf.Abs(this.characterController.velocity.x) > this.characterController.moveSpeed * this.characterController.currentMeleeWeapon.attackUpXNeededSpeedPercent;

                if (this.goHorizontal)
                {
                    this.attackStats.knockBack.x = this.attackStats.attackForce * this.characterController.inputControls.lookDirection;
                }
                break;

            case AttackDirection.Down:
                this.goHorizontal = false;
                break;
        }
    }

    private void AnticipationMovement()
    {
        switch (this.currentAttackDirection)
        {
            case AttackDirection.Front:

                if (this.characterController.currentComboAmount > 1)
                {
                    this.characterController.velocity.y -= this.attackStats.attackGravity * Time.deltaTime;
                }
                else
                {
                    this.characterController.velocity.y = Mathf.SmoothDamp(this.characterController.velocity.y, 0, ref this.velocityYSmoothing, this.attackStats.attackAccelerationY);
                }
                this.characterController.velocity.x = Mathf.SmoothDamp(this.characterController.velocity.x, 0, ref this.characterController.velocityXSmoothing, this.attackStats.attackAccelerationX);
                break;

            case AttackDirection.Up:
                this.characterController.velocity.y = Mathf.SmoothDamp(this.characterController.velocity.y, 0, ref this.velocityYSmoothing, this.attackStats.attackAccelerationY);
                this.characterController.velocity.x = Mathf.SmoothDamp(this.characterController.velocity.x, 0, ref this.characterController.velocityXSmoothing, this.attackStats.attackAccelerationX);
                break;

            case AttackDirection.Down:
                this.characterController.velocity.y = Mathf.SmoothDamp(this.characterController.velocity.y, 0, ref this.velocityYSmoothing, this.attackStats.attackAccelerationY);
                this.characterController.velocity.x = Mathf.SmoothDamp(this.characterController.velocity.x, 0, ref this.characterController.velocityXSmoothing, this.attackStats.attackAccelerationX);
                break;
        }
    }

    private void StrikeMovement()
    {
        switch (this.currentAttackDirection)
        {
            case AttackDirection.Front:
                this.characterController.velocity.y = Mathf.SmoothDamp(this.characterController.velocity.y, 0, ref this.velocityYSmoothing, this.attackStats.attackAccelerationY);
                this.characterController.velocity.x = this.characterController.attackVelocity * this.characterController.inputControls.lookDirection;
                break;

            case AttackDirection.Up:
                this.characterController.velocity.y = this.characterController.attackVelocity;
                this.characterController.velocity.x = Mathf.SmoothDamp(this.characterController.velocity.x, 0, ref this.characterController.velocityXSmoothing, this.attackStats.attackAccelerationX);
                break;

            case AttackDirection.Down:
                if (this.characterController.heavyHit)
                {
                    this.characterController.velocity.y = Mathf.SmoothDamp(this.characterController.velocity.y, 0, ref this.velocityYSmoothing, this.attackStats.attackAccelerationY);
                    this.characterController.velocity.x = this.characterController.attackVelocity * this.characterController.inputControls.lookDirection;
                }
                else
                {
                    this.characterController.velocity.y = -this.characterController.attackVelocity;
                    this.characterController.velocity.x = Mathf.SmoothDamp(this.characterController.velocity.x, 0, ref this.characterController.velocityXSmoothing, this.attackStats.attackAccelerationX);
                }
                break;
        }
    }

    private void RecoveryMovement()
    {
        switch (this.currentAttackDirection)
        {
            case AttackDirection.Front:
                this.characterController.velocity.y -= this.attackStats.attackGravity * Time.deltaTime;
                this.characterController.velocity.x = Mathf.SmoothDamp(this.characterController.velocity.x, 0, ref this.characterController.velocityXSmoothing, this.attackStats.attackAccelerationX);
                break;

            case AttackDirection.Up:
                this.characterController.velocity.y -= this.attackStats.attackGravity * Time.deltaTime;
                break;

            case AttackDirection.Down:
                this.characterController.velocity.x = Mathf.SmoothDamp(this.characterController.velocity.x, 0, ref this.characterController.velocityXSmoothing, this.attackStats.attackAccelerationX);
                break;
        }
    }
}
