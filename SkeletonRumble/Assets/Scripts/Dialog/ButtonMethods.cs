using UnityEngine;

public class ButtonMethods : MonoBehaviour
{
    private int answerIndex;

    private void OnEnable()
    {
        if (this.GetComponentInParent<Display_Dialogue>().answer)
        {
            answerIndex = this.GetComponentInParent<Display_Dialogue>().answerButtonIndex;
            this.GetComponentInParent<Display_Dialogue>().answerButtonIndex++;
        }
    }

    public void AcceptAnswerFromButton()
    {
        this.GetComponentInParent<Display_Dialogue>().answer = true;
        this.GetComponentInParent<Display_Dialogue>().ShowDialogueButton(answerIndex);
    }
}
