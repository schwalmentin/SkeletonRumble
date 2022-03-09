using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash : Node
{
    public CharacterController characterController;

    public override NodeStates Evaluate()
    {
        if (this.characterController.dashTime > 0)
        {
            this.characterController.invincibleTime -= Time.deltaTime;
            this.characterController.dashTime -= Time.deltaTime;

            this.characterController.velocity = this.characterController.dodgeRoll
                ? new Vector2(this.characterController.dodgeRollVelocity * this.characterController.dashDirectionX, 0)
                : (this.characterController.dashDirection.normalized * this.characterController.dashVelocity);
            return NodeStates.RUNNING;
        }

        this.characterController.velocity /= 2;
        this.characterController.invincibleTime = 0;
        this.characterController.y = this.characterController.transform.position;
        return NodeStates.SUCCESS;
    }

    public void ResetDash()
    {
        this.characterController.dashing = false;
        this.characterController.dashCooldown = 0;
        this.characterController.dashTime = 0;
        this.characterController.invincibleTime = 0;
    }
}
