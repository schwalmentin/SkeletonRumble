using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateCharacterGraphic : Node
{
    public CharacterController characterController;

    public override NodeStates Evaluate()
    {
        //Set Rotation and Position
        if (this.characterController.controller.collisions.below && (this.characterController.inputControls.positiveInputX > 0.2f || this.characterController.dashTime > 0) && !this.characterController.controller.collisions.right && !this.characterController.controller.collisions.left && !this.characterController.controller.collisions.slidingDownMaxSlope)
        {
            RaycastHit2D hit = Physics2D.Raycast(this.characterController.transform.position, Vector2.down, 2, this.characterController.controller.collisionMask);

            if (hit)
            {
                Vector2 vectorToTarget1 = hit.normal;
                float angle1 = Mathf.Atan2(vectorToTarget1.y, vectorToTarget1.x) * Mathf.Rad2Deg;
                Quaternion q1 = Quaternion.AngleAxis(angle1 - 90, Vector3.forward);
                this.characterController.characterGraphic.transform.rotation = Quaternion.Slerp(this.characterController.characterGraphic.transform.rotation, q1, Time.deltaTime * 5);

                this.characterController.characterGraphic.transform.position = Vector2.MoveTowards(this.characterController.characterGraphic.transform.position, hit.point, this.characterController.graphicAdjustSpeed * Time.deltaTime);
            }
        }
        else
        {
            Vector2 vectorToTarget1 = Vector2.up;
            float angle1 = Mathf.Atan2(vectorToTarget1.y, vectorToTarget1.x) * Mathf.Rad2Deg;
            Quaternion q1 = Quaternion.AngleAxis(angle1 - 90, Vector3.forward);
            this.characterController.characterGraphic.transform.rotation = Quaternion.Slerp(this.characterController.characterGraphic.transform.rotation, q1, Time.deltaTime * 10);
        }

        //Turn
        if (this.characterController.wallSliding)
        {
            this.characterController.characterGraphic.transform.localScale = new Vector3(Mathf.Abs(this.characterController.characterGraphic.transform.localScale.x) * this.characterController.wallDirX, this.characterController.characterGraphic.transform.localScale.y, this.characterController.characterGraphic.transform.localScale.z);
        }
        else
        {
            this.characterController.characterGraphic.transform.localScale = new Vector3(Mathf.Abs(this.characterController.characterGraphic.transform.localScale.x) * this.characterController.inputControls.lookDirection, this.characterController.characterGraphic.transform.localScale.y, this.characterController.characterGraphic.transform.localScale.z);
        }

        return NodeStates.SUCCESS;
    }
}
