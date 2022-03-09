using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateAnimations : Node
{
    public CharacterController characterController;

    public override NodeStates Evaluate()
    {
        //Check if animator is not null
        if (this.characterController.animator == null)
        {
            print("Player Animator is null");
            return NodeStates.FAILURE;
        }

        //Set Grounded
        this.characterController.animator.SetBool("Grounded", this.characterController.controller.collisions.below);

        //Set Wallsliding
        this.characterController.animator.SetBool("Wallsliding", this.characterController.wallSliding);

        //Set Wallrunning
        this.characterController.animator.SetBool("Wallrunning", this.characterController.wallrunning);

        //Set InputX
        if ((Mathf.Abs(this.characterController.inputControls.inputX) < 0.2f || this.characterController.controller.collisions.right || this.characterController.controller.collisions.left || this.characterController.controller.collisions.slidingDownMaxSlope) && this.characterController.timeToIdle < 0)
        {
            this.characterController.timeToMove = this.characterController.timeToMoveValue;
            this.characterController.animator.SetFloat("InputX", 0);
        }
        else
        {
            if (Mathf.Abs(this.characterController.inputControls.inputX) > 0.7f)
            {
                if (!this.characterController.controller.collisions.right && !this.characterController.controller.collisions.left)
                {
                    this.characterController.timeToIdle = this.characterController.timeToIdleValue;
                }

                this.characterController.timeToMove = this.characterController.timeToMoveValue;
                this.characterController.animator.SetFloat("InputX", 1);
            }

            this.characterController.timeToMove -= Time.deltaTime;
            this.characterController.timeToIdle -= Time.deltaTime;

            if (this.characterController.timeToMove <= 0)
            {
                this.characterController.animator.SetFloat("InputX", Mathf.Abs(this.characterController.inputControls.inputX));
            }
        }
        if (!this.characterController.isAbleToMove)
        {
            this.characterController.animator.SetFloat("InputX", 0);
        }

        //Set Fall
        bool falling = !this.characterController.controller.collisions.below && this.characterController.velocity.y <= 0;
        this.characterController.animator.SetBool("Falling", falling);

        //Set foot positions when grounded
        if ((this.characterController.controller.collisions.below && this.characterController.inputControls.positiveInputX < 0.2f) && this.characterController.isAbleToMove && this.characterController.dashTime > 0 || ((this.characterController.controller.collisions.right || this.characterController.controller.collisions.left) && this.characterController.controller.collisions.below) && !this.characterController.wallSliding && this.characterController.dashTime > 0 && this.characterController.isAbleToMove)
        {
            this.characterController.rigs[2].weight = Mathf.MoveTowards(this.characterController.rigs[2].weight, 1, this.characterController.footBlendTime * Time.deltaTime);
            this.characterController.rigs[3].weight = Mathf.MoveTowards(this.characterController.rigs[3].weight, 1, this.characterController.footBlendTime * Time.deltaTime);

            Vector2 rightFootPos;
            RaycastHit2D rightFootHit;

            foreach (Vector2 item in this.characterController.footSpacing)
            {
                rightFootPos = new Vector2(this.characterController.transform.position.x - (item.x * this.characterController.inputControls.lookDirection), this.characterController.transform.position.y + this.characterController.yOffset);
                rightFootHit = Physics2D.Raycast(rightFootPos, Vector2.down, item.y, this.characterController.controller.collisionMask);
                bool obstacleToTall = Physics2D.OverlapCircle(rightFootPos, 0.02f, this.characterController.controller.collisionMask);

                if (rightFootHit && !obstacleToTall)
                {
                    this.characterController.rigController[2].position = new Vector2(rightFootHit.point.x, rightFootHit.point.y + this.characterController.footOffset);
                    break;
                }
            }

            Vector2 leftFootPos;
            RaycastHit2D leftFootHit;

            foreach (Vector2 item in this.characterController.footSpacing)
            {
                leftFootPos = new Vector2(this.characterController.transform.position.x + (item.x * this.characterController.inputControls.lookDirection), this.characterController.transform.position.y + this.characterController.yOffset);
                leftFootHit = Physics2D.Raycast(leftFootPos, Vector2.down, item.y, this.characterController.controller.collisionMask);
                bool obstacleToTall = Physics2D.OverlapCircle(leftFootPos, 0.02f, this.characterController.controller.collisionMask);

                if (leftFootHit && !obstacleToTall)
                {
                    this.characterController.rigController[3].position = new Vector2(leftFootHit.point.x, leftFootHit.point.y + this.characterController.footOffset);
                    break;
                }
            }
        }
        else
        {
            this.characterController.rigs[2].weight = Mathf.MoveTowards(this.characterController.rigs[2].weight, 0, this.characterController.footBlendTime * Time.deltaTime);
            this.characterController.rigs[3].weight = Mathf.MoveTowards(this.characterController.rigs[3].weight, 0, this.characterController.footBlendTime * Time.deltaTime);

            //this.rigs[2].weight = 0;
            //this.rigs[3].weight = 0;
        }

        return NodeStates.SUCCESS;
    }
}
