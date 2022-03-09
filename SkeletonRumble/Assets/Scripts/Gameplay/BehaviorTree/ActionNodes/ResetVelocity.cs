using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetVelocity : Node
{
    public CharacterController characterController;

    public override NodeStates Evaluate()
    {
        if (this.characterController.controller.collisions.above || this.characterController.controller.collisions.below)
        {
            if (this.characterController.controller.collisions.slidingDownMaxSlope)
            {
                this.characterController.velocity.y += this.characterController.controller.collisions.slopeNormal.y * -this.characterController.gravity * Time.deltaTime;
                return NodeStates.SUCCESS;
            }
            else
            {
                this.characterController.velocity.y = 0;
                return NodeStates.SUCCESS;
            }
        }

        return NodeStates.FAILURE;
    }
}
