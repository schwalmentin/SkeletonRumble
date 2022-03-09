using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnParry : Node
{
    public CharacterController characterController;

    public override NodeStates Evaluate()
    {
        if (this.characterController.parryState != WeaponState.Idle)
        {
            return NodeStates.FAILURE;
        }

        return NodeStates.SUCCESS;
    }
}
