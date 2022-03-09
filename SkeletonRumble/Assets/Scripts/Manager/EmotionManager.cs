public class EmotionManager
{
    public static int maxEmotion = 100;

    private static int anger;
    private static int arrogance;
    private static int elegance;
    private static int fear;
    private static int neutral;

    public static int tempNeutral;
    public static int tempAnger;
    public static int tempArrogance;
    public static int tempElegance;
    public static int tempFear;
    private static int tempMultiplier = 3;

    public enum CurrentEmotion { Neutral, Elegance, Arrogance, Anger, Fear };

    public static int Elegance
    {
        get
        {
            return elegance;
        }
        set
        {
            //Calculate temp emotion
            tempElegance += (value - elegance) * tempMultiplier;

            if (tempElegance > maxEmotion)
            {
                tempElegance = maxEmotion;
            }

            if (tempElegance + tempArrogance > maxEmotion)
            {
                tempArrogance -= 100 - tempElegance;
            }
            if (tempElegance + tempFear > maxEmotion)
            {
                tempFear -= 100 - tempElegance;
            }

            if (tempAnger != 0)
            {
                tempElegance -= tempAnger;
                tempAnger = 0;
            }

            if (tempElegance < 0)
            {
                tempAnger += -tempElegance;
                tempElegance = 0;
            }


            //Calculate normal
            elegance = value;
            
            if (elegance > maxEmotion)
            {
                elegance = maxEmotion;
            }

            if(elegance + arrogance > maxEmotion)
            {
                arrogance -= 100 - elegance;
            }
            if (elegance + fear > maxEmotion)
            {
                fear -= 100 - elegance;
            }

            if (anger != 0)
            {
                elegance -= anger;
                anger = 0;
            }

            if (elegance < 0)
            {
                anger += -elegance;
                elegance = 0;
            }
        }
    }    //Opposite of Elegance     (Bottom)
    public static int Anger
    {
        get
        {
            return anger;
        }
        set
        {
            //Calculate temp emotion
            tempAnger += (value - anger) * tempMultiplier;

            if (tempAnger > maxEmotion)
            {
                tempAnger = maxEmotion;
            }

            if (tempAnger + tempArrogance > maxEmotion)
            {
                tempArrogance -= 100 - tempAnger;
            }
            if (tempAnger + tempFear > maxEmotion)
            {
                tempFear -= 100 - tempAnger;
            }

            if (tempElegance != 0)
            {
                tempAnger -= tempElegance;
                tempElegance = 0;
            }

            if (tempAnger < 0)
            {
                tempElegance += -tempAnger;
                tempAnger = 0;
            }


            //Calculate normal
            anger = value;

            if (anger > maxEmotion)
            {
                anger = maxEmotion;
            }

            if (anger + arrogance > maxEmotion)
            {
                arrogance -= 100 - anger;
            }
            if (anger + fear > maxEmotion)
            {
                fear -= 100 - anger;
            }

            if (elegance != 0)
            {
                anger -= elegance;
                elegance = 0;
            }

            if (anger < 0)
            {
                elegance += -anger;
                anger = 0;
            }
        }
    }       //Opposite of Anger        (Top)
    public static int Arrogance
    {
        get
        {
            return arrogance;
        }
        set
        {
            //Calculate temp emotion
            tempArrogance += (value - arrogance) * tempMultiplier;

            if (tempArrogance > maxEmotion)
            {
                tempArrogance = maxEmotion;
            }

            if (tempArrogance + tempElegance > maxEmotion)
            {
                tempElegance -= 100 - tempArrogance;
            }
            if (tempArrogance + tempAnger > maxEmotion)
            {
                tempAnger -= 100 - tempArrogance;
            }

            if (tempFear != 0)
            {
                tempArrogance -= tempFear;
                tempFear = 0;
            }

            if (tempArrogance < 0)
            {
                tempFear += -tempArrogance;
                tempArrogance = 0;
            }


            //Calculate normal
            arrogance = value;

            if (arrogance > maxEmotion)
            {
                arrogance = maxEmotion;
            }

            if (arrogance + elegance > maxEmotion)
            {
                elegance -= 100 - arrogance;
            }
            if (arrogance + anger > maxEmotion)
            {
                anger -= 100 - arrogance;
            }

            if (fear != 0)
            {
                arrogance -= fear;
                fear = 0;
            }

            if (arrogance < 0)
            {
                fear += -arrogance;
                arrogance = 0;
            }
        }
    }   //Opposite of Fear         (Rigth)
    public static int Fear
    {
        get
        {
            return fear;
        }
        set
        {
            //Calculate temp emotion
            tempFear += (value - fear) * tempMultiplier;

            if (tempFear > maxEmotion)
            {
                tempFear = maxEmotion;
            }

            if (tempFear + tempElegance > maxEmotion)
            {
                tempElegance -= 100 - tempFear;
            }
            if (tempFear + tempAnger > maxEmotion)
            {
                tempAnger -= 100 - tempFear;
            }

            if (tempArrogance != 0)
            {
                tempFear -= tempArrogance;
                tempArrogance = 0;
            }

            if (tempFear < 0)
            {
                tempArrogance += -tempFear;
                tempFear = 0;
            }


            //Calculate normal
            fear = value;

            if (fear > maxEmotion)
            {
                fear = maxEmotion;
            }

            if (fear + elegance > maxEmotion)
            {
                elegance -= 100 - fear;
            }
            if (fear + anger > maxEmotion)
            {
                anger -= 100 - fear;
            }

            if (arrogance != 0)
            {
                fear -= arrogance;
                arrogance = 0;
            }

            if (fear < 0)
            {
                arrogance += -fear;
                fear = 0;
            }
        }
    }        //Opposite of Arrogance    (Left)
    public static int Neutral
    {
        get { return 100 - Anger - Arrogance - Elegance - Fear; }
    }

    public static int TempNeutral
    {
        get { return 100 - tempAnger - tempArrogance - tempElegance - tempFear; }
    }

    public static CurrentEmotion GetEmotion()
    {
        float biggestEmotionCount = TempNeutral;
        CurrentEmotion currEmotion = CurrentEmotion.Neutral;

        if (biggestEmotionCount < tempAnger)
        {
            biggestEmotionCount = tempAnger;
            currEmotion = CurrentEmotion.Anger;
        }

        if (biggestEmotionCount < tempArrogance)
        {
            biggestEmotionCount = tempArrogance;
            currEmotion = CurrentEmotion.Arrogance;
        }

        if (biggestEmotionCount < tempElegance)
        {
            biggestEmotionCount = tempElegance;
            currEmotion = CurrentEmotion.Elegance;
        }

        if (biggestEmotionCount < tempFear)
        {
            currEmotion = CurrentEmotion.Fear;
        }

        return currEmotion;
    }
    

    public static CurrentEmotion GetEmotionAtPositionOfTempEmotion()
    {
        if (tempElegance >= 50)
        {
            return CurrentEmotion.Elegance;
        }    //Elegance over 50

        if (tempArrogance >= 50)
        {
            return CurrentEmotion.Arrogance;
        }   //Arrogance over 50

        if (tempAnger >= 50)
        {
            return CurrentEmotion.Anger;
        }   //Anger over 50

        if (tempFear >= 50)
        {
            return CurrentEmotion.Fear;
        }   //Fear over 50

        return CurrentEmotion.Neutral;
    }

    public static void ResetTempEmotions()
    {
        tempAnger = anger;
        tempArrogance = arrogance;
        tempElegance = elegance;
        tempFear = fear;

        Test_Stats.PrintEmotions(" - Temp Emotions Resetted");
    }
}
