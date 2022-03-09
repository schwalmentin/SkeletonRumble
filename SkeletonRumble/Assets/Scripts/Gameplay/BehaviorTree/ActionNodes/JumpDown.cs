using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpDown : Node
{
    public CharacterController characterController;

    public override NodeStates Evaluate()
    {
        if (this.characterController.controller.collisions.left || this.characterController.controller.collisions.right)
        {
            float x = Mathf.Abs(this.characterController.inputControls.inputX) >= 0.7f ? 1 : this.characterController.inputControls.inputX;
            x *= this.characterController.inputControls.lookDirection;

            if (this.characterController.wallDirX == x)
            {
                this.characterController.velocity.x = -this.characterController.wallDirX * this.characterController.wallJumpClimb.x;
                this.characterController.velocity.y = this.characterController.wallJumpClimb.y;

                this.characterController.wallrunTime = this.characterController.wallrunTimeJump;
                this.characterController.wallrunVelocity = this.characterController.wallrunVelocityJump;

                this.characterController.animator.SetTrigger("Walljump");
                this.characterController.audioManager.PlayFootstepSound(this.characterController.audioID, false);
                return NodeStates.SUCCESS;
            }
            else
            {
                if (this.characterController.inputControls.inputX == 0)
                {
                    this.characterController.velocity.x = -this.characterController.wallDirX * this.characterController.wallJumpOff.x;
                    this.characterController.velocity.y = this.characterController.wallJumpOff.y;

                    this.characterController.animator.SetTrigger("Jump");
                    this.characterController.audioManager.PlayFootstepSound(this.characterController.audioID, false);
                    return NodeStates.SUCCESS;
                }
                else
                {
                    this.characterController.velocity.x = -this.characterController.wallDirX * this.characterController.wallJumpLeap.x;
                    this.characterController.velocity.y = this.characterController.wallJumpLeap.y;

                    this.characterController.animator.SetTrigger("Jump");
                    this.characterController.audioManager.PlayFootstepSound(this.characterController.audioID, false);
                    return NodeStates.SUCCESS;
                }
            }
        }

        this.characterController.jumpCounter++;

        if (this.characterController.jumpCounter <= this.characterController.maxJumpCounter)
        {
            if (this.characterController.controller.collisions.slidingDownMaxSlope)
            {
                if (this.characterController.inputControls.inputX != -Mathf.Sign(this.characterController.controller.collisions.slopeNormal.x))
                {

                    this.characterController.velocity.y = this.characterController.maxJumpVelocity * this.characterController.controller.collisions.slopeNormal.y;
                    this.characterController.velocity.x = this.characterController.maxJumpVelocity * this.characterController.controller.collisions.slopeNormal.x;
                    return NodeStates.SUCCESS;

                }
            }
            else
            {
                if (this.characterController.controller.collisions.right || this.characterController.controller.collisions.left || this.characterController.inputControls.inputY <= -0.7f)
                {
                    return NodeStates.SUCCESS;
                }

                this.characterController.velocity.y = this.characterController.maxJumpVelocity;
                this.characterController.animator.SetTrigger("Jump");

                if (this.characterController.controller.collisions.below)
                {
                    this.characterController.audioManager.PlayFootstepSound(this.characterController.audioID, this.characterController.groundType);
                }

                return NodeStates.SUCCESS;
            }
        }

        return NodeStates.SUCCESS;
    }
}
