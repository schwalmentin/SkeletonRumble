using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveStatusValues : MonoBehaviour
{
    //Progress
    [HideInInspector] public int phaseAtEndOfBattle;

    //Health
    [HideInInspector] public float enemy_HealthAtEndOfBattle_InPercent = 100;
    [HideInInspector] public float player_HealthAtEndOfBattle_InPercent = 100;

    //Movement
    [HideInInspector] public int playerParriesPerPhase;
    [HideInInspector] public int playerJumpsPerPhase;
    [HideInInspector] public int playerDashesPerPhase;
    [HideInInspector] public int playerWalljumpsPerPhase;

    //Combat
    [HideInInspector] public int enemy_UsedWeapon;
    [HideInInspector] public int player_UsedWeapon;
    [HideInInspector] public int currentAirCombo = 0;

    //Time
    [HideInInspector] public float phase1DurationRealtime;
    [HideInInspector] public float phase2DurationRealtime;

    public void SaveStats()
    {
        StatusTracker.UpdateStats_EndOfBattle(
            this.enemy_HealthAtEndOfBattle_InPercent,
            this.phaseAtEndOfBattle,
            this.player_HealthAtEndOfBattle_InPercent,
            this.playerParriesPerPhase,
            this.playerJumpsPerPhase,
            this.playerDashesPerPhase,
            this.playerWalljumpsPerPhase,
            this.enemy_UsedWeapon,
            this.player_UsedWeapon,
            this.currentAirCombo,
            this.phase1DurationRealtime,
            this.phase2DurationRealtime);

        print("EnemyHealth: " + this.enemy_HealthAtEndOfBattle_InPercent);
        print("Phase: " + this.phaseAtEndOfBattle);
        print("PlayerHealth: " + this.player_HealthAtEndOfBattle_InPercent);
        print("Parrys: " + this.playerParriesPerPhase);
        print("Jumps: " + this.playerJumpsPerPhase);
        print("Dashes: " + this.playerDashesPerPhase);
        print("Walljumps: " + this.playerWalljumpsPerPhase);
        print("EnemyWeapon: " + this.enemy_UsedWeapon);
        print("PlayerWeapon: " + this.player_UsedWeapon);
        print("CurrentAirCombo: " + this.currentAirCombo);
        print("Phase1Duration: " + this.phase1DurationRealtime);
        print("Phase2Durátion: " + this.phase2DurationRealtime);
    }
}
