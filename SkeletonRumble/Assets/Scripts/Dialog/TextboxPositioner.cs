using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextboxPositioner : MonoBehaviour
{
    public bool answer;
    [Space]
    public int textboxPositionNumber;
    public Vector2 relativeTextboxOffset;
    public Vector2 relativeAnswerButtonOffset;

    private Display_Dialogue displayDialog;

    private void Start()
    {
        this.displayDialog = FindObjectOfType<Display_Dialogue>();
    }

    private void Update()
    {
        if (this.answer)
        {
            this.displayDialog.answerTextBoxPositionings[this.textboxPositionNumber].TextBoxPivotPosition = this.transform.position * 333.5f;
            this.displayDialog.answerTextBoxPositionings[this.textboxPositionNumber].TextBoxImagePositionRelativeToPivot = this.relativeTextboxOffset;
            this.displayDialog.answerTextBoxPositionings[this.textboxPositionNumber].AnswerButtonPositionRelativeToPivot = this.relativeAnswerButtonOffset;
            this.displayDialog.answerTextBoxPositionings[this.textboxPositionNumber].pivotStartAngle = this.transform.localEulerAngles.z;
            this.displayDialog.answerTextBoxPositionings[this.textboxPositionNumber].pivotEndAngle = this.transform.localEulerAngles.z;
            return;
        }

        this.displayDialog.textBoxPositionings[this.textboxPositionNumber].TextBoxPivotPosition = this.transform.position * 333.5f;
        this.displayDialog.textBoxPositionings[this.textboxPositionNumber].TextBoxImagePositionRelativeToPivot = this.relativeTextboxOffset;
        this.displayDialog.textBoxPositionings[this.textboxPositionNumber].AnswerButtonPositionRelativeToPivot = this.relativeAnswerButtonOffset;
        this.displayDialog.textBoxPositionings[this.textboxPositionNumber].pivotStartAngle = -this.transform.localEulerAngles.z;
        this.displayDialog.textBoxPositionings[this.textboxPositionNumber].pivotEndAngle = -this.transform.localEulerAngles.z;
    }
}
