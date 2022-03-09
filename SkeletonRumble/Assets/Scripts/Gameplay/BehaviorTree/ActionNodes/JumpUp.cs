using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpUp : Node
{
    public CharacterController characterController;
    public override NodeStates Evaluate()
    {
        if (this.characterController.velocity.y > this.characterController.minJumpVelocity)
        {
            this.characterController.velocity.y = this.characterController.minJumpVelocity;
            return NodeStates.SUCCESS;
        }

        return NodeStates.FAILURE;
    }
}
