using UnityEngine;

[CreateAssetMenu]
public class MeleeWeapon : ScriptableObject
{
    [Header("Attack Stats")]
    public int weaponIndex;
    public int comboAmount;
    public float superArmour;

    [Space(12f)]
    public AttackStats attackUp;
    [Tooltip("The amount of percent of the players movespeed, the player gets when he attacks up")]
    [Range(0, 1)]
    public float attackUpXSpeedPercent;
    [Tooltip("The amount of percent of the players movespeed, the player needs to be able move on x-Axis during an attack up")]
    [Range(0, 1)]
    public float attackUpXNeededSpeedPercent;
    public AttackStats attackFront;
    public AttackStats attackDown;

    [Space(12f)]
    public AttackStats attackHeavyUp;
    public AttackStats attackHeavyFront;
    public AttackStats attackHeavyDown;

    [Space(12f)]
    [Range(0, 1)]
    [Tooltip("Anticipation in % from Right to Left")]
    public float inputqueueAnticipationPercentage;
    [Range(0, 1)]
    [Tooltip("Recovery in % from Left to Right")]
    public float inputqueueRecoveryPercentage;

    [Header("Parry Stats")]
    public float parryAnticipationTime;
    public float parryStrikeTime;
    public float parryRecoveryTime;

    [Space(12f)]
    [Range(0, 1)]
    public float inputQueueParryPercentage;
    public float parryRadius;
    public float stunTime;
}

public enum WeaponState
{
    Idle,
    Anticipation,
    Strike,
    Recovery
}

[System.Serializable]
public struct AttackStats
{
    public float anticipationTime;
    public float strikeTime;
    public float recoveryTime;

    [Space(12)]
    [Tooltip("from right to left")]
    [Range(0, 1)]
    public float inputqueueTurn;

    [Space(12)]
    public float damage;
    public Vector2 knockBack;
    public float stunTime;
    public bool weightless;

    [Space(12)]
    public Vector2 hitVelocity;

    [Space(12)]
    public float attackLength;
    public float attackForce;
    public float attackAccelerationY;
    public float attackAccelerationX;
    public float attackGravity;

    [Space(12)]
    public Vector3 size;
}
