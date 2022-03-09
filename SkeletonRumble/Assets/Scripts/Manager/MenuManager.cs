using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    private Animator cameraAnimator;
    public Animator fadeAnimator;
    private bool pressedAnyButton;
    public GameObject menuButtons;
    public Animator menuButtonsAnimator;
    public Animator pressedAnyButtonAnimator;
    public Animator creditsAnimator;
    public Button startButton;
    public Button exitCreditButton;

    private void Start()
    {
        this.cameraAnimator = this.GetComponent<Animator>();
    }

    public void PressAnyButton()
    {
        if (this.pressedAnyButton)
        {
            return;
        }

        this.cameraAnimator.SetBool("pressedAnyButton", true);
        this.pressedAnyButtonAnimator.SetBool("pressedAnyButton", true);

        this.pressedAnyButton = true;
    }

    public void CameraMovedToEnd()
    {
        this.menuButtons.SetActive(true);
        this.menuButtonsAnimator.SetBool("pressedAnyButton", true);
        this.startButton.Select();
    }

    public void SwitchScene(int sceneIndex)
    {
        //SceneManager.LoadScene(sceneIndex);
        this.fadeAnimator.SetBool("Fade", true);
    }

    public void OnCredits()
    {
        this.menuButtonsAnimator.SetBool("pressedAnyButton", false);
        this.creditsAnimator.SetBool("credit", true);
        this.exitCreditButton.Select();
    }

    public void ExitCredits()
    {
        this.menuButtonsAnimator.SetBool("pressedAnyButton", true);
        this.creditsAnimator.SetBool("credit", false);
        this.startButton.Select();
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
