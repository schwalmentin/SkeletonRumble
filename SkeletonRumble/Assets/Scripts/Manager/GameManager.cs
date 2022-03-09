using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Cinemachine;

public class GameManager : MonoBehaviour
{
    #region Variables

    [Header("Characters")]
    //Player
    public Transform playerSpawnTransform;
    public Transform playerHubTransform;
    private PlayerCharacter playerCharacter;

    //Enemy
    public Transform enemySpawnTransform;
    private EnemyController enemyCharacter;

    //Manager
    private TeleportManager tpManager;
    private PlayerInput inputManager;
    private CameraManager cameraManager;
    private SaveStatusValues statusManager;
    private PostProcessingProfileSwitcher profileManager;

    //Image Lerp
    [Header("Image Lerp")]
    public float lerpSpeed;
    public LayerMask playerMask;
    private float timer;

    //On Fight
    [Header("On Fight")]
    public Vector2 fightSize;
    public ButtonPrompts fightUI;
    public Animator onFightAnimator;
    public Animator healthbarAnimator;
    private bool fightInRange;

    [Header("Weapon Selection")]
    public MeleeWeapon sword;
    public MeleeWeapon hammer;
    [Space]
    public Transform swordTransform;
    public Transform hammerTransform;
    [Space]
    public Vector2 swordSize;
    public Vector2 hammerSize;
    [Space]
    public ButtonPrompts swordUI;
    public ButtonPrompts hammerUI;
    private bool inSwordRange;
    private bool inHammerRange;

    [Header("Fight")]
    public Animator fightAnimator;
    public Image phase1Count;
    public Image phase2Count;
    private int phaseCounter;
    //Change Button Prompts
    private string currentInputDevice;
    private bool firstDevice;

    //Dialog
    public float dialogCameraMoveSpeed;
    private Construct_Dialogue dialogConstructer;
    private Display_Dialogue displayDialog;
    private bool onAlreadyInteracted;
    private bool playerLost;
    private bool enemyLost;

    //Pathfinding
    [Header("Pathfinding")]
    public GameObject[] platforms;

    //Ending
    [Header("Ending")]
    public Animator endingAnimator;
    #endregion

    private void Start()
    {
        this.playerCharacter = FindObjectOfType<PlayerCharacter>();
        this.enemyCharacter = FindObjectOfType<EnemyController>();
        this.tpManager = FindObjectOfType<TeleportManager>();
        this.cameraManager = FindObjectOfType<CameraManager>();
        this.profileManager = FindObjectOfType<PostProcessingProfileSwitcher>();

        this.dialogConstructer = FindObjectOfType<Construct_Dialogue>();
        this.displayDialog = FindObjectOfType<Display_Dialogue>();
        this.inputManager = FindObjectOfType<PlayerInput>();
        this.statusManager = FindObjectOfType<SaveStatusValues>();

        this.enemyCharacter.velocity.x = 0;
        this.enemyCharacter.isAbleToMove = false;

        //+statusmanager updaten
        //this.dialogConstructer.ConstructDialogue(true);
        //this.inputManager.SwitchCurrentActionMap("UI");
        this.onAlreadyInteracted = false;

        Cursor.visible = false;
    }

    private void Update()
    {
        float targetScreenX = this.displayDialog.answer ? 0.53f : 0.43f;
        this.cameraManager.dialogCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_ScreenX = Mathf.MoveTowards(this.cameraManager.dialogCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_ScreenX, targetScreenX, this.dialogCameraMoveSpeed * Time.deltaTime);

        if (this.playerCharacter.isAbleToMove && this.enemyCharacter.isAbleToMove)
        {
            //print("Player and Enemy can move");
            if (this.phaseCounter == 1)
            {
                this.phase1DurationRealtime += Time.deltaTime;
            }

            if (this.phaseCounter == 2)
            {
                this.phase2DurationRealtime += Time.deltaTime;
            }
        }

        this.SwitchUI(this.transform, this.fightSize, this.fightUI, ref this.fightInRange);
        this.SwitchUI(this.swordTransform, this.swordSize, this.swordUI, ref this.inSwordRange);
        this.SwitchUI(this.hammerTransform, this.hammerSize, this.hammerUI, ref this.inHammerRange);

        this.playerCharacter.cannotReceiveInput = Physics2D.OverlapBox(this.transform.position, this.fightSize, 0, this.playerMask) ||
            Physics2D.OverlapBox(this.swordTransform.position, this.swordSize, 0, this.playerMask) ||
            Physics2D.OverlapBox(this.hammerTransform.position, this.hammerSize, 0, this.playerMask);

        this.onFightAnimator.SetBool("ReadyToFight", this.fightInRange);

        this.UpdateEnemyLost();
        this.UpdatePlayerLost();
    }

    private bool inNearOfPlayer;
    private bool inNearOfEnemy;
    private bool playerDead;
    private bool enemyDead;

    #region Lose States

    private void UpdatePlayerLost()
    {
        if (!this.playerLost)
        {
            return;
        }

        this.playerCharacter.stunGraphic.SetActive(false);

        foreach (var item in this.platforms)
        {
            item.SetActive(false);
        }

        if (this.enemyCharacter.GoTorwardsPlayer(this.enemySpawnTransform.position) && this.playerCharacter.GoTorwardsEnemy(this.playerSpawnTransform.position))
        {
            foreach (var item in this.platforms)
            {
                item.SetActive(true);
            }

            this.enemyCharacter.goTorwardsPlayer = false;
            this.playerCharacter.goTorwardsEnemy = false;

            this.enemyCharacter.velocity.x = 0;
            this.playerCharacter.velocity.x = 0;

            this.cameraManager.SwitchToDialog();
            this.StartCoroutine(this.TriggerDialog(0.2f));

            this.playerLost = false;
            this.playerDead = true;
        }
    }

    private void UpdateEnemyLost()
    {
        if (!this.enemyLost)
        {
            return;
        }

        this.enemyCharacter.stunGraphic.SetActive(false);


        foreach (var item in this.platforms)
        {
            item.SetActive(false);
        }

        //print("Enemy lost pls go to spawnPoints");
        if (this.enemyCharacter.GoTorwardsPlayer(this.enemySpawnTransform.position) && this.playerCharacter.GoTorwardsEnemy(this.playerSpawnTransform.position))
        {
            this.playerCharacter.goTorwardsEnemy = false;
            this.enemyCharacter.goTorwardsPlayer = false;

            this.playerCharacter.velocity.x = 0;
            this.enemyCharacter.velocity.x = 0;

            this.cameraManager.SwitchToDialog();
            this.StartCoroutine(this.TriggerDialog(0.2f));

            this.enemyLost = false;
            this.enemyDead = true;

            foreach (var item in this.platforms)
            {
                item.SetActive(true);
            }
        }        
    }

    #endregion

    #region On ...
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }

        //On Fight
        if (Physics2D.OverlapBox(this.transform.position, this.fightSize, 0, this.playerMask))
        {
            //Teleport Player
            this.tpManager.TeleportPlayer(this.playerSpawnTransform.position, true, true);
            this.tpManager.screenColor = Color.black;
        }

        //On select sword
        if (Physics2D.OverlapBox(this.swordTransform.position, this.swordSize, 0, this.playerMask))
        {
            this.playerCharacter.currentMeleeWeapon = this.sword;
            if (this.playerCharacter.animator.GetInteger("WeaponIndex") != 0)
            {
                this.playerCharacter.animator.SetInteger("WeaponIndex", 0);
                this.playerCharacter.animator.SetTrigger("SwitchWeapon");
            }

            this.playerCharacter.attackPositions[0] = this.playerCharacter.swordTransform;
            this.playerCharacter.attackPositions[1] = this.playerCharacter.swordTransform;
            this.playerCharacter.attackPositions[2] = this.playerCharacter.swordTransform;
        }

        //On select hammer
        if (Physics2D.OverlapBox(this.hammerTransform.position, this.hammerSize, 0, this.playerMask))
        {
            this.playerCharacter.currentMeleeWeapon = this.hammer;
            if (this.playerCharacter.animator.GetInteger("WeaponIndex") != 1)
            {
                this.playerCharacter.animator.SetInteger("WeaponIndex", 1);
                this.playerCharacter.animator.SetTrigger("SwitchWeapon");
            }

            this.playerCharacter.attackPositions[0] = this.playerCharacter.hammerTransform;
            this.playerCharacter.attackPositions[1] = this.playerCharacter.hammerTransform;
            this.playerCharacter.attackPositions[2] = this.playerCharacter.hammerTransform;
        }
    }

    public void OnTeleported()
    {
        this.phase1DurationRealtime = 0;
        this.phase2DurationRealtime = 0;

        //Set Player
        this.playerCharacter.inputControls.lookDirection = 1;
        this.playerCharacter.updateCharacterGraphic_Node.Evaluate();

        //Set Enemy
        this.enemyCharacter.gameObject.SetActive(true);
        this.enemyCharacter.isAbleToMove = false;
        this.enemyCharacter.velocity.x = 0;
        this.enemyCharacter.inputControls.lookDirection = -1;
        this.enemyCharacter.updateCharacterGraphic_Node.Evaluate();
        this.enemyCharacter.transform.position = this.enemySpawnTransform.position;
        this.phaseCounter = 0;

        //Dialog
        if (!this.onAlreadyInteracted)
        {
            this.StartCoroutine(this.TriggerDialog(1));
            this.onAlreadyInteracted = true;
        }
        else
        {
            this.StartCoroutine(this.TriggerFight(2));
            this.cameraManager.SwitchToDialog();
        }

        this.profileManager.SwitchProfile(Profiles.Normal);
    }

    public void OnCompletedPhase(bool playerWon)
    {
        this.playerCharacter.isAbleToMove = false;
        this.enemyCharacter.isAbleToMove = false;
        this.playerCharacter.velocity.x = 0;
        this.enemyCharacter.velocity.x = 0;
        this.playerCharacter.ResetFight();
        this.enemyCharacter.ResetFight();


        this.playerLost = !playerWon;
        this.enemyLost = playerWon;

        //Healthbar
        this.healthbarAnimator.SetBool("Fighting", false);

        //Profiles
        this.profileManager.fighting = false;
        if (playerWon)
        {
            this.profileManager.SwitchProfile(Profiles.Win);
        }
        else
        {
            this.profileManager.SwitchProfile(Profiles.Lose);
        }
    }

    public void OnEndDialog()
    {
        //Reset Status Manager
        this.playerParriesPerPhase = 0;
        this.playerJumpsPerPhase = 0;
        this.playerDashesPerPhase = 0;
        this.playerWalljumpsPerPhase = 0;
        this.longestAirCombo = 0;

        //End Dialog
        this.enemyCharacter.animator.SetBool("Emotion", false);
        this.inputManager.SwitchCurrentActionMap("Player");

        if (this.playerDead)
        {
            //this.StartCoroutine(this.TriggerTeleport(1, this.playerHubTransform));
            this.tpManager.TeleportPlayer(this.playerHubTransform.position);
            this.cameraManager.SwitchToPlayer();
            this.playerDead = false;
            this.profileManager.SwitchProfile(Profiles.Normal);

            this.playerCharacter.isAbleToMove = true;
            return;
        }

        if (this.enemyDead && this.phaseCounter == 2)
        {
            //Show tombs + end game/go to credits
            this.playerCharacter.isAbleToMove = false;
            this.enemyCharacter.isAbleToMove = false;
            this.profileManager.SwitchProfile(Profiles.Normal);
            this.cameraManager.SwitchToEnding();
            this.endingAnimator.SetBool("Armageddon", true);
            print("DIE");
            return;
        }

        this.enemyDead = false;
        this.StartCoroutine(this.TriggerFight(1));
    }
    #endregion

    #region Trigger
    public IEnumerator TriggerDialog(float timeOffset)
    {
        //Set Player
        this.playerCharacter.inputControls.lookDirection = 1;
        this.playerCharacter.updateCharacterGraphic_Node.Evaluate();

        //Set Enemy
        this.enemyCharacter.inputControls.lookDirection = -1;
        this.enemyCharacter.updateCharacterGraphic_Node.Evaluate();

        yield return new WaitForSeconds(timeOffset);

        //Construct Dialog
        this.UpdateStatus();
        this.displayDialog.currentDialogue = this.dialogConstructer.ConstructDialogue(true);
        this.displayDialog.DisplayDialogue(this.displayDialog.currentDialogue);
        this.inputManager.SwitchCurrentActionMap("UI");

        this.cameraManager.SwitchToDialog();
        this.enemyCharacter.animator.SetBool("Emotion", true);
    }

    public IEnumerator TriggerFight(float timeOffset)
    {
        this.phaseCounter++;
        this.phase1Count.gameObject.SetActive(this.phaseCounter == 1);
        this.phase2Count.gameObject.SetActive(this.phaseCounter == 2);

        yield return new WaitForSeconds(1);
        this.fightAnimator.SetTrigger("Fight");
        yield return new WaitForSeconds(0.5f);

        yield return new WaitForSeconds(timeOffset);
        this.inputManager.SwitchCurrentActionMap("Player");
        this.cameraManager.SwitchToMultiple();
        this.playerCharacter.isAbleToMove = true;
        this.enemyCharacter.isAbleToMove = true;

        //Reset player stats
        this.playerCharacter.currentHealth = this.playerCharacter.maxHealth;
        this.playerCharacter.healthSlider.fillAmount = 1;

        //Reset enemy stats
        this.enemyCharacter.healthSlider.fillAmount = 1;

        if (this.phaseCounter == 1)
        {
            this.enemyCharacter.maxHealth = this.enemyCharacter.phaseStats.phaseOneHealth;
            this.enemyCharacter.attackReactionTime = this.enemyCharacter.phaseStats.phaseOneReactionTime;
            this.enemyCharacter.currentMeleeWeapon = this.enemyCharacter.phaseStats.phaseOneWeapon;
            this.enemyCharacter.runSpeed = this.enemyCharacter.phaseStats.phaseOneMoveSpeed;

            this.enemyCharacter.attackPositions[0] = this.enemyCharacter.swordTransform;
            this.enemyCharacter.attackPositions[1] = this.enemyCharacter.swordTransform;
            this.enemyCharacter.attackPositions[2] = this.enemyCharacter.swordTransform;

            this.enemyCharacter.animator.SetInteger("WeaponIndex", 0);
            this.enemyCharacter.animator.SetTrigger("SwitchWeapon");
        }

        if (this.phaseCounter == 2)
        {
            this.enemyCharacter.maxHealth = this.enemyCharacter.phaseStats.phaseTwoHealth;
            this.enemyCharacter.attackReactionTime = this.enemyCharacter.phaseStats.phaseTwoReactionTime;
            this.enemyCharacter.currentMeleeWeapon = this.enemyCharacter.phaseStats.phaseTwoWeapon;
            this.enemyCharacter.runSpeed = this.enemyCharacter.phaseStats.phaseTwoMoveSpeed;

            this.enemyCharacter.attackPositions[0] = this.enemyCharacter.hammerTransform;
            this.enemyCharacter.attackPositions[1] = this.enemyCharacter.hammerTransform;
            this.enemyCharacter.attackPositions[2] = this.enemyCharacter.hammerTransform;

            this.enemyCharacter.animator.SetInteger("WeaponIndex", 1);
            this.enemyCharacter.animator.SetTrigger("SwitchWeapon");
        }

        if (this.phaseCounter != 1 && this.phaseCounter != 2)
        {
            Debug.LogError("Enemy is not in phase 1 or 2. Where is he? He's in phase: " + this.phaseCounter);
        }

        this.enemyCharacter.currentHealth = this.enemyCharacter.maxHealth;
        this.healthbarAnimator.SetBool("Fighting", true);

        //Profile
        this.profileManager.SwitchProfile(Profiles.Normal);
        this.profileManager.fighting = true;
    }

    public IEnumerator TriggerTeleport(float timeOffset, Transform destination)
    {
        yield return new WaitForSeconds(timeOffset);

        this.tpManager.TeleportPlayer(destination.position);
    }
    #endregion

    #region Side Functions
    public void ChangeButtonPrompts(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }
        InputDevice device = context.control.device;

        if (device.displayName == this.currentInputDevice)
        {
            return;
        }

        this.currentInputDevice = device.displayName;

        //Switch to Keyboard
        if (device.displayName == "Keyboard" || device.displayName == "Mouse")
        {
            this.fightUI.keyBoard.gameObject.SetActive(true);
            this.fightUI.xBox.gameObject.SetActive(false);
            this.fightUI.playStation.gameObject.SetActive(false);
            this.fightUI.nintendo.gameObject.SetActive(false);
            this.fightUI.neutral.gameObject.SetActive(false);

            this.swordUI.keyBoard.gameObject.SetActive(true);
            this.swordUI.xBox.gameObject.SetActive(false);
            this.swordUI.playStation.gameObject.SetActive(false);
            this.swordUI.nintendo.gameObject.SetActive(false);
            this.swordUI.neutral.gameObject.SetActive(false);

            this.hammerUI.keyBoard.gameObject.SetActive(true);
            this.hammerUI.xBox.gameObject.SetActive(false);
            this.hammerUI.playStation.gameObject.SetActive(false);
            this.hammerUI.nintendo.gameObject.SetActive(false);
            this.hammerUI.neutral.gameObject.SetActive(false);
            return;
        }

        //Switch to xBox
        if (device.displayName == "Xbox Controller")
        {
            this.fightUI.keyBoard.gameObject.SetActive(false);
            this.fightUI.xBox.gameObject.SetActive(true);
            this.fightUI.playStation.gameObject.SetActive(false);
            this.fightUI.nintendo.gameObject.SetActive(false);
            this.fightUI.neutral.gameObject.SetActive(false);

            this.swordUI.keyBoard.gameObject.SetActive(false);
            this.swordUI.xBox.gameObject.SetActive(true);
            this.swordUI.playStation.gameObject.SetActive(false);
            this.swordUI.nintendo.gameObject.SetActive(false);
            this.swordUI.neutral.gameObject.SetActive(false);

            this.hammerUI.keyBoard.gameObject.SetActive(false);
            this.hammerUI.xBox.gameObject.SetActive(true);
            this.hammerUI.playStation.gameObject.SetActive(false);
            this.hammerUI.nintendo.gameObject.SetActive(false);
            this.hammerUI.neutral.gameObject.SetActive(false);
            return;
        }

        //Switch to Playstation
        if (device.displayName == "PlayStation Controller" || device.displayName == "Wireless Controller")
        {
            this.fightUI.keyBoard.gameObject.SetActive(false);
            this.fightUI.xBox.gameObject.SetActive(false);
            this.fightUI.playStation.gameObject.SetActive(true);
            this.fightUI.nintendo.gameObject.SetActive(false);
            this.fightUI.neutral.gameObject.SetActive(false);

            this.swordUI.keyBoard.gameObject.SetActive(false);
            this.swordUI.xBox.gameObject.SetActive(false);
            this.swordUI.playStation.gameObject.SetActive(true);
            this.swordUI.nintendo.gameObject.SetActive(false);
            this.swordUI.neutral.gameObject.SetActive(false);

            this.hammerUI.keyBoard.gameObject.SetActive(false);
            this.hammerUI.xBox.gameObject.SetActive(false);
            this.hammerUI.playStation.gameObject.SetActive(true);
            this.hammerUI.nintendo.gameObject.SetActive(false);
            this.hammerUI.neutral.gameObject.SetActive(false);
            return;
        }

        //Switch to "Switch"
        if (device.displayName == "Switch Controller" || device.displayName == "Pro Controller")
        {
            this.fightUI.keyBoard.gameObject.SetActive(false);
            this.fightUI.xBox.gameObject.SetActive(false);
            this.fightUI.playStation.gameObject.SetActive(false);
            this.fightUI.nintendo.gameObject.SetActive(true);
            this.fightUI.neutral.gameObject.SetActive(false);

            this.swordUI.keyBoard.gameObject.SetActive(false);
            this.swordUI.xBox.gameObject.SetActive(false);
            this.swordUI.playStation.gameObject.SetActive(false);
            this.swordUI.nintendo.gameObject.SetActive(true);
            this.swordUI.neutral.gameObject.SetActive(false);

            this.hammerUI.keyBoard.gameObject.SetActive(false);
            this.hammerUI.xBox.gameObject.SetActive(false);
            this.hammerUI.playStation.gameObject.SetActive(false);
            this.hammerUI.nintendo.gameObject.SetActive(true);
            this.hammerUI.neutral.gameObject.SetActive(false);
            return;
        }

        //Switch to neutral icons
        this.fightUI.keyBoard.gameObject.SetActive(false);
        this.fightUI.xBox.gameObject.SetActive(false);
        this.fightUI.playStation.gameObject.SetActive(false);
        this.fightUI.nintendo.gameObject.SetActive(false);
        this.fightUI.neutral.gameObject.SetActive(true);

        this.swordUI.keyBoard.gameObject.SetActive(false);
        this.swordUI.xBox.gameObject.SetActive(false);
        this.swordUI.playStation.gameObject.SetActive(false);
        this.swordUI.nintendo.gameObject.SetActive(false);
        this.swordUI.neutral.gameObject.SetActive(true);

        this.hammerUI.keyBoard.gameObject.SetActive(false);
        this.hammerUI.xBox.gameObject.SetActive(false);
        this.hammerUI.playStation.gameObject.SetActive(false);
        this.hammerUI.nintendo.gameObject.SetActive(false);
        this.hammerUI.neutral.gameObject.SetActive(true);
    }

    public void SwitchUI(Transform uiTransform, Vector2 uiSize, ButtonPrompts ui, ref bool inRange)
    {
        if (Physics2D.OverlapBox(uiTransform.position, uiSize, 0, this.playerMask))
        {
            if (inRange)
            {
                this.timer += Time.deltaTime * this.lerpSpeed;

                ui.keyBoard.color = new Color(ui.keyBoard.color.r, ui.keyBoard.color.g, ui.keyBoard.color.b, Mathf.Lerp(ui.keyBoard.color.a, 1, this.timer));
                ui.xBox.color = new Color(ui.xBox.color.r, ui.xBox.color.g, ui.xBox.color.b, Mathf.Lerp(ui.xBox.color.a, 1, this.timer));
                ui.playStation.color = new Color(ui.playStation.color.r, ui.playStation.color.g, ui.playStation.color.b, Mathf.Lerp(ui.playStation.color.a, 1, this.timer));
                ui.nintendo.color = new Color(ui.playStation.color.r, ui.playStation.color.g, ui.playStation.color.b, Mathf.Lerp(ui.playStation.color.a, 1, this.timer));
                ui.neutral.color = new Color(ui.neutral.color.r, ui.neutral.color.g, ui.neutral.color.b, Mathf.Lerp(ui.neutral.color.a, 1, this.timer));
                return;
            }

            inRange = true;
            this.timer = 0;
            return;
        }

        if (!inRange)
        {
            this.timer += Time.deltaTime * this.lerpSpeed;
            ui.keyBoard.color = new Color(ui.keyBoard.color.r, ui.keyBoard.color.g, ui.keyBoard.color.b, Mathf.Lerp(ui.keyBoard.color.a, 0, this.timer));
            ui.xBox.color = new Color(ui.xBox.color.r, ui.xBox.color.g, ui.xBox.color.b, Mathf.Lerp(ui.xBox.color.a, 0, this.timer));
            ui.playStation.color = new Color(ui.playStation.color.r, ui.playStation.color.g, ui.playStation.color.b, Mathf.Lerp(ui.playStation.color.a, 0, this.timer));
            ui.nintendo.color = new Color(ui.playStation.color.r, ui.playStation.color.g, ui.playStation.color.b, Mathf.Lerp(ui.playStation.color.a, 0, this.timer));
            ui.neutral.color = new Color(ui.neutral.color.r, ui.neutral.color.g, ui.neutral.color.b, Mathf.Lerp(ui.neutral.color.a, 0, this.timer));
            return;
        }

        inRange = false;
        this.timer = 0;
    }

    [HideInInspector] public int playerParriesPerPhase;
    [HideInInspector] public int playerJumpsPerPhase;
    [HideInInspector] public int playerDashesPerPhase;
    [HideInInspector] public int playerWalljumpsPerPhase;

    [HideInInspector] public int longestAirCombo;

    [HideInInspector] public float phase1DurationRealtime;
    [HideInInspector] public float phase2DurationRealtime;


    private void UpdateStatus()
    {
        this.statusManager.phaseAtEndOfBattle = this.phaseCounter + 1;

        this.statusManager.enemy_HealthAtEndOfBattle_InPercent = this.enemyCharacter.currentHealth < 0 ? 0 : this.enemyCharacter.currentHealth / this.enemyCharacter.maxHealth * 100;
        this.statusManager.player_HealthAtEndOfBattle_InPercent = this.playerCharacter.currentHealth < 0 ? 0 : this.playerCharacter.currentHealth / this.playerCharacter.maxHealth * 100;

        this.statusManager.playerParriesPerPhase = this.playerParriesPerPhase;
        this.statusManager.playerJumpsPerPhase = this.playerJumpsPerPhase;
        this.statusManager.playerDashesPerPhase = this.playerDashesPerPhase;
        this.statusManager.playerWalljumpsPerPhase = this.playerWalljumpsPerPhase;

        this.statusManager.enemy_UsedWeapon = this.enemyCharacter.currentMeleeWeapon.weaponIndex;
        this.statusManager.player_UsedWeapon = this.playerCharacter.currentMeleeWeapon.weaponIndex;
        this.statusManager.currentAirCombo = this.longestAirCombo;

        this.statusManager.phase1DurationRealtime = this.phase1DurationRealtime;
        this.statusManager.phase2DurationRealtime = this.phase2DurationRealtime;

        this.statusManager.SaveStats();
    }

    public void UpdateEnemyEmotionAnimation()
    {
        float biggestEmotionCount = EmotionManager.TempNeutral;
        int currentEmotionIndex = 1;

        if (biggestEmotionCount < EmotionManager.tempAnger)
        {
            biggestEmotionCount = EmotionManager.tempAnger;
            currentEmotionIndex = 2;
        }

        if (biggestEmotionCount < EmotionManager.tempArrogance)
        {
            biggestEmotionCount = EmotionManager.tempArrogance;
            currentEmotionIndex = 3;
        }

        if (biggestEmotionCount < EmotionManager.tempElegance)
        {
            biggestEmotionCount = EmotionManager.tempElegance;
            currentEmotionIndex = 4;
        }

        if (biggestEmotionCount < EmotionManager.tempFear)
        {
            currentEmotionIndex = 5;
        }

        this.enemyCharacter.animator.SetInteger("EmotionIndex", currentEmotionIndex);
    }

    public void PlayEmotionAction()
    {
        int randomAction = Random.Range(1, 4);
        this.enemyCharacter.animator.SetInteger("ActionIndex", randomAction);
        this.enemyCharacter.animator.SetTrigger("Action");
    }
    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(this.transform.position, this.fightSize);

        if (this.swordTransform != null && this.hammerTransform != null)
        {
            Gizmos.DrawWireCube(this.swordTransform.position, this.swordSize);
            Gizmos.DrawWireCube(this.hammerTransform.position, this.hammerSize);
        }
    }
}
