using System.Collections.Generic;
using System.Globalization;
using System.IO;
using TMPro;
using UnityEngine;

public class Construct_Dialogue : MonoBehaviour
{
    public StatusTracker tracker;
    public string currentCharacterName = "RandomlyGeneratedCharacter";
    public string currentLanguage = "Englisch";
    private enum Priority { TopPriority, High, Medium, Low, NoPriority } //TopPriority = Needs to be said NOW!  |  High = Should be said Now  |  Medium = Can be said now  |  Low = Should only be said if nothing else is there to be said

    public int answerAmount;
    public string commandToStartCombat = "*";
    private string currentTopic;
    private int currentFileNumber;
    private int currentSubTopic;

    public TMP_Text debugFileDisplay;

    public List<Dialogue.EmotionalChange> emotionalChanges = new List<Dialogue.EmotionalChange>();

    [Header("Dialog Trigger Conditiones")]
    public int almostNewHighscore_Threshold = 3;
    
    public int dashedManyTimes_Threshold = 150;
    public int jumpedManyTimes_Threshold = 250;
    public int parriedManyTimes_Threshold = 100;
    
    public int fightCloseness_Close_remainingPercentThreshold = 10;
    
    public int swordID = 0;
    public int hammerID = 1;

    public Dialogue ConstructDialogue(bool newDialoge)
    {
        if (newDialoge)
        {
            this.currentTopic = this.ReturnFittingTopic();
            this.currentFileNumber = 0;

            int numberOfSubTopics = 0;
            while (Directory.Exists(Application.dataPath + "/Dialogue/" + this.currentLanguage + "/" + this.currentCharacterName + "/" + EmotionManager.GetEmotion() + "/" + this.currentTopic + "/Topic" + (numberOfSubTopics + 1)))
            {
                numberOfSubTopics++;
            }

            if (numberOfSubTopics != 0)
            {
                this.currentSubTopic = Random.Range(1, numberOfSubTopics + 1);
            }
            else { Debug.LogError("The File Path '" + Application.dataPath + "/Dialogue/" + this.currentLanguage + "/" + this.currentCharacterName + "/" + EmotionManager.GetEmotion() + "/" + this.currentTopic + "' does not Contain a Topic!"); return null; }
        }

        List<string> textSegments = new List<string>(); //This list represents 1 dialogue with its segments as different entries in the list
        List<string> validAnswers = new List<string>(); //This list represents all the different answers which is possible since the Answers will alwasy use 1 Text Box

        textSegments.AddRange(this.SearchForTopic(this.currentTopic, false));
        validAnswers.AddRange(this.SearchForTopic(this.currentTopic, true));

        return new Dialogue(textSegments.ToArray(), validAnswers.ToArray(), this.emotionalChanges);
    }

    private string ReturnFittingTopic()
    {
        List<string> validTopics = new List<string>();
        Priority hightestPriority = 0; //0 always corresponds to the highest priority possible

        while (validTopics.Count == 0)
        {
            //Player Won
            if (StatusTracker.enemy_HealthAtEndOfBattle_InPercent <= 0 && StatusTracker.phaseAtEndOfBattle == 3)
            {
                //Top Priority
                if (hightestPriority == Priority.TopPriority)
                {
                    if (StatusTracker.timesTried == 2)
                    { validTopics.Add("Dead_Enemy_Try1"); }
                }

                //High Priority
                if (hightestPriority == Priority.High)
                {
                    if (StatusTracker.player_HealthAtEndOfBattle_InPercent == 100)
                    { validTopics.Add("Dead_Enemy_PlayerFlawless"); }
                }

                //No Priority
                if (hightestPriority == Priority.NoPriority)
                { validTopics.Add("Dead_Enemy_PlayerFlawless"); }

                hightestPriority++;
                continue;
            }

            //Enemy Won
            if (StatusTracker.player_HealthAtEndOfBattle_InPercent <= 0)
            {
                //Top Priority
                if (hightestPriority == Priority.TopPriority)
                {
                    if (StatusTracker.timesTried == 1)
                    { validTopics.Add("Dead_Story_FirstEncounterWithEnemy"); }

                    if (StatusTracker.timesTried == 10)
                    { validTopics.Add("Dead_Player_Try10"); }

                    if (StatusTracker.timesTried == 50)
                    { validTopics.Add("Dead_Player_Try50"); }

                    if (StatusTracker.timesTried == 100)
                    { validTopics.Add("Dead_Player_Try100"); }
                }

                //High Priority
                if (hightestPriority == Priority.High)
                {
                    //if (StatusTracker.timesTried == 0)
                    //{ validTopics.Add("Story_FirstEncounterWithEnemy"); }

                    if (StatusTracker.enemy_HealthAtEndOfBattle_InPercent == 100)
                    { validTopics.Add("Dead_Player_FightCloseness_EnemyFlawless"); }

                    if (StatusTracker.enteredAPhaseForTheFirstTime == true && StatusTracker.furthestPhaseReached == 2)
                    { validTopics.Add("Dead_Player_FirstTimePhase2"); }
                }

                //Medium Priority
                if (hightestPriority == Priority.Medium)
                {
                    if (StatusTracker.currentAirCombo >= StatusTracker.longestAirCombo - this.almostNewHighscore_Threshold && StatusTracker.currentAirCombo != StatusTracker.longestAirCombo)
                    { validTopics.Add("Dead_Player_AirCombo_AlmostNewHighscore"); }

                    if (StatusTracker.currentAirCombo == StatusTracker.longestAirCombo)
                    { validTopics.Add("Dead_Player_AirCombo_NewHighscore"); }

                    if (StatusTracker.enemy_HealthAtEndOfBattle_InPercent <= this.fightCloseness_Close_remainingPercentThreshold)
                    { validTopics.Add("Dead_Player_FightCloseness_Close"); }

                    if (StatusTracker.playerParriesPerPhase == 0 && StatusTracker.playerDashesPerPhase == 0)
                    { validTopics.Add("Dead_Player_NeverUsedAbilities"); }
                }

                //Low Priority
                if (hightestPriority == Priority.Low)
                {
                    if (StatusTracker.playerDashesPerPhase >= this.dashedManyTimes_Threshold)
                    { validTopics.Add("Dead_Player_DashedManyTimes"); }

                    if (StatusTracker.playerJumpsPerPhase >= this.jumpedManyTimes_Threshold)
                    { validTopics.Add("Dead_Player_JumpedManyTimes"); }

                    if (StatusTracker.playerParriesPerPhase >= this.parriedManyTimes_Threshold)
                    { validTopics.Add("Dead_Player_ParriedManyTimes"); }
                }

                //No Priority
                if (hightestPriority == Priority.NoPriority)
                {
                    if (StatusTracker.player_UsedWeapon == this.hammerID)
                    { validTopics.Add("Dead_Player_HammerEquiped"); }

                    if (StatusTracker.player_UsedWeapon == this.swordID)
                    { validTopics.Add("Dead_Player_SwordEquiped"); }
                }

                hightestPriority++;
                continue;
            }

            //Between Phases
            #region Between Phases

            //Top Priority
            if (hightestPriority == Priority.TopPriority)
            {
                if (StatusTracker.timesTried == 0 && !StatusTracker.firstDialog)
                { 
                    validTopics.Add("Story_FirstEncounterWithEnemy");
                    StatusTracker.firstDialog = true;
                }

                if (StatusTracker.timesTried == 10)
                { validTopics.Add("Player_Try10"); }

                if (StatusTracker.timesTried == 50)
                { validTopics.Add("Player_Try50"); }

                if (StatusTracker.timesTried == 100)
                { validTopics.Add("Player_Try100"); }
            }

            //High Priority
            if (hightestPriority == Priority.High)
            {
                if (StatusTracker.enteredAPhaseForTheFirstTime == true && StatusTracker.furthestPhaseReached == 2)
                { validTopics.Add("Player_FirstTimePhase2"); }

                if (StatusTracker.playerParriesPerPhase == 0 && StatusTracker.playerDashesPerPhase == 0)
                { validTopics.Add("Player_NeverUsedAbilities"); }
            }

            //Medium Priority
            if (hightestPriority == Priority.Medium)
            {
                if (StatusTracker.currentAirCombo >= StatusTracker.longestAirCombo - this.almostNewHighscore_Threshold && StatusTracker.currentAirCombo != StatusTracker.longestAirCombo)
                { validTopics.Add("Player_AirCombo_AlmostNewHighscore"); }

                if (StatusTracker.currentAirCombo == StatusTracker.longestAirCombo)
                { validTopics.Add("Player_AirCombo_NewHighscore"); }
            }

            //Low Priority
            if (hightestPriority == Priority.Low)
            {
                if (StatusTracker.playerDashesPerPhase >= this.dashedManyTimes_Threshold)
                { validTopics.Add("Player_DashedManyTimes"); }

                if (StatusTracker.playerJumpsPerPhase >= this.jumpedManyTimes_Threshold)
                { validTopics.Add("Player_JumpedManyTimes"); }

                if (StatusTracker.playerParriesPerPhase >= this.parriedManyTimes_Threshold)
                { validTopics.Add("Player_ParriedManyTimes"); }
            }

            //No Priority
            if (hightestPriority == Priority.NoPriority)
            {
                if (StatusTracker.player_UsedWeapon == this.hammerID)
                { validTopics.Add("Player_HammerEquiped"); }

                if (StatusTracker.player_UsedWeapon == this.swordID)
                { validTopics.Add("Player_SwordEquiped"); }
            }
            #endregion;

            hightestPriority++;
        }

        return validTopics[Random.Range(0, validTopics.Count)];
    }
    private List<string> SearchForTopic(string topic, bool isAnswer)
    {
        string validFilePath = "Error: No FilePath Found";

        if (!Directory.Exists(Application.dataPath + "/Dialogue/" + this.currentLanguage + "/" + this.currentCharacterName + "/" + EmotionManager.GetEmotion() + "/" + topic))
        { Debug.LogError("The Topic '" + topic + "' at the Path '" + Application.dataPath + "/Dialogue/" + this.currentLanguage + "/" + this.currentCharacterName + "/" + EmotionManager.GetEmotion() + "' does not exist!"); return null; }

        int numberOfFiles = 1;
        while (File.Exists(Application.dataPath + "/Dialogue/" + this.currentLanguage + "/" + this.currentCharacterName + "/" + EmotionManager.GetEmotion() + "/" + topic + "/Topic" + this.currentSubTopic + "/" + numberOfFiles + ".txt"))
        {
            numberOfFiles++;
        }
        numberOfFiles--;
        if (numberOfFiles != 0)
        {
            this.currentFileNumber++;

            if (this.currentFileNumber - 1 == numberOfFiles)
            {
                List<string> startCombatCommand = new List<string>();
                startCombatCommand.Add(this.commandToStartCombat);
                return startCombatCommand;
            }

            if (numberOfFiles >= this.currentFileNumber)
            {
                validFilePath = Application.dataPath + "/Dialogue/" + this.currentLanguage + "/" + this.currentCharacterName + "/" + EmotionManager.GetEmotion() + "/" + topic + "/Topic" + this.currentSubTopic + "/" + this.currentFileNumber + ".txt";
            }

            if (this.currentFileNumber == 1)
            {
                this.debugFileDisplay.text = Application.dataPath + "/Dialogue/" + this.currentLanguage + "/" + this.currentCharacterName + "/" + EmotionManager.GetEmotion() + "/" + topic + "/Topic" + this.currentSubTopic + "/";
            }
        }
        else { Debug.LogError("The File Path '" + Application.dataPath + "/Dialogue/" + this.currentLanguage + "/" + this.currentCharacterName + "/" + EmotionManager.GetEmotion() + "/" + topic + "/Topic" + this.currentSubTopic + "' does not Contain Files!"); return null; }

        return this.ReadStream(validFilePath, isAnswer);
    }
    private List<string> ReadStream(string FilePath, bool isAnswer)
    {
        List<List<string>> allDialogueLines = new List<List<string>>();

        StreamReader streamReader = new StreamReader(FilePath);

        if (streamReader.Peek() != '\r') { allDialogueLines.Add(new List<string>()); } //account for first line of the text file
        while (!streamReader.EndOfStream)
        {
            if (streamReader.Peek() == '\r')
            {
                allDialogueLines.Add(new List<string>());
                streamReader.ReadLine();
            }
            else
            {
                allDialogueLines[allDialogueLines.Count - 1].Add(streamReader.ReadLine());
            }
        }


        this.emotionalChanges.Clear();
        if (!isAnswer)
        {
            int randomDialogueLine = Random.Range(0, allDialogueLines.Count);

            this.debugFileDisplay.text += randomDialogueLine + 1;
            return allDialogueLines[randomDialogueLine];
        }
        else
        {
            List<string> answers = new List<string>();
            List<List<string>> allDialogueLine_JustForDialogeIndex = new List<List<string>>();
            allDialogueLine_JustForDialogeIndex.AddRange(allDialogueLines);

            for (int i = 0; i < this.answerAmount; i++)
            {
                Dialogue.EmotionalChange change = new Dialogue.EmotionalChange();
                int randomDialogueLine = Random.Range(0, allDialogueLines.Count);

                answers.Add(allDialogueLines[randomDialogueLine][0]); //allDialogueLines[randomDialogueLine][0] should always be the whole answer - if not, the system doesn't work

                for (int z = 0; z < allDialogueLine_JustForDialogeIndex.Count; z++)
                {
                    if (allDialogueLine_JustForDialogeIndex[z] == allDialogueLines[randomDialogueLine])
                    {
                        change.dialogueIndex = z + 1;
                    }
                } //The for loop and the allDialogueLine_JustForDialogeIndex list are just for representing the Dialogue path!


                char[] value = allDialogueLines[randomDialogueLine][1].ToCharArray(); //allDialogueLines[randomDialogueLine][1] should always be the value change for this answer - if not, the system doesn't work

                string emotionalChangeValue = "";
                string emotionalChangeEmotion = "";

                foreach (char character in value)
                {
                    if (character == '+' || character == '-' || character == '0' || character == '1' ||
                       character == '2' || character == '3' || character == '4' || character == '5' ||
                       character == '6' || character == '7' || character == '8' || character == '9')
                    {
                        emotionalChangeValue += character;
                    }
                    else
                    {
                        if (character != ' ') { emotionalChangeEmotion += character; }
                    }
                }

                CultureInfo parseInEnglisch = CultureInfo.CreateSpecificCulture("en-US");   //Make sure Parsing always uses english and not the operating-system language (important for differences like , and .)
                int.TryParse(emotionalChangeValue, NumberStyles.Integer, parseInEnglisch, out change.value);

                if (emotionalChangeEmotion == "Anger") { change.emotion = Dialogue.EmotionalChange.Emotion.Anger; };
                if (emotionalChangeEmotion == "Arrogance") { change.emotion = Dialogue.EmotionalChange.Emotion.Arrogance; };
                if (emotionalChangeEmotion == "Elegance") { change.emotion = Dialogue.EmotionalChange.Emotion.Elegance; };
                if (emotionalChangeEmotion == "Fear") { change.emotion = Dialogue.EmotionalChange.Emotion.Fear; };

                this.emotionalChanges.Add(change);
                allDialogueLines.RemoveAt(randomDialogueLine);
            }

            return answers;
        }
    }


    //Tracked Stats:
    //private GameObject[] playerMainWeaponFromTheLastFights; // The Weapon which is equiped 
    //private GameObject[] playerSecondaryWeaponFromTheLastFights;
    //playerFavouriteMainWeapon
    //playerFavouriteSecondaryWeapon
    //enemyWeapon

    //private void Construct_Premisse()
    //{
    //    if (true)
    //    {

    //    }
    //    //if Gleiche Waffe wie letztes mal
    //    //if (enemyWeapon == playerWeaponsFromTheLastFights[0]) Gleiche Waffen
    //    //if (playerWeaponsFromTheLastFights[0 - 5] == playerFavouriteWeapon) Noch immer lieblings Waffe
    //    //if () Kommentar zur Lieblingswaffe
    //    //if () Kommentar wenn sich die Lieblingswaffe gewechselt hat
    //    //if () Kommentar wenn der Spieler plötzlich nicht mehr seine Lieblingswaffe benutzt
    //    //if () Kommentar wenn der Spieler wieder zurück zu seiner lieblingswaffe wechselt
    //    //if () 
    //    //if ()
    //    //if ()
    //}
}