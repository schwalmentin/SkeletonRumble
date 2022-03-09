using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportPlayer : MonoBehaviour
{
    //Values
    public Vector2 size;
    public LayerMask playerMask;
    public Transform destination;
    private TeleportManager tpManager;
    public Color screenColor;

    private void Start()
    {
        this.tpManager = FindObjectOfType<TeleportManager>();
    }

    private void Update()
    {
        if (Physics2D.OverlapBox(this.transform.position, this.size, 0, this.playerMask) && !this.tpManager.teleport)
        {
            this.tpManager.TeleportPlayer(this.destination.position);
            this.tpManager.screenColor = this.screenColor;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(this.transform.position, this.size);
    }
}
