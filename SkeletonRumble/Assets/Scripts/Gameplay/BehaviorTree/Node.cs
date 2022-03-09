using System.Collections;
using UnityEngine;
using TMPro;

public abstract class Node : MonoBehaviour
{
    public Transform nodeTopPosition;
    public Transform nodeBottomPosition;
    public TextMeshProUGUI index;

    protected NodeStates m_nodeState;

    public NodeStates nodeState
    {
        get
        {
            return this.m_nodeState;
        }
    }

    public abstract NodeStates Evaluate();
}
