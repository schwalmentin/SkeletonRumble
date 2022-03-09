using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnDash : Node
{
    public CharacterController characterController;

    public override NodeStates Evaluate()
    {
        if (this.characterController.dashTime > 0 || this.characterController.dashCooldown > 0 || this.characterController.dashCounter >= this.characterController.maxDashCounter)
        {
            return NodeStates.FAILURE;
        }

        this.characterController.dashCounter++;
        this.characterController.invincibleTime = this.characterController.inviciblePercent * this.characterController.dashTimeValue;
        this.characterController.dashCooldown = this.characterController.dashCoolDownValue;
        this.characterController.dodgeRoll = false;

        if (this.characterController.controller.collisions.below)
        {
            float dashAngle = this.characterController.controller.collisions.slopeNormal != Vector2.zero
                ? Vector2.Dot(this.characterController.controller.collisions.slopeNormal, this.characterController.dashDirection)
                : Vector2.Dot(Vector2.up, this.characterController.dashDirection);

            this.characterController.dodgeRoll = dashAngle < this.characterController.dashAngle;
        }

        if (this.characterController.dodgeRoll)
        {
            this.characterController.dashTime = this.characterController.dodgeRollTimeValue;
            this.characterController.dashDirectionX = this.characterController.inputControls.lookDirection;
            this.characterController.animator.SetTrigger("Dodgeroll");
        }
        else
        {
            this.characterController.dashTime = this.characterController.dashTimeValue;
            this.characterController.animator.SetTrigger("Dash");
        }
        this.characterController.x = this.characterController.transform.position;
        return NodeStates.SUCCESS;
    }
}
