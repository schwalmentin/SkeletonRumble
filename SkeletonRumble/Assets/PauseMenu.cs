using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public Animator pauseMenuAnimator;
    public Animator FadeAnimator;
    public Button[] buttons;

    public void OnPause(bool pause)
    {
        this.pauseMenuAnimator.SetBool("Pause", pause);

        if (this.buttons.Length > 0 && pause)
        {
            this.buttons[0].Select();
        }

        foreach (Button item in this.buttons)
        {
            item.enabled = pause;
        }

        Time.timeScale = pause ? 0 : 1;
    }

    public void SwitchToMainMenu()
    {
        this.FadeAnimator.SetBool("Fade2", true);
    }
}
