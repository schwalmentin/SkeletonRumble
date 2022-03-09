using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    //Generel
    [Header("Generel")]
    public bool disableGizmos;
    public LayerMask platformMask;

    private void Start()
    {
        this.enemycharacter = this.GetComponent<EnemyController>();
    }

    #region Pathfinding

    //Can Jump
    [Header("Can Jump")]
    public float checkDoubleJumpRadius;
    public Vector2 checkJumpLength;
    public Vector2 checkDoubleJumpLength;

    //Can Dash
    [Header("Can Dash")]
    public float checkDashRadius;
    public float platformSegments;

    public bool CanDoubleJump(Controller2D controller, CharacterController.InputControls inputControls, Vector2 targetPos)
    {
        Collider2D[] collider = Physics2D.OverlapCircleAll(this.transform.position, this.checkDoubleJumpRadius, this.platformMask);

        foreach (Collider2D item in collider)
        {
            if (inputControls.lookDirection <= 0 && item.bounds.center.x - this.transform.position.x > 0 ||
                inputControls.lookDirection >= 0 && item.bounds.center.x - this.transform.position.x < 0)
            {
                continue;
            }

            Vector2 startPoint = new Vector2(inputControls.lookDirection >= 0 ? item.bounds.min.x : item.bounds.max.x, item.bounds.max.y);

            if (targetPos.y - startPoint.y < 0)
            {
                continue;
            }

            Color green = Color.green;
            Color red = Color.red;
            Debug.DrawRay(startPoint, Vector2.down * this.checkDoubleJumpLength.y, Mathf.Abs(startPoint.y - this.transform.position.y) < this.checkDoubleJumpLength.y ? green : red);
            Debug.DrawRay(new Vector2(startPoint.x, startPoint.y - this.checkDoubleJumpLength.y), (inputControls.lookDirection >= 0 ? -Vector2.right : Vector2.right) * this.checkDoubleJumpLength.x, Mathf.Abs(startPoint.x - this.transform.position.x) < this.checkDoubleJumpLength.x ? green : red);
            if (Mathf.Abs(startPoint.x - this.transform.position.x) < this.checkDoubleJumpLength.x && startPoint.y - this.transform.position.y < this.checkDoubleJumpLength.y && startPoint.y - this.transform.position.y > 0.1f)
            {
                return true;
            }
        }

        return false;
    }

    public bool CanDash(ref Vector2 outputDirection, CharacterController.InputControls inputControls, Vector2 startPos)
    {
        Collider2D[] collider = Physics2D.OverlapCircleAll(startPos, this.checkDashRadius, this.platformMask);

        foreach (Collider2D item in collider)
        {
            if (inputControls.lookDirection <= 0 && item.bounds.center.x - startPos.x > 0 || inputControls.lookDirection >= 0 && item.bounds.center.x - startPos.x < 0)
            {
                continue;
            }

            //Check if jump is not possible
            float platformHeight = item.bounds.max.y;

            if (platformHeight - this.transform.position.y < this.checkDoubleJumpLength.y)
            {
                continue;
            }

            Debug.DrawRay(item.bounds.center, Vector2.up * 100, Color.blue);

            //Check Dash Direction
            float distance = Mathf.Abs(item.bounds.max.x - item.bounds.min.x);
            distance /= this.platformSegments - 1;
            float rayLength = 1234;
            float posX = 0;

            for (int i = 0; i < this.platformSegments; i++)
            {
                Vector2 pos = new Vector2(item.bounds.max.x - distance * i, platformHeight);
                float newLength = Vector2.Distance(startPos, pos);

                if (newLength < rayLength)
                {
                    rayLength = newLength;
                    posX = pos.x;
                }
            }

            Vector2 finalDirection = (new Vector2(posX, platformHeight) - new Vector2(startPos.x, startPos.y)).normalized;
            RaycastHit2D hit = Physics2D.Raycast(startPos, finalDirection, this.checkDashRadius, this.platformMask);

            if (hit)
            {
                Collider2D hit2 = Physics2D.OverlapCircle(new Vector2(startPos.x, startPos.y) + finalDirection * (this.checkDashRadius + 0.1f), 0.1f);

                if (hit2 == null)
                {
                    outputDirection = finalDirection.normalized;
                    return true;
                }
            }
        }
        return false;
    }

    public bool CanJumpDash(ref Vector2 outputDirection, CharacterController.InputControls inputControls, int lookDirection, Vector3 targetPos, bool canJump)
    {
        if (!canJump)
        {
            return false;
        }

        Collider2D[] collider = Physics2D.OverlapCircleAll(this.transform.position, this.checkDoubleJumpRadius, this.platformMask);

        foreach (Collider2D item in collider)
        {
            if (inputControls.lookDirection <= 0 && item.bounds.center.x - this.transform.position.x > 0 ||
                inputControls.lookDirection >= 0 && item.bounds.center.x - this.transform.position.x < 0)
            {
                continue;
            }

            Vector2 startPos = new Vector2(this.transform.position.x + this.checkJumpLength.x * lookDirection, this.transform.position.y + this.checkJumpLength.y);
            Debug.DrawRay(startPos, Vector2.up * 10, Color.yellow);

            if (targetPos.y - item.bounds.max.y < 0)
            {
                continue;
            }

            this.startPosGizmo = startPos;
            if (this.CanDash(ref outputDirection, inputControls, startPos))
            {
                return true;
            }
        }

        return false;
    }

    private Vector2 startPosGizmo;

    #endregion

    #region Attack State

    [Header("Attack States Sword")]
    public bool swordGizmo;
    public LayerMask playerMask;
    public Transform attackFrontPosition_S;
    public Vector2 attackFrontSize_S;
    public Transform attackUpPosition_S;
    public Vector2 attackUpSize_S;
    public Transform attackDownPosition_S;
    public Vector2 attackDownSize_S;
    public Transform attackHeavyDownPosition_S;
    public Vector2 attackHeavyDownSize_S;
    public float dodgeRollRange_S;
    public float parryRange_S;
    public float doubleJumpRange_S;

    [Header("Attack States Hammer")]
    public bool hammerGizmo;
    public Transform attackFrontPosition_H;
    public Vector2 attackFrontSize_H;
    public Transform attackUpPosition_H;
    public Vector2 attackUpSize_H;
    public Transform attackDownPosition_H;
    public Vector2 attackDownSize_H;
    public Transform attackHeavyDownPosition_H;
    public Vector2 attackHeavyDownSize_H;
    public float dodgeRollRange_H;
    public float parryRange_H;
    public float doubleJumpRange_H;

    private EnemyController enemycharacter;

    //Atacks
    public bool CanAttackFront()
    {
        Collider2D collider = Physics2D.OverlapBox(this.attackFrontPosition_S.position, this.attackFrontSize_S, 0, this.playerMask);

        //print("CanAttackFront: " + (collider != null ? "true" : "false"));
        return collider != null;
    }

    public bool CanAttackUp()
    {       
        Collider2D collider = Physics2D.OverlapBox(this.attackUpPosition_S.position, this.attackUpSize_S, 0, this.playerMask);

        //print("CanAttackUp: " + (collider != null ? "true" : "false"));
        return collider != null;
    }

    public bool CanAttackDown()
    {
        Collider2D collider = Physics2D.OverlapBox(this.attackDownPosition_S.position, this.attackDownSize_S, 0, this.playerMask);

        //print("CanAttackDown: " + (collider != null ? "true" : "false"));
        return collider != null;
    }

    public bool CanAttackHeavyDown()
    {
        Collider2D collider = Physics2D.OverlapBox(this.attackHeavyDownPosition_S.position, this.attackHeavyDownSize_S, 0, this.playerMask);

        //print("CanAttackHeavyDown: " + (collider != null ? "true" : "false"));
        return collider != null && this.enemycharacter.controller.collisions.below;
    }

    //Evades
    public bool CanDodgeRoll()
    {
        bool canDodgeRoll = this.enemycharacter.playerCharacter.attackTimer > 0 && Physics2D.OverlapCircle(this.transform.position, this.dodgeRollRange_S, this.playerMask);

        //print("CanDodgeRoll: " + (canDodgeRoll ? "true" : "false"));
        return canDodgeRoll;
    }

    public bool CanParry()
    {
        Vector2 direction = this.enemycharacter.playerTransform.position - this.transform.position;

        //bool canParry = Physics2D.OverlapCircle(this.transform.position, this.parryRange_S, this.playerMask) && this.enemycharacter.playerCharacter.currentMeleeWeapon;

        if (Physics2D.OverlapCircle(this.transform.position, this.parryRange_S, this.playerMask))
        {
            if (this.enemycharacter.playerCharacter.weaponState == WeaponState.Anticipation)
            {
                print("CANPARRY---------------");
                return true;
            }

            if(this.enemycharacter.playerCharacter.velocity.x < 0 && this.transform.position.x - this.enemycharacter.playerCharacter.transform.position.x < 0)
            {
                return true;
            }

            if (this.enemycharacter.playerCharacter.velocity.x > 0 && this.transform.position.x - this.enemycharacter.playerCharacter.transform.position.x > 0)
            {
                return true;
            }

            //if (this.enemycharacter.CalculatePercentage(0.5f) && direction.x < 0 && this.enemycharacter.playerCharacter.velocity.x > 0 ||
            //direction.x > 0 && this.enemycharacter.playerCharacter.velocity.x < 0)
            //{
            //    return true;
            //}
        }
        else
        {
            return false;
        }

        //print("CanParry: " + (canParry ? "true" : "false"));
        return false;
    }

    //Movement
    public bool CanDashTorwardsPlayer()
    {
        //print("CanDashTorwardsPlayer: true");
        return true;
    }

    public bool CanDashAwayFromPlayer()
    {
        //print("CanDashAwayFromPlayer: true");
        return true;
    }

    public bool CanAttackDoublejump()
    {
        bool canDoublejump = this.enemycharacter.controller.collisions.below && Physics2D.OverlapCircle(this.transform.position, this.doubleJumpRange_S, this.playerMask);

        //print("CanDoublejump: " + (canDoublejump ? "true" : "false"));
        return canDoublejump;
    }

    public bool CanJump()
    {
        //print("CanJump: " + (enemycharacter.jumpCounter < enemycharacter.maxJumpCounter ? "true" : "false"));
        return this.enemycharacter.jumpCounter < this.enemycharacter.maxJumpCounter;
    }

    public bool CanGoThroughPlatform()
    {
        bool canGoThroughPlatform = this.enemycharacter.controller.collisions.below && Physics2D.Raycast(this.transform.position, Vector2.down, 0.2f, this.platformMask) && this.enemycharacter.playerTransform.position.y - this.transform.position.y < 0;

        //print("CanGoThroughPlatform: " + (canGoThroughPlatform ? "true" : "false"));
        return canGoThroughPlatform;
    }

    #endregion

    private void OnDrawGizmos()
    {
        if (this.swordGizmo)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(this.attackFrontPosition_S.position, this.attackFrontSize_S);
            Gizmos.DrawWireCube(this.attackUpPosition_S.position, this.attackUpSize_S);
            Gizmos.DrawWireCube(this.attackDownPosition_S.position, this.attackDownSize_S);
            Gizmos.DrawWireCube(this.attackHeavyDownPosition_S.position, this.attackHeavyDownSize_S);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(this.transform.position, this.dodgeRollRange_S);
            Gizmos.DrawWireSphere(this.transform.position, this.parryRange_S);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(this.transform.position, this.doubleJumpRange_S);
        }

        if (this.hammerGizmo)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(this.attackFrontPosition_H.position, this.attackFrontSize_H);
            Gizmos.DrawWireCube(this.attackUpPosition_H.position, this.attackUpSize_H);
            Gizmos.DrawWireCube(this.attackDownPosition_H.position, this.attackDownSize_H);
            Gizmos.DrawWireCube(this.attackHeavyDownPosition_H.position, this.attackHeavyDownSize_H);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(this.transform.position, this.dodgeRollRange_H);
            Gizmos.DrawWireSphere(this.transform.position, this.parryRange_H);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(this.transform.position, this.doubleJumpRange_H);
        }


        if (this.disableGizmos)
        {
            return;
        }
        Gizmos.DrawWireSphere(this.transform.position, this.checkDashRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.startPosGizmo, this.checkDashRadius);
    }
}
