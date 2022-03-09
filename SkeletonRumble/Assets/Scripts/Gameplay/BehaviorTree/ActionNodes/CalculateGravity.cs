using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalculateGravity : Node
{
    public CharacterController characterController;

    public override NodeStates Evaluate()
    {
        this.characterController.velocity.y += this.characterController.gravity * Time.deltaTime;

        if (this.characterController.velocity.y <= 0 && this.characterController.velocity.y > -this.characterController.maxgravity)
        {
            this.characterController.velocity.y -= this.characterController.gravityMultiplyer * Time.deltaTime;
        }

        return NodeStates.RUNNING;
    }
}
