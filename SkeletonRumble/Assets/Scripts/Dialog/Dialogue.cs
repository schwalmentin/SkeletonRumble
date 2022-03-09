using System.Collections.Generic;

public class Dialogue
{
    public Dialogue(string[] DialogueSegments, string[] Answers, List<EmotionalChange> Change)
    {
        dialogueSegments = DialogueSegments;
        answers = Answers;
        emotionalChange = Change;
    }


    public string[] dialogueSegments;

    public string[] answers;
    
    public struct EmotionalChange
    {
        public enum Emotion{ Elegance, Anger, Arrogance, Fear };
        public Emotion emotion;
        public int value;
        public int dialogueIndex;
    }
    public List<EmotionalChange> emotionalChange;
}
