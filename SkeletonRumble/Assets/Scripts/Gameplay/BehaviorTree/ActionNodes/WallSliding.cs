using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSliding : Node
{
    public CharacterController characterController;

    public override NodeStates Evaluate()
    {
        this.characterController.wallDirX = (this.characterController.controller.collisions.left) ? -1 : 1;
        this.characterController.wallSliding = false;

        if ((this.characterController.controller.collisions.left || this.characterController.controller.collisions.right) && !this.characterController.controller.collisions.below && this.characterController.velocity.y < 0)
        {
            this.characterController.wallSliding = true;

            if (this.characterController.velocity.y < -this.characterController.wallSlideSpeedMax)
            {
                this.characterController.velocity.y = -this.characterController.wallSlideSpeedMax;
            }

            if (this.characterController.timeToWallUnstick > 0)
            {
                this.characterController.velocityXSmoothing = 0;
                this.characterController.velocity.x = 0;

                if (this.characterController.inputControls.inputX != this.characterController.wallDirX && this.characterController.inputControls.inputX != 0)
                {
                    this.characterController.timeToWallUnstick -= Time.deltaTime;
                }
                else
                {
                    this.characterController.timeToWallUnstick = this.characterController.wallStickTime;
                }
            }
            else
            {
                this.characterController.timeToWallUnstick = this.characterController.wallStickTime;
            }

            return NodeStates.SUCCESS;
        }

        return NodeStates.FAILURE;
    }
}
