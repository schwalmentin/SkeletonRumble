using UnityEngine;
using UnityEngine.SceneManagement;
public class Test_Stats : MonoBehaviour
{
    //void Awake()
    //{
    //    Test_RandomDialogue();
    //}

    public void QuickRestart(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    public void Test_RandomDialogue()
    {
        StatusTracker.UpdateStats_EndOfBattle(Random.Range(0, 100), Random.Range(1, 3), Random.Range(0, 100),
                                        Random.Range(0, 300), Random.Range(0, 300), Random.Range(0, 300),
                                        Random.Range(0, 300), Random.Range(0, 2), Random.Range(0, 2),
                                        Random.Range(0, 15), Random.Range(100, 1000), Random.Range(100, 1000));
    }

    public void Test_AbilityDialog()
    {
        StatusTracker.UpdateStats_EndOfBattle(50, 1, 1,
                                        300, 300, 300,
                                        300, 1, 1,
                                        0, 1, 1);
    }

    public void Test_DoneNothing()
    {
        StatusTracker.UpdateStats_EndOfBattle(50, 1, 1,
                                        0, 0, 0, 0,
                                        1, 1,
                                        0, 1, 1);
    }

    public void Test_PlayerDead()
    {
        StatusTracker.UpdateStats_EndOfBattle(Random.Range(0, 100), Random.Range(1, 3), 0,
                                        Random.Range(0, 200), Random.Range(0, 200), Random.Range(0, 200),
                                        Random.Range(0, 200), Random.Range(0, 2), Random.Range(0, 2),
                                        Random.Range(0, 7), Random.Range(100, 1000), Random.Range(100, 1000));
    }

    public void Test_EnemyDead()
    {
        StatusTracker.UpdateStats_EndOfBattle(0, 1, Random.Range(1, 50),
                                        0, 0, 0, 0,
                                        1, 1,
                                        0, 1, 1);
    }

    public static void PrintEmotions(string additionalInfo = "")
    {
        print("Fea: " + EmotionManager.Fear +
              " | Ang: " + EmotionManager.Anger +
              " | Aro: " + EmotionManager.Arrogance +
              " | Ele: " + EmotionManager.Elegance +
              " | Neu: " + EmotionManager.Neutral +
              " | TempFea: " + EmotionManager.tempFear +
              " | TempAng: " + EmotionManager.tempAnger +
              " | TempAro: " + EmotionManager.tempArrogance +
              " | TempEle: " + EmotionManager.tempElegance +
              additionalInfo);
    }
}
