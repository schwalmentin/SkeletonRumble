using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class SimpleEnemy : CharacterController
{
    #region Nodes

    private Move move_Node;
    private JumpDown jumpDown_Node;
    private JumpUp jumpUp_Node;

    private WallSliding wallSlide_Node;
    private WallrunDown wallrunDown_Node;
    private WallrunUp wallrunUp_Node;

    private OnDash onDash_Node;
    private Dash dash_Node;

    private OnMeleeAttack onMeleeAttack_Node;
    private MeleeAttack meleeAttack_Node;

    private OnParry onParry_Node;
    private Parry parry_Node;

    private CalculateGravity gravity_Node;
    private ResetVelocity resetVelocity_Node;

    private UpdateAnimations updateAnimations_Node;
    private UpdateCharacterGraphic updateCharacterGraphic_Node;

    #endregion

    public int startLookingDirection;
    public float timeToAttack;
    public float timeToAttackTimer;
    public bool canAttack;

    [SerializeField] private PlayerStates playerStates;

    public override void Start()
    {
        base.Start();

        if (this.startLookingDirection != 0)
        {
            this.inputControls.lookDirection = this.startLookingDirection;
        }

        this.timeToAttackTimer = this.timeToAttack;

        //Set PlayerStates
        this.playerStates = PlayerStates.Move;

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
        this.jumpUp_Node = new JumpUp
        {
            characterController = this
        };

        //Inizialize Wallsliding
        this.wallSlide_Node = new WallSliding
        {
            characterController = this
        };

        //Inizialize Wallrun
        this.wallrunDown_Node = new WallrunDown
        {
            characterController = this
        };
        this.wallrunUp_Node = new WallrunUp
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

        //Inizialize Update Animations
        this.updateAnimations_Node = new UpdateAnimations
        {
            characterController = this
        };

        //Inizialize Update Character Graphic
        this.updateCharacterGraphic_Node = new UpdateCharacterGraphic
        {
            characterController = this
        };

        //Inizialize Melee Attack
        this.onMeleeAttack_Node = new OnMeleeAttack
        {
            characterController = this
        };
        this.meleeAttack_Node = new MeleeAttack
        {
            characterController = this
        };

        //Inizialize Parry
        this.onParry_Node = new OnParry
        {
            characterController = this
        };
        this.parry_Node = new Parry
        {
            characterController = this
        };

        //Inizialize Dash
        this.onDash_Node = new OnDash
        {
            characterController = this
        };
        this.dash_Node = new Dash
        {
            characterController = this
        };

        this.weaponState = WeaponState.Idle;
        this.parryState = WeaponState.Idle;
    }

    private void Update()
    {
        switch (this.playerStates)
        {
            case PlayerStates.Empty:
                break;

            case PlayerStates.Move:

                if (this.canAttack)
                {
                    this.timeToAttackTimer -= Time.deltaTime;

                    if (this.timeToAttackTimer <= 0)
                    {
                        this.timeToAttackTimer = this.timeToAttack;
                        this.OnMeleeAttack();
                        return;
                    }
                }

                this.RotateObj();

                if (this.parryCooldown > 0)
                {
                    this.parryCooldown -= Time.deltaTime;
                }

                if (this.dashCooldown > 0)
                {
                    this.dashCooldown -= Time.deltaTime;
                }

                this.updateCharacterGraphic_Node.Evaluate();
                this.move_Node.Evaluate();
                this.wallSlide_Node.Evaluate();
                this.gravity_Node.Evaluate();
                this.updateAnimations_Node.Evaluate();

                if (this.canWallrun)
                {
                    this.wallrunDown_Node.Evaluate();
                }

                this.controller.Move(this.velocity * Time.deltaTime, new Vector2(this.inputControls.inputX, this.inputControls.inputY));
                this.resetVelocity_Node.Evaluate();

                if (this.controller.collisions.below)
                {
                    this.wallrunTime = this.wallrunTimeGround;
                    this.wallrunVelocity = this.wallrunVelocityGround;

                    this.jumpCounter = 0;
                }

                if (this.controller.collisions.right || this.controller.collisions.left)
                {
                    this.jumpCounter = 1;
                }

                break;

            case PlayerStates.Dash:

                if (this.dash_Node.Evaluate() == NodeStates.SUCCESS)
                {
                    this.playerStates = PlayerStates.Move;
                }

                this.gravity_Node.Evaluate();
                this.controller.Move(this.velocity * Time.deltaTime, false);
                this.resetVelocity_Node.Evaluate();

                break;

            case PlayerStates.Attack:

                if (this.meleeAttack_Node.Evaluate() == NodeStates.SUCCESS)
                {
                    this.playerStates = PlayerStates.Move;
                }

                break;

            case PlayerStates.Parry:

                if (this.parry_Node.Evaluate() == NodeStates.SUCCESS)
                {
                    if (this.canParryCombo)
                    {
                        //Switch to attack + combo
                        this.playerStates = PlayerStates.Attack;
                        break;
                    }
                    this.playerStates = PlayerStates.Move;
                }

                break;

            case PlayerStates.Stunned:

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
                    this.playerStates = PlayerStates.Move;
                }

                this.controller.Move(this.velocity * Time.deltaTime, false);
                break;

            default:
                break;
        }
    }

    public void UpdateInput(InputAction.CallbackContext context)
    {
        //LastInput
        this.inputControls.lastInputX = this.inputControls.inputX;
        this.inputControls.lastInputY = this.inputControls.inputY;

        //Input
        this.inputControls.inputX = context.ReadValue<Vector2>().x;
        this.inputControls.inputY = context.ReadValue<Vector2>().y;

        //Positive Input
        this.inputControls.positiveInputX = Mathf.Abs(this.inputControls.inputX);
        this.inputControls.positiveInputY = Mathf.Abs(this.inputControls.inputY);

        //Last Look Direction
        this.inputControls.lastLookDirection = this.inputControls.lookDirection;

        //Look Direction
        if (Mathf.Abs(this.inputControls.inputX) > 0.2f)
        {
            this.inputControls.lookDirection = Mathf.RoundToInt(Mathf.Sign(this.inputControls.inputX));
        }
    }

    public void OnJumpDown(InputAction.CallbackContext context)
    {
        if (this.playerStates != PlayerStates.Move || !context.started)
        {
            return;
        }

        this.jumpDown_Node.Evaluate();
    }

    public void OnJumpUp(InputAction.CallbackContext context)
    {
        if (this.playerStates != PlayerStates.Move || !context.canceled)
        {
            return;
        }

        this.jumpUp_Node.Evaluate();
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (this.playerStates != PlayerStates.Move || !context.started)
        {
            return;
        }

        if (this.onDash_Node.Evaluate() == NodeStates.SUCCESS)
        {
            this.playerStates = PlayerStates.Dash;
        }
    }

    public void OnWallrunDown(InputAction.CallbackContext context)
    {
        if (!context.started)
        {
            return;
        }

        this.canWallrun = true;
    }

    public void OnWallrunUp(InputAction.CallbackContext context)
    {
        if (!context.canceled)
        {
            return;
        }

        this.canWallrun = false;
        this.wallrunUp_Node.Evaluate();
    }

    public void OnMeleeAttack()
    {
        if ((this.playerStates != PlayerStates.Move && this.playerStates != PlayerStates.Attack && this.playerStates != PlayerStates.Parry))
        {
            return;
        }

        this.onMeleeAttack_Node.Evaluate();
        if (this.parryState == WeaponState.Idle)
        {
            this.playerStates = PlayerStates.Attack;
        }
    }

    public void OnParry(InputAction.CallbackContext context)
    {
        if (this.playerStates != PlayerStates.Move || !context.performed)
        {
            return;
        }

        if (this.onParry_Node.Evaluate() == NodeStates.SUCCESS)
        {
            this.playerStates = PlayerStates.Parry;
            this.velocity = Vector2.zero;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(this.attackHolder.position, DegreeToVector2(this.attackAngleUp));
        Gizmos.DrawRay(this.attackHolder.position, DegreeToVector2(this.attackAngleDown));

        if (this.showAnimationGizmo)
        {
            Gizmos.color = Color.yellow;

            foreach (Vector2 item in this.footSpacing)
            {
                Vector2 rightFootPos = new Vector2(this.transform.position.x - item.x, this.transform.position.y + this.yOffset);
                Gizmos.DrawSphere(rightFootPos, 0.05f);
                Gizmos.DrawRay(rightFootPos, Vector2.down * item.y);

                Vector2 leftFootPos = new Vector2(this.transform.position.x + item.x, this.transform.position.y + this.yOffset);
                Gizmos.DrawSphere(leftFootPos, 0.05f);
                Gizmos.DrawRay(leftFootPos, Vector2.down * item.y);
            }
        }

        if (this.showParryGizmo)
        {
            Gizmos.color = this.parryDuration > 0 ? new Color(0, 0, 255, 0.5f) : new Color(0, 0, 255, 0.2f);

            Gizmos.DrawSphere(this.parryTransform.position, this.currentMeleeWeapon.parryRadius);
        }

        if (this.showCombatGizmo)
        {
            Gizmos.color = this.weaponState == WeaponState.Strike ? new Color(255, 0, 0, 0.5f) : new Color(255, 0, 0, 0.2f);

            Matrix4x4 rotationMatrix = Matrix4x4.TRS(this.attackPositions[0].position, this.attackPositions[0].rotation, this.currentMeleeWeapon.attackUp.size);
            Gizmos.matrix = rotationMatrix;
            Gizmos.DrawCube(Vector3.zero, this.currentMeleeWeapon.attackUp.size);

            rotationMatrix = Matrix4x4.TRS(this.attackPositions[1].position, this.attackPositions[1].rotation, this.currentMeleeWeapon.attackFront.size);
            Gizmos.matrix = rotationMatrix;
            Gizmos.DrawCube(Vector3.zero, this.currentMeleeWeapon.attackFront.size);

            rotationMatrix = Matrix4x4.TRS(this.attackPositions[2].position, this.attackPositions[2].rotation, this.currentMeleeWeapon.attackDown.size);
            Gizmos.matrix = rotationMatrix;
            Gizmos.DrawCube(Vector3.zero, this.currentMeleeWeapon.attackDown.size);
        }

        //Warning: Matrix
    }

    private enum PlayerStates
    {
        Empty,
        Move,
        Dash,
        Attack,
        Parry,
        Stunned,
        Heal
    }

    public static Vector2 RadianToVector2(float radian)
    {
        return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
    }

    public static Vector2 DegreeToVector2(float degree)
    {
        return RadianToVector2(degree * Mathf.Deg2Rad);
    }

    private void RotateObj()
    {
        this.attackHolder.eulerAngles = new Vector3(this.attackHolder.eulerAngles.x, this.attackHolder.eulerAngles.y, (Mathf.Atan2(this.inputControls.inputX, this.inputControls.inputY) * 180 / Mathf.PI) + 90);

        //if (this.attackHolder.eulerAngles.z > this.attackAngleUp)
        //{
        //    this.attackHolder .eulerAngles = new Vector3(this.attackHolder.eulerAngles.x, this.attackHolder.eulerAngles.y, this.attackAngleUp);
        //}

        if (this.attackHolder.eulerAngles.z > Mathf.Abs(this.attackAngleDown) && this.attackHolder.eulerAngles.z < 90)
        {
            this.attackHolder.eulerAngles = new Vector3(this.attackHolder.eulerAngles.x, this.attackHolder.eulerAngles.y, this.attackAngleDown);
        }
    }

    public override void GetStunned(float stunTime)
    {
        base.GetStunned(stunTime);
        this.playerStates = PlayerStates.Stunned;
        this.stunTimer = this.stunTime;
        this.timeToAttackTimer = this.timeToAttack;
        this.animator.SetTrigger("ResetAttack");
        this.meleeAttack_Node.ResetAttack();
    }

    //public override void GetDamage(float damage, Vector2 knockBack, float stunTime, bool weightless)
    //{
    //    this.currentHealth -= damage;
    //    this.healthSlider.value = this.currentHealth / this.maxHealth;
    //    this.GetStunned(stunTime);
    //    this.velocity = knockBack;
    //    this.weightless = weightless;

    //    if (this.currentHealth <= 0)
    //    {
    //        this.Die();
    //    }
    //}
    public float stunTime;

    public override void Die()
    {
        this.currentHealth = this.maxHealth;
        this.healthSlider.fillAmount = this.currentHealth / this.maxHealth;
    }
}
