using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallrunDown : Node
{
    public CharacterController characterController;

    public override NodeStates Evaluate()
    {
        if (!this.characterController.controller.collisions.right && !this.characterController.controller.collisions.left)
        {
            this.characterController.wallrunning = false;
            return NodeStates.FAILURE;
        }

        if (this.characterController.wallrunTime <= 0)
        {
            this.characterController.wallrunning = false;
            return NodeStates.SUCCESS;
        }

        this.characterController.wallrunning = true;
        this.characterController.wallrunTime -= Time.deltaTime;

        if (this.characterController.velocity.y < this.characterController.wallrunVelocity)
        {
            this.characterController.velocity.y = this.characterController.wallrunVelocity;
        }

        return NodeStates.RUNNING;
    }
}
