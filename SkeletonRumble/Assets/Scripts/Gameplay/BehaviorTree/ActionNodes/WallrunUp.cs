using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallrunUp : Node
{
    public CharacterController characterController;

    public override NodeStates Evaluate()
    {
        this.characterController.wallrunTime = 0;
        this.characterController.wallrunning = false;
        return NodeStates.SUCCESS;
    }
}
