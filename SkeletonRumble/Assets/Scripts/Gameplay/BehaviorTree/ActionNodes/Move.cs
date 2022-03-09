using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : Node
{
    public CharacterController characterController;
    private bool collided;

    public override NodeStates Evaluate()
    {
        this.characterController.goTroughTimer = this.characterController.inputControls.lookDirection != this.characterController.inputControls.lastLookDirection ? this.characterController.goTroughTime : this.characterController.goTroughTimer;

        this.Colliding();

        if (this.collided)
        {
            return NodeStates.SUCCESS;
        }

        this.characterController.moveSpeed = this.characterController.inputControls.positiveInputX < this.characterController.runInput ? this.characterController.walkSpeed : this.characterController.runSpeed;

        float targetVelocityX = this.characterController.inputControls.positiveInputX < 0.2f ? 0 : this.characterController.inputControls.lookDirection * this.characterController.moveSpeed;

        this.characterController.velocity.x = Mathf.SmoothDamp(this.characterController.velocity.x, targetVelocityX, ref this.characterController.velocityXSmoothing, (this.characterController.controller.collisions.below) ? this.characterController.accelerationTimeGrounded : this.characterController.accelerationTimeAirborne);
        return NodeStates.SUCCESS;
    }

    private void Colliding()
    {
        if (!this.characterController.controller.collisions.below && !this.characterController.dashing)
        {
            this.collided = false;
            return;
        }

        Vector2 pos1 = new Vector2(this.characterController.transform.position.x + this.characterController.rayLength * this.characterController.inputControls.lookDirection, this.characterController.transform.position.y + this.characterController.controller.collider.size.y / 2);
        Vector2 pos2 = new Vector2(this.characterController.transform.position.x, this.characterController.transform.position.y + this.characterController.controller.collider.size.y / 2);
        RaycastHit2D[] rayHit1 = Physics2D.RaycastAll(pos1, Vector2.right * this.characterController.inputControls.lookDirection, this.characterController.velocity.x * Time.deltaTime, this.characterController.playerMask);

        //Player goes right
        if (this.characterController.inputControls.inputX > 0.1f)
        {
            foreach (RaycastHit2D item in rayHit1)
            {
                CharacterController cc = item.collider.gameObject.GetComponent<CharacterController>();

                if (item.collider.gameObject == this.characterController.gameObject || cc == null)
                {
                    continue;
                }

                //Enemy goes left
                if (cc.inputControls.inputX < -0.1f && this.characterController.goTroughTimer > 0 && !cc.dashing)
                {
                    this.characterController.velocity.x = 0;
                    this.characterController.goTroughTimer -= Time.deltaTime;
                    this.collided = true;
                    return;
                }
            }
            this.collided = false;
            return;
        }
        
        //Player goes left
        if (this.characterController.inputControls.inputX < -0.1f)
        {
            foreach (RaycastHit2D item in rayHit1)
            {
                CharacterController cc = item.collider.gameObject.GetComponent<CharacterController>();

                if (item.collider.gameObject == this.characterController.gameObject || cc == null)
                {
                    continue;
                }

                //Enemy goes right
                if (cc.inputControls.inputX > 0.1f && this.characterController.goTroughTimer > 0 && !cc.dashing)
                {
                    this.characterController.velocity.x = 0;
                    this.characterController.goTroughTimer -= Time.deltaTime;
                    this.collided = true;
                    return;
                }
            }
            this.collided = false;
            return;
        }

        //Player idles
        Collider2D[] colliders = Physics2D.OverlapCircleAll(pos2, this.characterController.circleRadius, this.characterController.playerMask);

        foreach (Collider2D item in colliders)
        {
            CharacterController cc = item.gameObject.GetComponent<CharacterController>();

            if (item.gameObject == this.characterController.gameObject || cc == null)
            {
                continue;
            }

            if (cc.dashing)
            {
                this.collided = false;
                return;
            }

            if (Mathf.Abs(cc.inputControls.inputX) > 0.2f || !cc.controller.collisions.below)
            {
                return;
            }

            this.characterController.velocity.x += this.characterController.pushSpeed * (this.characterController.transform.position.x < item.transform.position.x ? -1 : 1) *Time.deltaTime;
            this.collided = true;
            return;
        }

        //Check in front
        foreach (RaycastHit2D item in rayHit1)
        {
            CharacterController cc = item.collider.gameObject.GetComponent<CharacterController>();

            if (item.collider.gameObject == this.characterController.gameObject || cc == null)
            {
                continue;
            }

            if (cc.dashing)
            {              
                return;
            }

            //Move right
            if (this.characterController.transform.position.x > cc.transform.position.x)
            {
                //Enemy is moving
                if (cc.inputControls.inputX > 0.1f)
                {
                    this.characterController.velocity.x = Mathf.Abs(cc.velocity.x);
                    this.collided = true;
                    return;
                }
            }

            //Move left
            if (this.characterController.transform.position.x < cc.transform.position.x)
            {
                //Enemy is moving
                if (cc.inputControls.inputX < -0.1f)
                {
                    this.characterController.velocity.x = -Mathf.Abs(cc.velocity.x);
                    this.collided = true;
                    return;
                }
            }
        }

        Vector2 pos3 = new Vector2(this.characterController.transform.position.x - this.characterController.rayLength * this.characterController.inputControls.lookDirection, this.characterController.transform.position.y + this.characterController.controller.collider.size.y / 2);
        RaycastHit2D[] rayHit2 = Physics2D.RaycastAll(pos3, Vector2.right * -this.characterController.inputControls.lookDirection, this.characterController.velocity.x * Time.deltaTime, this.characterController.playerMask);

        //Check behind
        foreach (RaycastHit2D item in rayHit2)
        {
            CharacterController cc = item.collider.gameObject.GetComponent<CharacterController>();

            if (item.collider.gameObject == this.characterController.gameObject || cc == null)
            {
                continue;
            }

            if (cc.dashing)
            {
                return;
            }

            //Move right
            if (this.characterController.transform.position.x > cc.transform.position.x)
            {
                //Enemy is moving
                if (cc.inputControls.inputX > 0.1f)
                {
                    this.characterController.velocity.x = Mathf.Abs(cc.velocity.x);
                    this.collided = true;
                    return;
                }
            }

            //Move left
            if (this.characterController.transform.position.x < cc.transform.position.x)
            {
                //Enemy is moving
                if (cc.inputControls.inputX < -0.1f)
                {
                    this.characterController.velocity.x = -Mathf.Abs(cc.velocity.x);
                    this.collided = true;
                    return;
                }
            }
        }


        this.collided = false;
        this.characterController.goTroughTimer = this.characterController.goTroughTime;
    }
}
