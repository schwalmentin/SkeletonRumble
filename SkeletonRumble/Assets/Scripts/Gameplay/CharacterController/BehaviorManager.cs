using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(EnemyController))]
public class BehaviorManager : MonoBehaviour
{
    #region Manager

    [Header("Manager")]
    public float defaultClockMin;
    public float defaultClockMax;
    [Space]
    public float chargeClockMin;
    public float chargeClockMax;
    [Space]
    public float fleeClockMin;
    public float fleeClockMax;
    [Space]
    public float duellClockMin;
    public float duellClockMax;
    [Space]
    public float attackClockMin;
    public float attackClockMax;

    [HideInInspector] public float clockTimer;
    private EnemyController enemyController;
    private PercentageCFDA finalPercentageCFDA;

    #endregion

    #region Variables

    [Header("Variables")]
    public int statsWeight;
    public int emotionWeight;
    [Space]
    [Tooltip("From min to max, you must asign the weight of Element 0")] public PercentageCFDA[] distanceP;
    [Tooltip("From min to max, you must asign the weight of Element 0")] public PercentageCFDA[] enemyHealthP;
    [Tooltip("From min to max, you must asign the weight of Element 0")] public PercentageCFDA[] enemyWeaponP;
    [Tooltip("From min to max, you must asign the weight of Element 0")] public PercentageCFDA[] enemyPhaseP;
    [Tooltip("From min to max, you must asign the weight of Element 0")] public PercentageCFDA[] playerHealthP;
    [Tooltip("From min to max, you must asign the weight of Element 0")] public PercentageCFDA[] playerWeaponP;
    [Space]
    public PercentageCFDA neutralEmotionP;
    public PercentageCFDA angerEmotionP;
    public PercentageCFDA arroganceEmotionP;
    public PercentageCFDA eleganceEmotionP;
    public PercentageCFDA fearEmotionP;

    private List<float> distanceV;
    private List<float> enemyHealthV;
    private List<float> enemyWeaponV;
    private List<float> enemyPhaseV;
    private List<float> playerHealthV;
    private List<float> playerWeaponV;

    private PercentageCFDA distance;
    private PercentageCFDA enemyHealth;
    private PercentageCFDA enemyWeapon;
    private PercentageCFDA enemyPhase;
    private PercentageCFDA playerHealth;
    private PercentageCFDA playerWeapon;
    private PercentageCFDA emotions;

    private PercentageCFDA[] allPercentageCDFAs;
    private InputValues inputValues;
    #endregion

    public TextMeshProUGUI state;

    private void Start()
    {
        this.enemyController = this.GetComponent<EnemyController>();

        //Declare all percentageCDFAs
        this.allPercentageCDFAs = new PercentageCFDA[] { this.distance, this.enemyHealth, this.enemyWeapon, this.enemyPhase, this.playerHealth, this.playerWeapon };

        //Calculate all Values -
        this.CalculateValues(this.distanceP, ref this.distanceV);
        this.CalculateValues(this.enemyHealthP, ref this.enemyHealthV);
        this.CalculateValues(this.enemyWeaponP, ref this.enemyWeaponV);
        this.CalculateValues(this.enemyPhaseP, ref this.enemyPhaseV);
        this.CalculateValues(this.playerHealthP, ref this.playerHealthV);
        this.CalculateValues(this.playerWeaponP, ref this.playerWeaponV);

        #region Test
        ////Test
        //this.SwitchState();

        //PercentageCFDA p = this.CalculateAveragePercentage(new PercentageCFDA[] { this.distance, this.enemyHealth }, 1);
        //print("New CFDA: " + p.pCharge + "; " + p.pFlee + "; " + p.pDuell + "; " + p.pAttack);
        #endregion
    }

    private void Update()
    {
        if (!this.enemyController.isAbleToMove)
        {
            return;
        }

        switch (this.enemyController.enemyStates)
        {
            case EnemyStates.Charge:
                this.state.text = "Charge";
                break;
            case EnemyStates.Flee:
                this.state.text = "Flee";
                break;
            case EnemyStates.Duell:
                this.state.text = "Duell";
                break;
            case EnemyStates.Attack:
                this.state.text = "Attack";
                break;
        }

        this.clockTimer -= Time.deltaTime;

        if (this.clockTimer <= 0)
        {
            if (this.enemyController.enemyStates == EnemyStates.Attack && this.enemyController.attackStates != AttackStates.Move)
            {
                return;
            }
            this.SwitchState();
        }
    }

    private void SwitchState()
    {
        //Update Input Values
        this.UpdateInputValues();

        //Calculate CFDA percentages for each variable
        this.distance = this.CalculatePercentage(this.distance, this.distanceP, this.distanceV, this.inputValues.distanceInput);
        this.enemyHealth = this.CalculatePercentage(this.enemyHealth, this.enemyHealthP, this.enemyHealthV, this.inputValues.enemyHealthInput);
        this.enemyWeapon = this.CalculatePercentage(this.enemyWeapon, this.enemyWeaponP, this.enemyWeaponV, this.inputValues.enemyWeaponInput);
        this.enemyPhase = this.CalculatePercentage(this.enemyPhase, this.enemyPhaseP, this.enemyPhaseV, this.inputValues.enemyPhaseInput);
        this.playerHealth = this.CalculatePercentage(this.playerHealth, this.playerHealthP, this.playerHealthV, this.inputValues.playerHealthInput);
        this.playerWeapon = this.CalculatePercentage(this.playerWeapon, this.playerWeaponP, this.playerWeaponV, this.inputValues.playerWeaponInput);

        //Calculate CDFA percentages for emotion variable
        #region Calculate emotions
        this.emotions = new PercentageCFDA();

        this.emotions.pCharge = this.neutralEmotionP.pCharge * EmotionManager.Neutral / 100 +
            this.angerEmotionP.pCharge * EmotionManager.Anger / 100 +
            this.arroganceEmotionP.pCharge * EmotionManager.Arrogance / 100 +
            this.eleganceEmotionP.pCharge * EmotionManager.Elegance / 100 +
            this.fearEmotionP.pCharge * EmotionManager.Fear / 100;

        this.emotions.pFlee = this.neutralEmotionP.pFlee * EmotionManager.Neutral / 100 +
            this.angerEmotionP.pFlee * EmotionManager.Anger / 100 +
            this.arroganceEmotionP.pFlee * EmotionManager.Arrogance / 100 +
            this.eleganceEmotionP.pFlee * EmotionManager.Elegance / 100 +
            this.fearEmotionP.pFlee * EmotionManager.Fear / 100;

        this.emotions.pDuell = this.neutralEmotionP.pDuell * EmotionManager.Neutral / 100 +
            this.angerEmotionP.pDuell * EmotionManager.Anger / 100 +
            this.arroganceEmotionP.pDuell * EmotionManager.Arrogance / 100 +
            this.eleganceEmotionP.pDuell * EmotionManager.Elegance / 100 +
            this.fearEmotionP.pDuell * EmotionManager.Fear / 100;

        this.emotions.pAttack = this.neutralEmotionP.pAttack * EmotionManager.Neutral / 100 +
            this.angerEmotionP.pAttack * EmotionManager.Anger / 100 +
            this.arroganceEmotionP.pAttack * EmotionManager.Arrogance / 100 +
            this.eleganceEmotionP.pAttack * EmotionManager.Elegance / 100 +
            this.fearEmotionP.pAttack * EmotionManager.Fear / 100;

        this.emotions.weight = this.emotionWeight;
        #endregion

        //UpdateAllPercentage
        this.allPercentageCDFAs = new PercentageCFDA[] { this.distance, this.enemyHealth, this.enemyWeapon, this.enemyPhase, this.playerHealth, this.playerWeapon };

        //Calculate final percentage for CFDA
        PercentageCFDA[] percentageCFDAWithEmotions = new PercentageCFDA[] { this.CalculateAveragePercentage(this.allPercentageCDFAs, this.statsWeight), this.emotions };
        this.finalPercentageCFDA = this.CalculateAveragePercentage(percentageCFDAWithEmotions, 1);
        //this.finalPercentageCFDA = this.CalculateAveragePercentage(this.allPercentageCDFAs, 1);

        //Pick winning enemy state
        float charge = this.finalPercentageCFDA.pCharge;
        float flee = charge + this.finalPercentageCFDA.pFlee;
        float duell = flee + this.finalPercentageCFDA.pDuell;
        float attack = duell + this.finalPercentageCFDA.pAttack;

        float randomNumber = Random.Range(0f, 100f);
        //print("Charge: " + charge + "   Flee: " + flee + "  Duell: " + duell + "    Attack: " + attack + "                        " + randomNumber);

        if (randomNumber <= charge)
        {
            this.enemyController.UpdateEnemyController(EnemyStates.Charge);
            this.clockTimer = Random.Range(this.chargeClockMin, this.chargeClockMax);
            return;
        }

        if (randomNumber <= flee)
        {
            this.enemyController.UpdateEnemyController(EnemyStates.Flee);
            this.clockTimer = Random.Range(this.fleeClockMin, this.fleeClockMax);
            return;
        }

        if (randomNumber <= duell)
        {
            this.enemyController.UpdateEnemyController(EnemyStates.Duell);
            this.clockTimer = Random.Range(this.duellClockMin, this.duellClockMax);
            this.enemyController.recoverOfAttackTimer = this.enemyController.recoverOfAttackTime;
            return;
        }

        if (randomNumber <= attack)
        {
            this.enemyController.UpdateEnemyController(EnemyStates.Attack);
            this.clockTimer = Random.Range(this.attackClockMin, this.attackClockMax);
            return;
        }

        this.clockTimer = Random.Range(this.defaultClockMin, this.defaultClockMax);
    }

    private void CalculateValues(PercentageCFDA[] p, ref List<float> v)
    {
        float total = p.Length;
        float value = 1 / (total - 1);

        v = new List<float>();
        for (int i = 0; i < total; i++)
        {
            v.Add(value * i);
        }
    }

    private PercentageCFDA CalculatePercentage(PercentageCFDA percentageOutCome, PercentageCFDA[] percentageInput, List<float> percentageValues, float inputValue)
    {
        int weight = percentageInput[0].weight;

        float min = 0;
        float max = 0;
        PercentageCFDA first = percentageOutCome;
        PercentageCFDA second = percentageOutCome;

        for (int i = 0; i < percentageValues.Count; i++)
        {
            if (inputValue <= percentageValues[i])
            {
                if (i == 0)
                {
                    percentageOutCome = percentageInput[0];
                    percentageOutCome.weight = weight;
                    return percentageOutCome;
                }

                min = percentageValues[i - 1];
                max = percentageValues[i];

                first = percentageInput[i - 1];
                second = percentageInput[i];

                break;
            }
        }

        float percentageSecond = (inputValue - min) / (max - min) * 100;
        float percentageFirst = (100 - percentageSecond);

        first.weight = (int)percentageFirst;
        second.weight = (int)percentageSecond;

        percentageOutCome = this.CalculateAveragePercentage(new PercentageCFDA[] { first, second }, weight);

        return percentageOutCome;
    }

    private PercentageCFDA CalculateAveragePercentage(PercentageCFDA[] percentageCFDAs, int weightOutcome)
    {
        int weightTotal = 0;

        float cPercentageTotal = 0;
        float fPercentageTotal = 0;
        float dPercentageTotal = 0;
        float aPercentageTotal = 0;

        foreach (PercentageCFDA item in percentageCFDAs)
        {
            weightTotal += item.weight;

            cPercentageTotal += item.pCharge * item.weight;
            fPercentageTotal += item.pFlee * item.weight;
            dPercentageTotal += item.pDuell * item.weight;
            aPercentageTotal += item.pAttack * item.weight;
        }

        PercentageCFDA finalPercentage = new PercentageCFDA
        {
            pCharge = cPercentageTotal / weightTotal,
            pFlee = fPercentageTotal / weightTotal,
            pDuell = dPercentageTotal / weightTotal,
            pAttack = aPercentageTotal / weightTotal,
            weight = weightOutcome
        };

        return finalPercentage;
    }

    [Header("InputValueSettings")]
    public float maxDistance;

    private void UpdateInputValues()
    {
        //DistanceValue
        float distance = Vector2.Distance(this.transform.position, this.enemyController.playerTransform.position);
        this.inputValues.distanceInput = distance / this.maxDistance;
        this.inputValues.distanceInput = this.inputValues.distanceInput > 1 ? 1 : this.inputValues.distanceInput;

        //EnemyPhaseValue
        //---------------

        //EnemyHealthValue
        this.inputValues.enemyHealthInput = this.enemyController.currentHealth / this.enemyController.maxHealth;
        this.inputValues.enemyHealthInput = this.inputValues.enemyHealthInput > 1 ? 1 : this.inputValues.enemyHealthInput;

        //PlayerHealthValue
        this.inputValues.playerHealthInput = this.enemyController.playerCharacter.currentHealth / this.enemyController.playerCharacter.maxHealth;
        this.inputValues.playerHealthInput = this.inputValues.playerHealthInput > 1 ? 1 : this.inputValues.playerHealthInput;

        //EnemyWeaponValue
        this.inputValues.enemyWeaponInput = this.enemyController.currentMeleeWeapon.weaponIndex;

        //PlayerWeaponValue
        this.inputValues.playerWeaponInput = this.enemyController.playerCharacter.currentMeleeWeapon.weaponIndex;
    }

    [System.Serializable]
    public struct PercentageCFDA
    {
        public float pCharge, pFlee, pDuell, pAttack;
        public int weight;
    }

    public struct InputValues
    {
        public float distanceInput;
        public float enemyHealthInput;
        public float enemyWeaponInput;
        public float enemyPhaseInput;
        public float playerHealthInput;
        public float playerWeaponInput;
    }
}
