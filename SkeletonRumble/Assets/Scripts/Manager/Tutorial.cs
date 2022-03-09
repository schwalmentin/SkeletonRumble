using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class Tutorial : MonoBehaviour
{
    //Values
    public Vector2 size;
    public LayerMask playerMask;
    public Image image;
    public ButtonPrompts buttonPrompts;
    public float lerpSpeed;
    private float timer;
    private bool inRange;

    //Change Button Prompts
    private string currentInputDevice;
    private bool firstDevice;

    private void Update()
    {
        if (Physics2D.OverlapBox(this.transform.position, this.size, 0, this.playerMask))
        {
            if (this.inRange)
            {
                this.timer += Time.deltaTime * this.lerpSpeed;
                //this.image.color = new Color(this.image.color.r, this.image.color.g, this.image.color.b, Mathf.Lerp(this.image.color.a, 0.7f, this.timer));
                this.buttonPrompts.keyBoard.color = new Color(this.buttonPrompts.keyBoard.color.r, this.buttonPrompts.keyBoard.color.g, this.buttonPrompts.keyBoard.color.b, Mathf.Lerp(this.buttonPrompts.keyBoard.color.a, 1, this.timer));
                this.buttonPrompts.xBox.color = new Color(this.buttonPrompts.xBox.color.r, this.buttonPrompts.xBox.color.g, this.buttonPrompts.xBox.color.b, Mathf.Lerp(this.buttonPrompts.xBox.color.a, 1, this.timer));
                this.buttonPrompts.playStation.color = new Color(this.buttonPrompts.playStation.color.r, this.buttonPrompts.playStation.color.g, this.buttonPrompts.playStation.color.b, Mathf.Lerp(this.buttonPrompts.playStation.color.a, 1, this.timer));
                this.buttonPrompts.nintendo.color = new Color(this.buttonPrompts.playStation.color.r, this.buttonPrompts.playStation.color.g, this.buttonPrompts.playStation.color.b, Mathf.Lerp(this.buttonPrompts.playStation.color.a, 1, this.timer));
                this.buttonPrompts.neutral.color = new Color(this.buttonPrompts.neutral.color.r, this.buttonPrompts.neutral.color.g, this.buttonPrompts.neutral.color.b, Mathf.Lerp(this.buttonPrompts.neutral.color.a, 1, this.timer));
                return;
            }

            this.inRange = true;
            this.timer = 0;
            return;
        }

        if (!this.inRange)
        {
            this.timer += Time.deltaTime * this.lerpSpeed;
            //this.image.color = new Color(this.image.color.r, this.image.color.g, this.image.color.b, Mathf.Lerp(this.image.color.a, 0, this.timer));
            this.buttonPrompts.keyBoard.color = new Color(this.buttonPrompts.keyBoard.color.r, this.buttonPrompts.keyBoard.color.g, this.buttonPrompts.keyBoard.color.b, Mathf.Lerp(this.buttonPrompts.keyBoard.color.a, 0, this.timer));
            this.buttonPrompts.xBox.color = new Color(this.buttonPrompts.xBox.color.r, this.buttonPrompts.xBox.color.g, this.buttonPrompts.xBox.color.b, Mathf.Lerp(this.buttonPrompts.xBox.color.a, 0, this.timer));
            this.buttonPrompts.playStation.color = new Color(this.buttonPrompts.playStation.color.r, this.buttonPrompts.playStation.color.g, this.buttonPrompts.playStation.color.b, Mathf.Lerp(this.buttonPrompts.playStation.color.a, 0, this.timer));
            this.buttonPrompts.nintendo.color = new Color(this.buttonPrompts.playStation.color.r, this.buttonPrompts.playStation.color.g, this.buttonPrompts.playStation.color.b, Mathf.Lerp(this.buttonPrompts.playStation.color.a, 0, this.timer));
            this.buttonPrompts.neutral.color = new Color(this.buttonPrompts.neutral.color.r, this.buttonPrompts.neutral.color.g, this.buttonPrompts.neutral.color.b, Mathf.Lerp(this.buttonPrompts.neutral.color.a, 0, this.timer));
            return;
        }

        this.inRange = false;
        this.timer = 0;
    }

    public void ChangeButtonPrompts(InputAction.CallbackContext context)
    {
        if (!context.performed) 
        { 
            return; 
        }
        InputDevice device = context.control.device;

        if (device.displayName == this.currentInputDevice)
        {
            return;
        }

        this.currentInputDevice = device.displayName;

        //Switch to Keyboard
        if (device.displayName == "Keyboard" || device.displayName == "Mouse")
        {
            this.buttonPrompts.keyBoard.gameObject.SetActive(true);
            this.buttonPrompts.xBox.gameObject.SetActive(false);
            this.buttonPrompts.playStation.gameObject.SetActive(false);
            this.buttonPrompts.nintendo.gameObject.SetActive(false);
            this.buttonPrompts.neutral.gameObject.SetActive(false);

            return;
        }

        //Switch to xBox
        if (device.displayName == "Xbox Controller")
        {
            this.buttonPrompts.keyBoard.gameObject.SetActive(false);
            this.buttonPrompts.xBox.gameObject.SetActive(true);
            this.buttonPrompts.playStation.gameObject.SetActive(false);
            this.buttonPrompts.nintendo.gameObject.SetActive(false);
            this.buttonPrompts.neutral.gameObject.SetActive(false);
            return;
        }

        //Switch to Playstation
        if (device.displayName == "PlayStation Controller" || device.displayName == "Wireless Controller")
        {
            this.buttonPrompts.keyBoard.gameObject.SetActive(false);
            this.buttonPrompts.xBox.gameObject.SetActive(false);
            this.buttonPrompts.playStation.gameObject.SetActive(true);
            this.buttonPrompts.nintendo.gameObject.SetActive(false);
            this.buttonPrompts.neutral.gameObject.SetActive(false);
            return;
        }

        //Switch to "Switch"
        if (device.displayName == "Switch Controller" || device.displayName == "Pro Controller")
        {
            this.buttonPrompts.keyBoard.gameObject.SetActive(false);
            this.buttonPrompts.xBox.gameObject.SetActive(false);
            this.buttonPrompts.playStation.gameObject.SetActive(false);
            this.buttonPrompts.nintendo.gameObject.SetActive(true);
            this.buttonPrompts.neutral.gameObject.SetActive(false);
            return;
        }

        //Switch to neutral icons
        this.buttonPrompts.keyBoard.gameObject.SetActive(false);
        this.buttonPrompts.xBox.gameObject.SetActive(false);
        this.buttonPrompts.playStation.gameObject.SetActive(false);
        this.buttonPrompts.nintendo.gameObject.SetActive(false);
        this.buttonPrompts.neutral.gameObject.SetActive(true);       
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(this.transform.position, this.size);
    }
}

[System.Serializable]
public struct ButtonPrompts
{
    public Image keyBoard;
    public Image xBox;
    public Image playStation;
    public Image nintendo;
    public Image neutral;
}
