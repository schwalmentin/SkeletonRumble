using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationManager : MonoBehaviour
{
    private CharacterController characterController;
    public Transform splineOneJoint;
    public Transform hipBindJoint;

    private void Start()
    {
        this.characterController = this.GetComponentInParent<CharacterController>();
    }

    private void Update()
    {
        if (this.splineOneJoint != null && this.hipBindJoint != null)
        {
            this.splineOneJoint.position = this.hipBindJoint.position;
        }
    }

    public void OnFootstep()
    {
        this.characterController.audioManager.PlayFootstepSound(this.characterController.audioID, this.characterController.groundType);
    }
}
