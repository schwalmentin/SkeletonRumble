using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

[RequireComponent(typeof(EnemyBehavior))]
public class EnemyController : CharacterController
{
    public EnemyStates enemyStates;
    private EnemyBehavior behavior;
    private BehaviorManager behaviorManager;
    public PlayerCharacter playerCharacter;
    public int startLookDirection;
    public bool disableGizmo;

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
    [HideInInspector] public UpdateCharacterGraphic updateCharacterGraphic_Node;

    #endregion

    public override void Start()
    {
        base.Start();

        this.behavior = this.GetComponent<EnemyBehavior>();
        this.behaviorManager = this.GetComponent<BehaviorManager>();
        this.inputControls.lookDirection = this.startLookDirection <= 0 ? -1 : 1;
        this.playerCharacter = FindObjectOfType<PlayerCharacter>();
        this.recoverOfAttackTimer = this.recoverOfAttackTime;

        #region Inizialize Nodes

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

        #endregion

        this.enemyStates = EnemyStates.Duell;

        this.neutral = new float[]
        {
            this.neutralP.attackDown,
            this.neutralP.attackFront,
            this.neutralP.attackHeavyDown,
            this.neutralP.attackUp,
            this.neutralP.dashTorwardsPlayer,
            this.neutralP.dashAwayFromPlayer,
            this.neutralP.dodgeRoll,
            this.neutralP.doubleJump,
            this.neutralP.jump,
            this.neutralP.parry,
            this.neutralP.throughPlatform,
        };
        this.anger = new float[]
        {
            this.angerP.attackDown,
            this.angerP.attackFront,
            this.angerP.attackHeavyDown,
            this.angerP.attackUp,
            this.angerP.dashTorwardsPlayer,
            this.angerP.dashAwayFromPlayer,
            this.angerP.dodgeRoll,
            this.angerP.doubleJump,
            this.angerP.jump,
            this.angerP.parry,
            this.angerP.throughPlatform,
        };
        this.arrogance = new float[]
        {
            this.arroganceP.attackDown,
            this.arroganceP.attackFront,
            this.arroganceP.attackHeavyDown,
            this.arroganceP.attackUp,
            this.arroganceP.dashTorwardsPlayer,
            this.arroganceP.dashAwayFromPlayer,
            this.arroganceP.dodgeRoll,
            this.arroganceP.doubleJump,
            this.arroganceP.jump,
            this.arroganceP.parry,
            this.arroganceP.throughPlatform,
        };
        this.elegance = new float[]
        {
            this.eleganceP.attackDown,
            this.eleganceP.attackFront,
            this.eleganceP.attackHeavyDown,
            this.eleganceP.attackUp,
            this.eleganceP.dashTorwardsPlayer,
            this.eleganceP.dashAwayFromPlayer,
            this.eleganceP.dodgeRoll,
            this.eleganceP.doubleJump,
            this.eleganceP.jump,
            this.eleganceP.parry,
            this.eleganceP.throughPlatform,
        };
        this.fear = new float[]
        {
            this.fearP.attackDown,
            this.fearP.attackFront,
            this.fearP.attackHeavyDown,
            this.fearP.attackUp,
            this.fearP.dashTorwardsPlayer,
            this.fearP.dashAwayFromPlayer,
            this.fearP.dodgeRoll,
            this.fearP.doubleJump,
            this.fearP.jump,
            this.fearP.parry,
            this.fearP.throughPlatform,
        };

        this.canAttackFunctions = new Func<bool>[] 
        {
            this.behavior.CanAttackDown,
            this.behavior.CanAttackFront,
            this.behavior.CanAttackHeavyDown,
            this.behavior.CanAttackUp,
            this.behavior.CanDashTorwardsPlayer,
            this.behavior.CanDashAwayFromPlayer,
            this.behavior.CanDodgeRoll,
            this.behavior.CanAttackDoublejump,
            this.behavior.CanJump,
            this.behavior.CanParry,
            this.behavior.CanGoThroughPlatform
        };
        this.attackFunctions = new Func<bool>[]
        {
            this.OnAttackDown,
            this.OnAttackFront,
            this.OnAttackHeavyDown,
            this.OnAttackUp,
            this.OnDashTorwardsPlayer,
            this.OnDashAwayFromPlayer,
            this.OnDodgeRoll,
            this.OnDoublejump,
            this.OnJump,
            this.OnParry,
            this.OnGoThroughPlatform
        };

        EmotionManager.Anger = 0;
        EmotionManager.Arrogance = 0;
        EmotionManager.Elegance = 0;
        EmotionManager.Fear = 0;
    }

    public void UpdateEnemyController(EnemyStates state)
    {
        this.enemyStates = state;
    }

    private void Update()
    {
        this.hammerCollider.enabled = this.currentMeleeWeapon.weaponIndex != 0;
        this.swordCollider.enabled = this.currentMeleeWeapon.weaponIndex == 0;
        this.animator.SetBool("Grounded", this.controller.collisions.below);
        if (Mathf.Abs(this.inputControls.lookDirection) < 1)
        {
            this.inputControls.lookDirection = 1;
        }

        if (this.goTorwardsPlayer)
        {
            return;
        }

        if (!this.isAbleToMove)
        {
            this.updateAnimations_Node.Evaluate();
            this.updateCharacterGraphic_Node.Evaluate();
            this.gravity_Node.Evaluate();
            this.controller.Move(this.velocity * Time.deltaTime, false);
            this.resetVelocity_Node.Evaluate();
            return;
        }

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

        //Setup / Passive Behavior
        this.UpdateInput();


        this.updateAnimations_Node.Evaluate();

        if (this.controller.collisions.below)
        {
            this.wallrunTime = this.wallrunTimeGround;
            this.wallrunVelocity = this.wallrunVelocityGround;

            this.jumpCounter = 0;
            this.dashCounter = 0;
            this.canAttackUp = true;
        }

        if (this.controller.collisions.right || this.controller.collisions.left)
        {
            this.jumpCounter = 1;
            this.dashCounter = 0;
        }

        if (this.parryCooldown > 0)
        {
            this.parryCooldown -= Time.deltaTime;
        }

        if (this.dashCooldown > 0)
        {
            this.dashCooldown -= Time.deltaTime;
        }

        //Behavior
        switch (this.enemyStates)
        {
            case EnemyStates.Charge:
                this.Charge();
                this.updateCharacterGraphic_Node.Evaluate();
                break;

            case EnemyStates.Flee:
                this.Flee();
                this.updateCharacterGraphic_Node.Evaluate();
                break;

            case EnemyStates.Duell:
                this.Duell();
                this.updateCharacterGraphic_Node.Evaluate();
                break;

            case EnemyStates.Attack:
                this.Attack();
                break;
        }
    }

    private void LateUpdate()
    {
        if (this.inputControls.lastLookDirection != this.inputControls.lookDirection)
        {
            this.inputControls.lastLookDirection = this.inputControls.lookDirection;
        }
    }

    #region Charge Variables

    //Charge
    [Header("Charge")]
    public Transform playerTransform;
    private ChargeState chargeState;
    private Vector2 targetPosition;
    public float updateLookDirectionRange;
    public float chargeToAttackRange;

    //Target Position
    public float updateTargetPositionTimeMin;
    public float updateTargetPositionTimeMax;
    private float updateTargetPositionTimer;
    public float targetPositionRadius;
    public float minTargetPositionRadius;

    //Doublejump
    public float doubleJumpTimeMin;
    public float doubleJumpTimeMax;
    private float doubleJumpTimer;
    
    //Dash
    private bool dash;

    //Walljump
    public float wallJumpTime;
    private float wallJumpTimer;

    //Go trough platform
    public float goThroughPlatformTime;
    private float goThroughPlatformTimer;
    public float goThroughPercentage;

    //Dash To Player  
    public float dashToPlayerRange;
    public float dashToPlayerTime;
    private float dashToPlayerTimer;
    public float dashToPlayerPercentage;
    private Vector2 helpTets;

    #endregion

    private void Charge()
    {
        //Switch to attack when near player
        if (this.chargeToAttackRange > Vector2.Distance(this.transform.position, this.playerTransform.position))
        {
            this.enemyStates = EnemyStates.Attack;
            this.behaviorManager.clockTimer = UnityEngine.Random.Range(this.behaviorManager.attackClockMin, this.behaviorManager.attackClockMax);
            return;
        }

        //Update Target Position
        if (this.chargeState == ChargeState.Move)
        {
            this.updateTargetPositionTimer -= Time.deltaTime;

            if (this.updateTargetPositionTimer <= 0)
            {
                float range = Vector2.Distance(this.playerTransform.position, this.transform.position) / 10 * this.targetPositionRadius;
                range += this.minTargetPositionRadius;

                Vector2 position = new Vector2(this.playerTransform.position.x, this.playerTransform.position.y + 0.7f);

                this.targetPosition = new Vector2(UnityEngine.Random.Range(position.x - range, position.x + range), UnityEngine.Random.Range(position.y - range, position.y + range));

                this.inputControls.inputX = this.transform.position.x - this.targetPosition.x <= 0 ? 1 : -1;
                this.updateTargetPositionTimer = UnityEngine.Random.Range(this.updateTargetPositionTimeMin, this.updateTargetPositionTimeMax);
            }
        }

        this.GoTorwardsTarget(this.targetPosition, this.playerTransform);
    }

    #region Flee Variables

    [Header("Flee")]
    public Transform[] fleePositions;
    private List<Transform> currentFleePositions = new List<Transform>();
    private Transform finalTransform;
    public float updatePositionTimeMin;
    public float updatePositionTimeMax;
    private float updatePositionTimer;
    private float checkYLength;
    public float checkPointLength;

    public Transform arenaMidpoint;
    public float arenaRadiusX;
    public float sideToPlayerMaxRange;
    public float midToEnemyMaxRange;

    #endregion

    private void Flee()
    {
        //Update Target Position
        if (this.chargeState == ChargeState.Move)
        {
            this.updatePositionTimer -= Time.deltaTime;

            if (this.updatePositionTimer <= 0)
            {
                //Search for direction
                float sidePosX = this.playerTransform.position.x < this.arenaMidpoint.position.x ? this.arenaMidpoint.position.x - this.arenaRadiusX : this.arenaMidpoint.position.x + this.arenaRadiusX;
                float sideToPlayerRange = Mathf.Abs(sidePosX - this.playerTransform.position.x);
                float midToEnemyRange = Mathf.Abs(this.arenaMidpoint.transform.position.x - this.transform.position.x);

                if (sideToPlayerRange < this.sideToPlayerMaxRange && midToEnemyRange > this.midToEnemyMaxRange)
                {
                    foreach (Transform item in this.fleePositions)
                    {
                        if (!(this.playerTransform.position.x - this.transform.position.x < 0 && item.position.x - this.transform.position.x > 0 ||
                            this.playerTransform.position.x - this.transform.position.x > 0 && item.position.x - this.transform.position.x < 0) &&
                            !Physics2D.OverlapCircle(item.position, this.checkPointLength, this.playerMask))
                        {
                            this.currentFleePositions.Add(item);
                        }
                    }
                }
                else
                {
                    foreach (Transform item in this.fleePositions)
                    {
                        if ((this.playerTransform.position.x - this.transform.position.x < 0 && item.position.x - this.transform.position.x > 0 ||
                            this.playerTransform.position.x - this.transform.position.x > 0 && item.position.x - this.transform.position.x < 0) &&
                            !Physics2D.OverlapCircle(item.position, this.checkPointLength, this.playerMask))
                        {
                            this.currentFleePositions.Add(item);
                        }
                    }
                }

                this.checkYLength = 0;
                this.finalTransform = this.transform;

                //Search for farest point
                foreach (Transform item in this.currentFleePositions)
                {
                    float length = Mathf.Abs(item.position.y - this.playerTransform.position.y);

                    if (length > this.checkYLength)
                    {
                        this.checkYLength = length;
                        this.finalTransform = item;
                    }
                }

                this.inputControls.inputX = this.transform.position.x - this.finalTransform.position.x <= 0 ? 1 : -1;
                this.updatePositionTimer = UnityEngine.Random.Range(this.updatePositionTimeMin, this.updatePositionTimeMax);
            }

            this.currentFleePositions.Clear();
        }

        if (this.finalTransform == null)
        {
            this.enemyStates = EnemyStates.Charge;
        }

        this.GoTorwardsTarget(this.finalTransform.position, this.finalTransform);
    }

    #region Duell Variables

    [Header("Duell")]
    public float duellToAttackRange;
    private DuellStates duellState;
    private bool duellDash;
    public float dashTimeMin;
    public float dashTimeMax;
    private float duellDashTime;

    public float followTimeMin;
    public float followTimeMax;
    private float followTimer;
    public float recoverOfAttackTime;
    public float recoverOfAttackRange;
    [HideInInspector] public float recoverOfAttackTimer;

    #endregion

    private void Duell()
    {
        this.wallSlide_Node.Evaluate();

        this.recoverOfAttackTimer -= Time.deltaTime;

        switch (this.duellState)
        {
            case DuellStates.Move:

                //Switch to attack when near player
                if (this.duellToAttackRange > Vector2.Distance(this.transform.position, this.playerTransform.position))
                {
                    this.enemyStates = EnemyStates.Attack;
                    this.behaviorManager.clockTimer = UnityEngine.Random.Range(this.behaviorManager.attackClockMin, this.behaviorManager.attackClockMax);
                    return;
                }

                //Check for Dash
                if (this.duellDash)
                {
                    this.duellDashTime -= Time.deltaTime;

                    if (this.duellDashTime <= 0)
                    {
                        if (this.onDash_Node.Evaluate() == NodeStates.SUCCESS)
                        {
                            this.helpTets = this.transform.position;
                            this.duellState = DuellStates.Dash;
                        }

                        this.duellDash = false;
                    }
                }

                //Check for double jump
                if (this.behavior.CanDoubleJump(this.controller, this.inputControls, this.playerTransform.position) && this.jumpCounter < this.maxJumpCounter && this.playerCharacter.inputControls.positiveInputX > 0.2f)
                {
                    if (this.jumpDown_Node.Evaluate() == NodeStates.SUCCESS)
                    {
                        this.duellState =  DuellStates.DoubleJump;
                        this.doubleJumpTimer = UnityEngine.Random.Range(this.doubleJumpTimeMin, this.doubleJumpTimeMax);
                        break;
                    }
                }

                //Check for jumpdash
                if (this.behavior.CanJumpDash(ref this.dashDirection, this.inputControls, this.inputControls.lookDirection, this.playerTransform.position, this.jumpCounter < this.maxJumpCounter))
                {
                    this.duellState = DuellStates.JumpDash;
                    this.jumpDown_Node.Evaluate();
                    this.doubleJumpTimer = UnityEngine.Random.Range(this.doubleJumpTimeMin, this.doubleJumpTimeMax);
                    break;
                }

                //Move horizontal
                if (this.recoverOfAttackTimer <= 0 && this.recoverOfAttackRange < Vector2.Distance(this.transform.position, this.playerTransform.position))
                {
                    if (Mathf.Abs(this.inputControls.inputX - this.playerCharacter.inputControls.inputX) > 0.2f)
                    {
                        this.followTimer -= Time.deltaTime;

                        if (this.followTimer <= 0)
                        {
                            this.inputControls.inputX = this.playerCharacter.inputControls.inputX;
                            this.followTimer = UnityEngine.Random.Range(this.followTimeMin, this.followTimeMax);
                        }
                    }
                }
                else
                {
                    this.inputControls.inputX = this.transform.position.x - this.targetPosition.x <= 0 ? -1 : 1;
                }


                break;

            case DuellStates.DoubleJump:

                this.doubleJumpTimer -= Time.deltaTime;

                if (this.doubleJumpTimer <= 0)
                {
                    this.jumpDown_Node.Evaluate();
                    this.duellState = DuellStates.Move;
                }
                break;

            case DuellStates.JumpDash:

                this.doubleJumpTimer -= Time.deltaTime;

                if (this.doubleJumpTimer > 0)
                {
                    break;
                }

                if (!this.dash)
                {
                    if (this.onDash_Node.Evaluate() == NodeStates.FAILURE)
                    {
                        this.duellState = DuellStates.Move;
                        this.dashing = false;
                        break;
                    }

                    this.dash = true;
                }

                if (this.dash_Node.Evaluate() == NodeStates.SUCCESS)
                {
                    this.duellState = DuellStates.Move;
                    this.dashing = false;
                    this.dash = false;
                }

                break;

            case DuellStates.Dash:

                if (this.dash_Node.Evaluate() == NodeStates.SUCCESS)
                {
                    this.duellState = DuellStates.Move;
                    this.dashing = false;
                }

                break;
        }

        this.move_Node.Evaluate();
        this.gravity_Node.Evaluate();
        this.controller.Move(this.velocity * Time.deltaTime, new Vector2(this.inputControls.inputX, this.inputControls.inputY));
        this.resetVelocity_Node.Evaluate();
    }

    #region Attack Variables

    [Header("Attack")]
    public AttackPercentages neutralP;
    public AttackPercentages angerP;
    public AttackPercentages arroganceP;
    public AttackPercentages eleganceP;
    public AttackPercentages fearP;
    private float[] neutral;
    private float[] anger;
    private float[] arrogance;
    private float[] elegance;
    private float[] fear;
    private Func<bool>[] canAttackFunctions;
    private Func<bool>[] attackFunctions;
    [HideInInspector] public AttackStates attackStates;
    public float rangeNotToMove;
    public float attackToChargeRange;

    public AttackReactionTime attackReactionTime;
    private float attackReactionTimer;
    private bool parried;
    #endregion

    private void Attack()
    {
        this.inputControls.inputX = (Physics2D.OverlapCircle(this.transform.position, this.rangeNotToMove, this.playerMask) == null || !this.controller.collisions.below) ? (this.playerTransform.position.x - this.transform.position.x < 0 ? -1 : 1) : 0;
        //this.inputControls.lookDirection = (int)this.inputControls.inputX;
        if (this.inputControls.inputX == 0)
        {
            if (this.weaponState != WeaponState.Strike && this.weaponState != WeaponState.Recovery)
            {
                this.inputControls.lookDirection = this.playerTransform.position.x - this.transform.position.x < 0 ? -1 : 1;
            }
        }

        switch (this.attackStates)
        {
            #region case Move
            case AttackStates.Move:

                this.updateCharacterGraphic_Node.Evaluate();

                //Switch to attack when near player
                if (this.attackToChargeRange < Vector2.Distance(this.transform.position, this.playerTransform.position))
                {
                    this.enemyStates = EnemyStates.Charge;
                    this.behaviorManager.clockTimer = UnityEngine.Random.Range(this.behaviorManager.chargeClockMin, this.behaviorManager.chargeClockMax);
                    return;
                }

                this.attackReactionTimer -= Time.deltaTime;

                if (this.attackReactionTimer <= 0)
                {
                    float[] finalTemp = this.CalculateAttackPercentages();
                    float[] final = new float[finalTemp.Length];

                    for (int i = 0; i < final.Length; i++)
                    {
                        final[i] = finalTemp[i];
                    }


                    float maxValue = 0;

                    for (int i = 0; i < final.Length; i++)
                    {
                        if (!this.canAttackFunctions[i].Invoke())
                        {
                            final[i] = 0;
                        }

                        maxValue += final[i];                      
                    }

                    float randomValue = UnityEngine.Random.Range(0, maxValue);

                    float currentvalue = 0;

                    for (int i = 0; i < final.Length; i++)
                    {
                        currentvalue += final[i];

                        if (currentvalue > randomValue)
                        {
                            this.attackFunctions[i].Invoke();
                            break;
                        }
                    }
                }
                break;
            #endregion

            #region case AttackDown
            case AttackStates.AttackDown:

                if (this.meleeAttack_Node.Evaluate() == NodeStates.SUCCESS)
                {
                    this.attackStates = AttackStates.Move;
                }

                break;
            #endregion

            #region case AttackFront
            case AttackStates.AttackFront:

                if (this.weaponState == WeaponState.Strike && this.behavior.CanAttackFront())
                {
                    this.onMeleeAttack_Node.Evaluate();
                }

                if (this.meleeAttack_Node.Evaluate() == NodeStates.SUCCESS)
                {
                    this.attackStates = AttackStates.Move;
                }

                return;
                #endregion

            #region case AttackHeavyDown
            case AttackStates.AttackHeavyDown:

                if (this.meleeAttack_Node.Evaluate() == NodeStates.SUCCESS)
                {
                    this.attackStates = AttackStates.Move;
                }

                return;
            #endregion

            #region case AttackUp
            case AttackStates.AttackUp:

                if (this.weaponState == WeaponState.Strike && this.behavior.CanAttackFront())
                {
                    this.inputControls.inputY = 0;
                    this.attackReactionTimer = this.attackReactionTime.attackFront;
                    this.onMeleeAttack_Node.Evaluate();
                }

                if (this.meleeAttack_Node.Evaluate() == NodeStates.SUCCESS)
                {
                    this.attackStates = AttackStates.Move;
                }

                return;
            #endregion

            #region case Dash
            case AttackStates.Dash:

                if (this.dash_Node.Evaluate() == NodeStates.SUCCESS)
                {
                    this.attackStates = AttackStates.Move;
                    this.dashing = false;
                }

                break;
                #endregion

            #region case DoubleJump
            case AttackStates.DoubleJump:

                this.doubleJumpTimer -= Time.deltaTime;

                if (this.doubleJumpTimer <= 0)
                {
                    this.jumpDown_Node.Evaluate();
                    this.attackStates = AttackStates.Move;
                }

                break;
            #endregion

            #region case Parry
            case AttackStates.Parry:

                if (this.parry_Node.Evaluate() == NodeStates.SUCCESS)
                {
                    if (this.behavior.CanAttackFront())
                    {
                        //Switch to attack + combo
                        this.attackStates = AttackStates.AttackFront;
                        break;
                    }
                    else
                    {
                        this.attackStates = AttackStates.Move;
                    }
                }

                break;
                #endregion
        }

        this.move_Node.Evaluate();
        this.gravity_Node.Evaluate();
        this.controller.Move(this.velocity * Time.deltaTime, new Vector2(this.inputControls.inputX, this.inputControls.inputY));
        this.resetVelocity_Node.Evaluate();
    }

    #region Attack States

    //AttackDown
    private bool OnAttackDown()
    {
        this.inputControls.inputY = -1;

        if (this.onMeleeAttack_Node.Evaluate() == NodeStates.SUCCESS)
        {
            this.attackStates = AttackStates.AttackDown;
            this.attackReactionTimer = this.attackReactionTime.attackDown;

            //print("AttackDown NOW!!!!!!!!!!!!");
            return true;
        }
        return false;
    }

    //AttackFront
    private bool OnAttackFront()
    {
        this.inputControls.inputY = 0;

        if (this.onMeleeAttack_Node.Evaluate() == NodeStates.SUCCESS)
        {
            this.attackStates = AttackStates.AttackFront;
            this.attackReactionTimer = this.attackReactionTime.attackFront;

            //print("AttackFront NOW!!!!!!!!!!!!");
            return true;
        }
        return false;
    }

    //AttackHeavyDown
    private bool OnAttackHeavyDown()
    {
        this.inputControls.inputY = -1;

        if (this.onMeleeAttack_Node.Evaluate() == NodeStates.SUCCESS)
        {
            this.attackStates = AttackStates.AttackHeavyDown;
            this.attackReactionTimer = this.attackReactionTime.attackHeavyDown;
            
            //print("AttackHeavyDown NOW!!!!!!!!!!!!");
            return true;
        }
        return false;
    }

    //AttackUp
    private bool OnAttackUp()
    {
        this.inputControls.inputY = 1;

        if (this.onMeleeAttack_Node.Evaluate() == NodeStates.SUCCESS)
        {
            this.attackStates = AttackStates.AttackUp;
            this.attackReactionTimer = this.attackReactionTime.attackUp;

            //print("AttackUp NOW!!!!!!!!!!!!");
            return true;
        }
        return false;
    }

    //DashTorwardsPlayer
    private bool OnDashTorwardsPlayer()
    {
        if (this.onDash_Node.Evaluate() == NodeStates.SUCCESS)
        {
            Vector2 dashPosition = new Vector2(this.playerTransform.position.x, this.playerTransform.position.y + UnityEngine.Random.Range(0f, 2.5f));
            this.dashDirection = (dashPosition - new Vector2(this.transform.position.x, this.transform.position.y)).normalized;

            this.helpTets = this.transform.position;
            this.attackStates = AttackStates.Dash;
            this.attackReactionTimer = this.attackReactionTime.dashTorwardsPlayer;

            //print("DashTorwardsPlayer NOW!!!!!!!!!!!!");
            return true;
        }
        return false;
    }

    //DashAwayFromPlayer
    private bool OnDashAwayFromPlayer()
    {
        if (this.onDash_Node.Evaluate() == NodeStates.SUCCESS)
        {
            float directionX = this.playerTransform.position.x - this.transform.position.x < 0 ? -1 : 1;
            Vector2 dashPosition = new Vector2(directionX, this.transform.position.y + UnityEngine.Random.Range(-1f, 1f)).normalized;
            this.dashDirection = (dashPosition - new Vector2(this.transform.position.x, this.transform.position.y)).normalized;

            this.helpTets = this.transform.position;
            this.attackStates = AttackStates.Dash;
            this.attackReactionTimer = this.attackReactionTime.dashAwayFromPlayer;

            //print("DashAwayFromPlayer NOW!!!!!!!!!!!!");
            return true;
        }

        return false;
    }

    //Dodgeroll
    private bool OnDodgeRoll()
    {
        if (this.onDash_Node.Evaluate() == NodeStates.SUCCESS)
        {
            float directionX = this.playerTransform.position.x - this.transform.position.x < 0 ? 1 : -1;
            this.dashDirection = Vector2.right * directionX;

            this.helpTets = this.transform.position;
            this.attackStates = AttackStates.Dash;
            this.attackReactionTimer = this.attackReactionTime.dodgeRoll;

            //print("Dodgeroll NOW!!!!!!!!!!!!");
            return true;
        }

        return false;
    }

    //DoubleJump
    private bool OnDoublejump()
    {
        if (this.jumpDown_Node.Evaluate() == NodeStates.SUCCESS)
        {
            this.attackStates = AttackStates.DoubleJump;
            this.doubleJumpTimer = UnityEngine.Random.Range(this.doubleJumpTimeMin, this.doubleJumpTimeMax);

            //print("Doublejump NOW!!!!!!!!!!!!");
            return true;
        }

        return false;
    }

    //Jump
    private bool OnJump()
    {
        if( this.jumpDown_Node.Evaluate() == NodeStates.SUCCESS)
        {
            this.attackReactionTimer = this.attackReactionTime.jump;

            //print("Jump NOW!!!!!!!!!!!!");
            return true;
        }

        return false;
    }

    //Parry
    private bool OnParry()
    {
        if (this.onParry_Node.Evaluate() == NodeStates.SUCCESS)
        {
            this.attackStates = AttackStates.Parry;
            this.velocity = Vector2.zero;

            //print("Parry NOW!!!!!!!!!!!!");
            return true;
        }

        return false;
    }

    //GoThroughPlatform
    private bool OnGoThroughPlatform()
    {
        this.inputControls.inputY = -1;
        this.controller.canGoThroughPlatform = true;
        this.attackReactionTimer = this.attackReactionTime.goThroughPlatform;

        //print("GoThroughPlatfrom NOW!!!!!!!!!!!!");
        return true;
    }

    #endregion

    #region Side Functions

    public void UpdateInput()
    {

        this.inputControls.inputY = 0;
        

        //LastInput
        this.inputControls.lastInputX = this.inputControls.inputX;
        this.inputControls.lastInputY = this.inputControls.inputY;

        //Positive Input
        this.inputControls.positiveInputX = Mathf.Abs(this.inputControls.inputX);
        this.inputControls.positiveInputY = Mathf.Abs(this.inputControls.inputY);

        //Last Look Direction
        this.inputControls.lastLookDirection = this.inputControls.lookDirection;

        //Look Direction
        if (Mathf.Abs(this.inputControls.inputX) > 0.2f)
        {
            this.inputControls.lookDirection = (int)Mathf.Sign(this.inputControls.inputX);
        }
    }

    public bool CalculatePercentage(float percentage)
    {
        float randomPercentage = UnityEngine.Random.Range(0f, 1f);

        return percentage > randomPercentage;
    }

    private float[] CalculateAttackPercentages()
    {
        float[] final = new float[this.neutral.Length];

        for (int i = 0; i < final.Length; i++)
        {
            final[i] = this.neutral[i];
        }

        for (int i = 0; i < this.neutral.Length; i++)
        {
            final[i] = this.neutral[i] * EmotionManager.tempNeutral + this.anger[i] * EmotionManager.tempAnger + this.arrogance[i] * EmotionManager.tempArrogance + this.elegance[i] * EmotionManager.tempElegance + this.fear[i] * EmotionManager.tempFear;
            final[i] = final[i] / 100;      
        }

        return final;
    }

    private void GoTorwardsTarget(Vector2 targetPos,  Transform targetTransform)
    {
        this.wallSlide_Node.Evaluate();

        switch (this.chargeState)
        {
            case ChargeState.Move:

                #region Move

                //Update Input
                if (Mathf.Abs(targetTransform.position.x - this.transform.position.x) > this.updateLookDirectionRange)
                {
                    this.inputControls.inputX = this.transform.position.x - targetTransform.position.x <= 0 ? 1 : -1;
                    this.inputControls.lookDirection = (int)this.inputControls.inputX;
                }

                //Check dash to player
                if (Vector2.Distance(this.transform.position, targetTransform.position) > this.dashToPlayerRange)
                {
                    this.dashToPlayerTimer -= Time.deltaTime;

                    if (this.dashToPlayerTimer <= 0)
                    {
                        this.dashToPlayerTimer = this.dashToPlayerTime;

                        if (this.CalculatePercentage(this.dashToPlayerPercentage))
                        {
                            if (this.onDash_Node.Evaluate() == NodeStates.SUCCESS)
                            {
                                this.dashDirection = (targetTransform.position - this.transform.position).normalized;
                                this.helpTets = this.transform.position;
                                this.chargeState = ChargeState.Dash;
                            }
                        }
                    }
                }

                //Check if Target is below
                if (targetPos.y - this.transform.position.y < 0)
                {
                    //Go trough platform
                    this.goThroughPlatformTimer -= Time.deltaTime;
                    RaycastHit2D hit = Physics2D.Raycast(new Vector2(this.controller.collider.bounds.center.x, this.controller.collider.bounds.min.y + 0.1f), Vector2.down, 0.2f, this.controller.collisionMask);

                    if (this.goThroughPlatformTimer <= 0 && hit.collider != null)
                    {
                        if (this.CalculatePercentage(this.goThroughPercentage) && hit.collider.tag == "Through" && this.controller.collisions.below)
                        {
                            this.inputControls.inputY = -1;
                            this.controller.canGoThroughPlatform = true;
                        }

                        this.goThroughPlatformTimer = this.goThroughPlatformTime;
                    }

                    break;
                }

                //Wallslide when touching wall
                if (this.controller.collisions.left || this.controller.collisions.right)
                {
                    this.wallJumpTimer -= Time.deltaTime;

                    if (this.wallJumpTimer <= 0)
                    {
                        this.jumpDown_Node.Evaluate();
                        this.wallJumpTimer = this.wallJumpTime;
                    }
                }

                //Check for double jump
                if (this.behavior.CanDoubleJump(this.controller, this.inputControls, targetPos) && this.jumpCounter < this.maxJumpCounter)
                {
                    if (this.jumpDown_Node.Evaluate() == NodeStates.SUCCESS)
                    {
                        this.chargeState = ChargeState.DoubleJump;
                        this.doubleJumpTimer = UnityEngine.Random.Range(this.doubleJumpTimeMin, this.doubleJumpTimeMax);
                        break;
                    }
                }

                //Check for jumpdash
                if (this.behavior.CanJumpDash(ref this.dashDirection, this.inputControls, this.inputControls.lookDirection, targetPos, this.jumpCounter < this.maxJumpCounter))
                {
                    this.chargeState = ChargeState.JumpDash;
                    this.jumpDown_Node.Evaluate();
                    this.doubleJumpTimer = UnityEngine.Random.Range(this.doubleJumpTimeMin, this.doubleJumpTimeMax);
                    break;
                }
                #endregion

                break;

            case ChargeState.DoubleJump:

                this.doubleJumpTimer -= Time.deltaTime;

                if (this.doubleJumpTimer <= 0)
                {
                    this.jumpDown_Node.Evaluate();
                    this.chargeState = ChargeState.Move;
                }

                break;

            case ChargeState.Dash:

                if (this.dash_Node.Evaluate() == NodeStates.SUCCESS)
                {
                    this.chargeState = ChargeState.Move;
                    this.dashing = false;
                }

                this.gravity_Node.Evaluate();
                this.controller.Move(this.velocity * Time.deltaTime, new Vector2(this.inputControls.inputX, this.inputControls.inputY));
                this.resetVelocity_Node.Evaluate();

                return;

            case ChargeState.JumpDash:

                this.doubleJumpTimer -= Time.deltaTime;

                if (this.doubleJumpTimer > 0)
                {
                    break;
                }

                if (!this.dash)
                {
                    if (this.onDash_Node.Evaluate() == NodeStates.FAILURE)
                    {
                        this.chargeState = ChargeState.Move;
                        this.dashing = false;
                        break;
                    }

                    this.dash = true;
                }

                if (this.dash_Node.Evaluate() == NodeStates.SUCCESS)
                {
                    this.chargeState = ChargeState.Move;
                    this.dashing = false;
                    this.dash = false;
                }

                this.gravity_Node.Evaluate();
                this.controller.Move(this.velocity * Time.deltaTime, new Vector2(this.inputControls.inputX, this.inputControls.inputY));
                this.resetVelocity_Node.Evaluate();

                return;
        }

        this.move_Node.Evaluate();
        this.gravity_Node.Evaluate();

        this.controller.Move(this.velocity * Time.deltaTime, new Vector2(this.inputControls.inputX, this.inputControls.inputY));
        this.resetVelocity_Node.Evaluate();
    }

    public void OnPlayerDash(Vector2 dashDirection)
    {
        if (this.duellState != DuellStates.Move || this.enemyStates != EnemyStates.Duell)
        {
            return;
        }

        this.dashDirection = dashDirection;
        this.duellDash = true;
        this.duellDashTime = UnityEngine.Random.Range(this.dashTimeMin, this.dashTimeMax);
    }

    [Header("Go Torwards Player")]
    public float playerInRange;
    public float playerNotInRange;
    [HideInInspector] public bool goTorwardsPlayer;

    [Header("Phasees")]
    public PhaseStats phaseStats;

    public bool GoTorwardsPlayer(Vector2 targetPosition)
    {
        //if (Vector2.Distance(this.transform.position, targetPosition) < 0.5f && this.controller.collisions.below)
        //{
        //    this.inputControls.inputX = 0;
        //}
        //else
        //{
        //    this.inputControls.inputX = this.targetPosition.x - this.transform.position.x < 0 ? -1 : 1;
        //    this.inputControls.lookDirection = (int)this.inputControls.inputX;
        //}
        //print(this.inputControls.lookDirection);
        //this.move_Node.Evaluate();
        //this.gravity_Node.Evaluate();

        //this.controller.Move(this.velocity * Time.deltaTime, new Vector2(this.inputControls.inputX, this.inputControls.inputY));
        //this.resetVelocity_Node.Evaluate();

        //if (Vector2.Distance(this.transform.position, targetPosition) < 0.5f && this.controller.collisions.below)
        //{
        //    this.goTorwardsPlayer = false;
        //    return true;
        //}
        //else
        //{
        //    this.goTorwardsPlayer = true;
        //    return false;
        //}
        this.transform.position = targetPosition;
        return true;
    }

    #endregion

    #region SwitchStates

    [Header("Test")]
    public TextMeshProUGUI text;

    public void SwitchStateCharge()
    {
        this.enemyStates = EnemyStates.Charge;
        this.text.text = "Current state: Charge";
    }

    public void SwitchStateDuell()
    {
        this.enemyStates = EnemyStates.Duell;
        this.text.text = "Current state: Duell";
    }

    public void SwitchStateFlee()
    {
        this.enemyStates = EnemyStates.Flee;
        this.text.text = "Current state: Flee";
    }

    public void SwitchStateAttack()
    {
        this.enemyStates = EnemyStates.Attack;
        this.text.text = "Current state: Attack";
    }

    #endregion

    #region Healthsystem

    private bool enemyStunned;

    public override void GetDamage(float damage, Vector2 knockBack, float stunTime, bool weightless, MeleeAttack meleeAttack)
    {
        if (this.invincibleTime > 0)
        {
            return;
        }

        this.meleeAttack_Node.superArmour -= damage;
        this.audioManager.PlayDamageSound(this.audioID, true);

        if (this.meleeAttack_Node.superArmour <= 0)
        {
            this.GetStunned(stunTime);
            this.velocity = knockBack;
        }

        this.currentHealth -= damage;
        this.healthSlider.fillAmount = this.currentHealth / this.maxHealth;
        this.weightless = weightless;

        if (this.currentHealth <= 0)
        {
            this.Die();
        }
    }

    public override void Die()
    {
        this.gameManager.OnCompletedPhase(true);
    }

    public override void GetStunned(float stunTime)
    {
        base.GetStunned(stunTime);
        this.enemyStunned = true;
        this.stunTimer = stunTime;
        this.animator.SetTrigger("ResetAttack");
        this.meleeAttack_Node.ResetAttack();
        this.parry_Node.ResetParry();
        this.dash_Node.ResetDash();
    }

    public void ResetFight()
    {
        this.animator.SetTrigger("ResetAttack");
        this.meleeAttack_Node.ResetAttack();
        this.parry_Node.ResetParry();
        this.dash_Node.ResetDash();
        this.stunTimer = 0;
        this.enemyStunned = false;
    }

    #endregion

    private void OnDrawGizmos()
    {
        if (this.showCombatGizmo)
        {
            Gizmos.color = this.weaponState == WeaponState.Strike ? new Color(255, 0, 0, 0.7f) : new Color(255, 0, 0, 0.7f);

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

        if (this.disableGizmo)
        {
            return;
        }

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(this.transform.position, this.playerInRange);
        Gizmos.DrawWireSphere(this.transform.position, this.playerNotInRange);

        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(this.transform.position, this.rangeNotToMove);       

        float range = Vector2.Distance(this.playerTransform.position, this.transform.position) / 10 * this.targetPositionRadius;
        range += this.minTargetPositionRadius;
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(this.targetPosition, 0.1f);
        Gizmos.DrawWireCube(new Vector2(this.playerTransform.position.x, this.playerTransform.position.y + 0.7f), new Vector2(range * 2, range * 2));

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(new Vector2(this.transform.position.x, this.transform.position.y + 1), Vector2.right * this.updateLookDirectionRange);
        Gizmos.DrawRay(new Vector2(this.transform.position.x, this.transform.position.y + 1), Vector2.left * this.updateLookDirectionRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(this.transform.position, 10);

        Gizmos.color = Color.black;
        Gizmos.DrawSphere(this.x, 0.2f);
        Gizmos.DrawSphere(this.y, 0.2f);

        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(this.helpTets, 0.2f);
        Gizmos.DrawRay(this.helpTets, this.dashDirection * this.dashDistance);

        if (this.finalTransform != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(this.finalTransform.position, 0.2f);
        }

        foreach (Transform item in this.currentFleePositions)
        {
            Gizmos.DrawWireSphere(item.position, this.checkPointLength);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawRay(this.arenaMidpoint.position, Vector2.right * this.arenaRadiusX);
        Gizmos.DrawRay(new Vector2(this.arenaMidpoint.position.x, this.arenaMidpoint.position.y + 1), Vector2.right * this.midToEnemyMaxRange);
        Gizmos.DrawRay(new Vector2(this.arenaMidpoint.position.x + this.arenaRadiusX, this.arenaMidpoint.position.y - 1), Vector2.left * this.sideToPlayerMaxRange);

        Gizmos.DrawWireSphere(this.transform.position, this.duellToAttackRange);
        Gizmos.DrawWireSphere(this.transform.position, this.chargeToAttackRange);
        Gizmos.DrawWireSphere(this.transform.position, this.recoverOfAttackRange);
        Gizmos.DrawWireSphere(this.transform.position, this.attackToChargeRange);
    }
}

public enum EnemyStates
{
    Charge,
    Flee,
    Duell,
    Attack
}

public enum ChargeState
{
    Move,
    DoubleJump,
    Dash,
    JumpDash,
    DoubleJumpDash
}

public enum DuellStates
{
    Move,
    DoubleJump,
    JumpDash,
    Dash
}

public enum AttackStates
{
    Move,
    AttackDown,
    AttackFront,
    AttackHeavyDown,
    AttackUp,
    Dash,
    DoubleJump,
    Parry,
}

[System.Serializable]
public struct AttackPercentages
{
    public float attackFront;
    public float attackUp;
    public float attackDown;
    public float attackHeavyDown;
    public float dodgeRoll;
    public float parry;
    public float dashTorwardsPlayer;
    public float dashAwayFromPlayer;
    public float jump;
    public float doubleJump;
    public float throughPlatform;
}

[System.Serializable]
public struct AttackReactionTime
{
    public float attackDown, attackHeavyDown, attackFront, attackUp;
    public float dashTorwardsPlayer, dashAwayFromPlayer;
    public float dodgeRoll;
    public float doubleJump, jump;
    public float goThroughPlatform;
}

[System.Serializable]
public struct PhaseStats
{
    public AttackReactionTime phaseOneReactionTime;
    public AttackReactionTime phaseTwoReactionTime;
    public float phaseOneHealth;
    public float phaseTwoHealth;
    public float phaseOneMoveSpeed;
    public float phaseTwoMoveSpeed;
    public MeleeWeapon phaseOneWeapon;
    public MeleeWeapon phaseTwoWeapon;
}