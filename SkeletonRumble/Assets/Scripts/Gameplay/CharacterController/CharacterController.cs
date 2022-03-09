using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using TMPro;
using UnityEngine.UI;

public class CharacterController : MonoBehaviour, IDamage
{
    #region Character Variables

    [Header("Character")]
    public GameObject characterGraphic;
    public float graphicAdjustSpeed;
    public bool showCombatGizmo;
    public bool showAnimationGizmo;
    public bool showParryGizmo;
    public bool useHitToCombo;
    [HideInInspector] public InputControls inputControls;
    public float maxHealth;
    [HideInInspector] public float currentHealth;
    [HideInInspector] public bool isAbleToMove;

    #endregion

    #region Physic Variables

    [HideInInspector] public float gravity;
    public float maxgravity;
    [HideInInspector] public Vector3 velocity;
    [HideInInspector] public Controller2D controller;
    public float gravityMultiplyer;

    #endregion

    #region Movement Variables

    [Header("Movement")]
    public float walkSpeed;
    public float runSpeed;
    [HideInInspector] public float moveSpeed;
    public float accelerationTimeAirborne;
    public float accelerationTimeGrounded;
    public float velocityXSmoothing;
    private float canMove;
    [Range(0, 1)] public float runInput;

    #endregion

    #region Jump Variables

    [Header("Jump")]
    [SerializeField] private float maxJumpHeight;
    [SerializeField] private float minJumpHeight;
    [SerializeField] private float timeToJumpApex;
    [HideInInspector] public float maxJumpVelocity;
    [HideInInspector] public float minJumpVelocity;
    public int maxJumpCounter;
    [HideInInspector] public int jumpCounter;

    #endregion

    #region Walljump Variables

    [Header("Walljump")]
    public float wallSlideSpeedMax;
    public Vector2 wallJumpClimb;
    public Vector2 wallJumpOff;
    public Vector2 wallJumpLeap;
    [HideInInspector] public int wallDirX;
    [HideInInspector] public bool wallSliding;
    public float wallStickTime = .25f;
    [HideInInspector] public float timeToWallUnstick;
    #endregion

    #region Wallrun Variables

    [Header("Wallrun")]
    public float wallrunTimeGround;
    public float wallrunTimeJump;
    [HideInInspector] public float wallrunTime;
    [SerializeField] private float wallrunDistanceGround;
    [SerializeField] private float wallrunDistanceJump;
    [HideInInspector] public float wallrunVelocityGround;
    [HideInInspector] public float wallrunVelocityJump;
    [HideInInspector] public float wallrunVelocity;
    [HideInInspector] public bool wallrunning;
    [HideInInspector] public bool canWallrun;

    #endregion

    #region Dash Variables

    [Header("Dash")]
    public float dashCoolDownValue;
    [HideInInspector] public float dashCooldown;
    public float dashTimeValue;
    public float dodgeRollTimeValue;
    [HideInInspector] public float dashTime;
    public float dashDistance;
    [HideInInspector] public float dashVelocity;
    [HideInInspector] public float dodgeRollVelocity;
    [HideInInspector] public int dashDirectionX;
    [HideInInspector] public Vector2 dashDirection;
    [HideInInspector] public bool dodgeRoll;
    [Range(0,1)] public float dashAngle;
    [HideInInspector] public bool dashing;
    public int maxDashCounter;
    [HideInInspector] public int dashCounter;
    [Range(0, 1)] public float inviciblePercent;
    [HideInInspector] public float invincibleTime;

    #endregion

    #region Animation Variables

    [Header("Animation")]
    public float timeToMoveValue;
    [HideInInspector] public float timeToMove;
    public float timeToIdleValue;
    [HideInInspector] public float timeToIdle;
    [HideInInspector] public Animator animator;

    [Tooltip("Order of Locator: RightHand, Lefthand, RightFoot, leftFoot")]
    public Transform[] rigController;

    [Tooltip("Order of Locator: RightHand, Lefthand, RightFoot, leftFoot")]
    public Rig[] rigs;

    public Vector2[] footSpacing;

    public float yOffset;
    public float footOffset;
    public float footBlendTime;

    #endregion

    #region Attack Variables

    [Header("Attack")]
    [Range(0, 1)] public float attackUpInput;
    public LayerMask damageMask;
    [HideInInspector] public AttackDirection attackDirection;
    [HideInInspector] public float attackTimer;
    public Transform hammerTransform;
    public Transform swordTransform;
    public Transform[] attackPositions;
    [HideInInspector] public bool performingCombo;
    [Range(0, 1)] public float comboPercentage;

    [HideInInspector] public int currentComboAmount;
    [HideInInspector] public bool canDoDamage;
    [HideInInspector] public bool canPerformCombo;

    public MeleeWeapon currentMeleeWeapon;
    [HideInInspector] public IDamage damagedObject;

    public float attackUpOffset;

    public float attackGravity;
    public float accelerationTimeAttackX;
    public TextMeshProUGUI comboCount;
    public SpriteRenderer[] attackSprites;
    public int attackDirectionInt;

    [HideInInspector] public float inputqueueAnticipationTime;
    [HideInInspector] public float inputqueueRecoveryTime;

    public float attackAngleUp;
    public float attackAngleDown;
    public Transform attackHolder;
    [HideInInspector] public float attackVelocity;
    [HideInInspector] public Vector2 attackDirectionVector;
    [HideInInspector] public WeaponState weaponState;
    [HideInInspector] public bool heavyHit;
    [HideInInspector] public bool canAttackUp;
    [HideInInspector] public bool hitTarget;
    public Collider swordCollider;
    public Collider hammerCollider;

    #endregion

    #region Damage Variables

    [Header("Health")]
    public Image healthSlider;
    public bool weightless;
    public float stunTimer;
    public GameObject stunGraphic;
    public float smoothingY;

    public float stunAccelerationX;
    public float stunAccelerationY;
    public float stunGravity;


    #endregion

    #region Parry Variables

    [Header("Parry")]
    public Transform parryTransform;
    public LayerMask weaponMask;
    [HideInInspector] public float parryDuration;
    [HideInInspector] public float parryCooldown;
    [HideInInspector] public bool canDoParry;
    [HideInInspector] public bool canParryCombo;
    [HideInInspector] public WeaponState parryState;
    [HideInInspector] public bool parried;

    #endregion

    #region Colliding

    [Header("Colliding")]
    public LayerMask playerMask;
    public float rayLength;
    public float skinWidth;
    public float pushSpeed;
    public float circleRadius;
    public float goTroughTime;
    [HideInInspector] public float goTroughTimer;

    #endregion

    #region Audio

    [Header("Audio")]
    public int audioID;
    [HideInInspector] public AudioManager audioManager;
    [HideInInspector] public bool groundType;
    [HideInInspector] public bool lastCollisionBelow;
    [HideInInspector] public bool lastWallsliding;
    public bool weaponType;

    public Vector2 x;
    public Vector2 y;
    #endregion

    [HideInInspector] public GameManager gameManager;
    [HideInInspector] public int airCombo;

    public virtual void Start()
    {
        //Set Character
        this.currentHealth = this.maxHealth;

        //Set Controller
        this.controller = this.GetComponent<Controller2D>();
        this.inputControls.lookDirection = 1;

        //Set Animator
        this.animator = this.GetComponentInChildren<Animator>();

        //Jump
        this.gravity = -(2 * this.maxJumpHeight) / Mathf.Pow(this.timeToJumpApex, 2);
        this.maxJumpVelocity = Mathf.Abs(this.gravity) * this.timeToJumpApex;
        this.minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(this.gravity) * this.minJumpHeight);

        //Dash
        this.dashVelocity = this.dashDistance / this.dashTimeValue;
        this.dodgeRollVelocity = this.dashDistance / this.dodgeRollTimeValue;

        //Wallrun
        this.wallrunVelocityGround = this.wallrunDistanceGround / this.wallrunTimeGround;
        this.wallrunVelocityJump = this.wallrunDistanceJump / this.wallrunTimeJump;

        //Set Manager
        this.audioManager = FindObjectOfType<AudioManager>();
        this.gameManager = FindObjectOfType<GameManager>();
    }

    public virtual void Die()
    {
        print("I died");
    }

    public virtual void GetDamage(float damage, Vector2 knockBack, float stunTime, bool weightless, MeleeAttack meleeAttack)
    {
        print(this.gameObject.name + ": I got Damage");
    }

    public virtual void GetStunned(float stunTime)
    {
        //print(this.gameObject.name + ": I got stunned");
    }

    public struct InputControls
    {
        public float inputX, inputY;
        public float lastInputX, lastInputY;
        public float positiveInputX, positiveInputY;

        public int lookDirection;
        public int lastLookDirection;
    }
}
