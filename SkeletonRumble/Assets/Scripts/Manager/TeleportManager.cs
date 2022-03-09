using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeleportManager : MonoBehaviour
{
    public PlayerCharacter playerCharacter;
    public float tpTime;
    private float tpTimer;
    public float lerpSpeed;
    private float timer;
    [HideInInspector] public bool teleport;
    public Image screen;
    [HideInInspector] public Color screenColor;
    private Vector2 targetPos;
    private bool playerAbleToMove;
    private bool onTeleported;
    private GameManager gameManager;

    private bool x;
    private bool y;

    private void Start()
    {
        this.gameManager = FindObjectOfType<GameManager>();
    }

    private void Update()
    {
        if (!this.teleport)
        {
            return;
        }

        if (!this.x)
        {
            this.timer += Time.deltaTime * this.lerpSpeed;

            if (this.timer < 1)
            {
                this.screen.color = new Color(this.screenColor.r, this.screenColor.g, this.screenColor.b, this.timer);
                return;
            }

            if (!this.y)
            {
                this.y = true;
                this.playerCharacter.transform.position = this.targetPos;
                this.screen.color = new Color(this.screenColor.r, this.screenColor.g, this.screenColor.b, 1);

                if (this.onTeleported)
                {
                    this.gameManager.OnTeleported();
                }
            }

            this.tpTimer -= Time.deltaTime;

            if (this.tpTimer <= 0)
            {
                this.x = true;
                this.timer = 0;
            }

            return;
        }

        this.timer += Time.deltaTime * this.lerpSpeed;

        if (this.timer < 1)
        {
            this.screen.color = new Color(this.screenColor.r, this.screenColor.g, this.screenColor.b, 1 - this.timer);
            return;
        }

        this.teleport = false;
        this.x = false;
        this.timer = 0;
        this.screen.color = new Color(this.screen.color.r, this.screen.color.g, this.screen.color.b, 0);
        this.playerCharacter.isAbleToMove = this.playerAbleToMove;
        this.playerCharacter.cannotReceiveInput = true;
    }

    public void TeleportPlayer(Vector2 position, bool lockPlayerAfterTP = false, bool startOfBattle = false)
    {
        if (this.teleport)
        {
            return;
        }

        this.teleport = true;
        this.timer = 0;
        this.playerCharacter.isAbleToMove = false;
        this.playerCharacter.cannotReceiveInput = true;
        this.playerCharacter.velocity.x = 0;
        this.x = false;
        this.y = false;
        this.targetPos = position;
        this.tpTimer = this.tpTime;
        this.playerAbleToMove = !lockPlayerAfterTP;
        this.onTeleported = startOfBattle;
    }
}
