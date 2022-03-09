using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trainingsdummy : CharacterController
{
    #region Nodes

    private Move move_Node;
    private JumpDown jumpDown_Node;

    private CalculateGravity gravity_Node;
    private ResetVelocity resetVelocity_Node;

    #endregion

    [Header("Dummy")]
    public Transform checkPositionTransform;
    public Vector2 checkPositionSize;
    private Vector2 startPosition;
    public LayerMask dummyMask;
    private Animator dummyAnimator;
    private PlayerCharacter playerCharacter;
    public float doubleJumpTime;
    private float doubleJumpTimer;
    private bool doubleJump;

    public override void Start()
    {
        base.Start();

        this.startPosition = this.transform.position;
        this.dummyAnimator = this.GetComponentInChildren<Animator>();
        this.playerCharacter = FindObjectOfType<PlayerCharacter>();

        //Inizialize Move
        this.move_Node = new Move
        {
            characterController = this
        };

        //Inizialize Jump
        this.jumpDown_Node = new JumpDown
        {
            characterController = this
        };

        //Inizialize Gravity
        this.gravity_Node = new CalculateGravity
        {
            characterController = this
        };

        //Inizialize Reset Velocity
        this.resetVelocity_Node = new ResetVelocity
        {
            characterController = this
        };
    }

    private void Update()
    {
        if (this.enemyStunned)
        {
            this.stunGraphic.SetActive(true);
            this.stunTimer -= Time.deltaTime;

            if (this.weightless)
            {
                this.velocity.y = Mathf.SmoothDamp(this.velocity.y, 0, ref this.smoothingY, this.stunAccelerationY);
            }
            else
            {
                this.velocity.y -= this.stunGravity * Time.deltaTime;
            }

            this.velocity.x = Mathf.SmoothDamp(this.velocity.x, 0, ref this.velocityXSmoothing, this.stunAccelerationX);

            if (this.stunTimer <= 0)
            {
                this.stunGraphic.SetActive(false);
                this.enemyStunned = false;
            }

            this.controller.Move(this.velocity * Time.deltaTime, false);

            return;
        }

        if (this.controller.collisions.below)
        {
            this.jumpCounter = 0;
        }


        this.GoTorwardsStartPos();

        this.move_Node.Evaluate();
        this.gravity_Node.Evaluate();

        this.UpdateGraphics();

        this.controller.Move(this.velocity * Time.deltaTime, new Vector2(this.inputControls.inputX, this.inputControls.inputY));
        this.resetVelocity_Node.Evaluate();
    }

    private void GoTorwardsStartPos()
    {
        //Check if not able to move
        if (!this.controller.collisions.below && Physics2D.OverlapBox(this.checkPositionTransform.position, this.checkPositionSize, 0, this.dummyMask) != null || Vector2.Distance(this.transform.position, this.startPosition) < 0.5f || Physics2D.OverlapCircle(this.transform.position, 0.5f, this.playerMask) != null)
        {
            this.inputControls.inputX = 0;
            this.inputControls.positiveInputX = 0;
            this.dummyAnimator.SetBool("Move", false);
            return;
        }

        //Pathfinding
        //Check for doublejump
        if (Physics2D.OverlapBox(this.checkPositionTransform.position, this.checkPositionSize, 0, this.dummyMask) == null && this.controller.collisions.below)
        {
            this.jumpDown_Node.Evaluate();
            this.doubleJumpTimer = this.doubleJumpTime;
            this.dummyAnimator.SetTrigger("Jump");
            this.doubleJump = true;
        }

        //Double Jump
        if (this.doubleJump)
        {
            this.doubleJumpTimer -= Time.deltaTime;

            if (this.doubleJumpTimer <= 0)
            {
                this.jumpDown_Node.Evaluate();
                this.dummyAnimator.SetTrigger("Jump");
                this.doubleJump = false;
            }
        }

        //Move
        this.inputControls.inputX = this.startPosition.x - this.transform.position.x < 0 ? -1 : 1;
        this.inputControls.lookDirection = (int)this.inputControls.inputX;
        this.inputControls.positiveInputX = 1;
        this.dummyAnimator.SetBool("Move", true);
    }

    private void UpdateGraphics()
    {
        //Update Animations
        this.dummyAnimator.SetBool("Grounded", this.controller.collisions.below);

        bool fall = this.velocity.y < 0 && this.controller.collisions.below;
        this.dummyAnimator.SetBool("Fall", fall);

        //Update Graphic
        if (this.inputControls.positiveInputX < 0.2f)
        {
            this.characterGraphic.transform.localScale = new Vector3(Mathf.Abs(this.characterGraphic.transform.localScale.x) * -Mathf.Sign(this.playerCharacter.transform.position.x - this.transform.position.x), this.characterGraphic.transform.localScale.y, this.characterGraphic.transform.localScale.z);
        }
        else
        {
            this.characterGraphic.transform.localScale = new Vector3(Mathf.Abs(this.characterGraphic.transform.localScale.x) * -this.inputControls.lookDirection, this.characterGraphic.transform.localScale.y, this.characterGraphic.transform.localScale.z);
        }
    }

    private void OnDrawGizmos()
    {
        if (this.checkPositionTransform != null)
        {
            Gizmos.DrawWireCube(this.checkPositionTransform.position, this.checkPositionSize);
        }
    }

    private bool enemyStunned;

    public override void GetDamage(float damage, Vector2 knockBack, float stunTime, bool weightless, MeleeAttack meleeAttack)
    {
        this.GetStunned(stunTime);
        this.velocity = knockBack;
        this.dummyAnimator.SetTrigger("Hit");
        this.audioManager.PlayDamageSound(this.audioID, this.playerCharacter.currentMeleeWeapon.weaponIndex == 0);
        this.weightless = weightless;
    }

    public override void GetStunned(float stunTime)
    {
        base.GetStunned(stunTime);
        this.enemyStunned = true;
        this.stunTimer = stunTime;
    }
}
