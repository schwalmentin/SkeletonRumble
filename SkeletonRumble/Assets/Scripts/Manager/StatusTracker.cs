using System.IO;
using UnityEngine;

public class StatusTracker : MonoBehaviour
{
    public static bool writeLog = false;

    // TRACKED FIGHT RELATED STATS
    //Progress
    [HideInInspector] public static int timesTried = 0;
    [HideInInspector] public static int furthestPhaseReached = 1;
    [HideInInspector] public static bool enteredAPhaseForTheFirstTime = false;
    [HideInInspector] public static int phaseAtEndOfBattle = 1;
    [HideInInspector] public static bool firstDialog;

    //Health
    [HideInInspector] public static float enemy_HealthAtEndOfBattle_InPercent = 100;
    [HideInInspector] public static float player_HealthAtEndOfBattle_InPercent = 100;

    //Movement
    [HideInInspector] public static int playerParriesPerPhase;
    [HideInInspector] public static int playerJumpsPerPhase;
    [HideInInspector] public static int playerDashesPerPhase;
    [HideInInspector] public static int playerWalljumpsPerPhase;

    //Combat
    [HideInInspector] public static int enemy_UsedWeapon;
    [HideInInspector] public static int player_UsedWeapon;
    [HideInInspector] public static int longestAirCombo = 6;
    [HideInInspector] public static int currentAirCombo = 0;

    //Time
    [HideInInspector] public static float phase1DurationRealtime;
    [HideInInspector] public static float phase2DurationRealtime;
    [HideInInspector] public static float fightDurationRealtime;


    public static void UpdateStats_EndOfBattle(float Enemy_HealthAtEndOfBattle_InPercent, int PhaseAtEndOfBattle, float Player_HealthAtEndOfBattle_InPercent,   //Health
                            int ParriesPerPhase, int JumpsPerPhase, int DashesPerPhase, int WalljumpsPerPhase,                                                  //Movement
                            int Enemy_UsedWeapon, int Player_UsedWeapon, int CurrentAirCombo,                                                                   //Combat
                            float Phase1DurationRealtime, float Phase2DurationRealtime)                                                                         //Time
    {
        //Progress
        phaseAtEndOfBattle = PhaseAtEndOfBattle;

        if (phaseAtEndOfBattle > furthestPhaseReached)
        {
            furthestPhaseReached = phaseAtEndOfBattle;
            enteredAPhaseForTheFirstTime = true;
        }
        else { enteredAPhaseForTheFirstTime = false; }

        //Health
        enemy_HealthAtEndOfBattle_InPercent = Enemy_HealthAtEndOfBattle_InPercent;
        player_HealthAtEndOfBattle_InPercent = Player_HealthAtEndOfBattle_InPercent;

        if (player_HealthAtEndOfBattle_InPercent <= 0) { timesTried++; }

        //Movement
        playerParriesPerPhase = ParriesPerPhase;
        playerJumpsPerPhase = JumpsPerPhase;
        playerDashesPerPhase = DashesPerPhase;
        playerWalljumpsPerPhase = WalljumpsPerPhase;

        //Combat
        enemy_UsedWeapon = Enemy_UsedWeapon;
        player_UsedWeapon = Player_UsedWeapon;
        currentAirCombo = CurrentAirCombo;
        if (currentAirCombo > longestAirCombo) { longestAirCombo = currentAirCombo; }

        //Time
        phase1DurationRealtime = Phase1DurationRealtime;
        phase2DurationRealtime = Phase2DurationRealtime;

        fightDurationRealtime = phase1DurationRealtime + phase2DurationRealtime;

        //Other

        if (writeLog)
        {
            WriteFightLog();
        }
    }

    private static void WriteFightLog()
    {
        if (!System.IO.Directory.Exists(Application.dataPath + "/Logs/FigthLogs_" + System.DateTime.Now.Day + "_" + System.DateTime.Now.Month + "_" + System.DateTime.Now.Year))
        {
            print("Folder FigthLogs_" + System.DateTime.Now.Day + "_" + System.DateTime.Now.Month + "_" + System.DateTime.Now.Year + " Created: " + Application.dataPath + "/Logs/FigthLogs_" + System.DateTime.Now.Day + "_" + System.DateTime.Now.Month + "_" + System.DateTime.Now.Year);
            System.IO.Directory.CreateDirectory(Application.dataPath + "/Logs/FigthLogs_" + System.DateTime.Now.Day + "_" + System.DateTime.Now.Month + "_" + System.DateTime.Now.Year);
        }

        int i = 0;
        while (File.Exists(Application.dataPath + "/Logs/FigthLogs_" + System.DateTime.Now.Day + "_" + System.DateTime.Now.Month + "_" + System.DateTime.Now.Year + "/FightLog" + i + ".txt"))
        {
            i++;
        }

        print(Application.dataPath + "/Logs/FigthLogs_" + System.DateTime.Now.Day + "_" + System.DateTime.Now.Month + "_" + System.DateTime.Now.Year + "/FightLog" + i + ".txt");
        StreamWriter streamWriter = new StreamWriter(Application.dataPath + "/Logs/FigthLogs_" + System.DateTime.Now.Day + "_" + System.DateTime.Now.Month + "_" + System.DateTime.Now.Year + "/FightLog" + i + ".txt");

        //Health
        streamWriter.WriteLine("Enemy_Health_At_End_Of_Battle: " + enemy_HealthAtEndOfBattle_InPercent);
        streamWriter.WriteLine("Enemy_Phase_At_End_Of_Battle: " + phaseAtEndOfBattle);
        streamWriter.WriteLine("Player_Health_At_End_Of_Battle: " + player_HealthAtEndOfBattle_InPercent);

        //Movement
        streamWriter.WriteLine("Parries_Per_Phase: " + playerParriesPerPhase);
        streamWriter.WriteLine("Jumps_Per_Phase: " + playerJumpsPerPhase);
        streamWriter.WriteLine("Dashes_Per_Phase: " + playerDashesPerPhase);
        streamWriter.WriteLine("Walljumps_Per_Phase: " + playerWalljumpsPerPhase);

        //Combat
        //streamWriter.WriteLine("Enemy_Used_Weapon: " + enemy_UsedWeapon);
        //streamWriter.WriteLine("Player_Used_Weapon: " + player_UsedWeapon);
        streamWriter.WriteLine("Longest_Air_Combo: " + longestAirCombo);

        //Time
        streamWriter.WriteLine("Phase_1_Duration_Realtime: " + phase1DurationRealtime);
        streamWriter.WriteLine("Phase_2_Duration_Realtime: " + phase2DurationRealtime);
        streamWriter.WriteLine("Fight_Duration_Realtime: " + fightDurationRealtime);

        streamWriter.Close();
    }
}