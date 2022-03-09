using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inverter : Node
{
    protected Node m_node;

    public Inverter(Node node)
    {
        this.m_node = node;
    }

    public override NodeStates Evaluate()
    {
        switch (this.m_node.Evaluate())
        {
            case NodeStates.RUNNING:
                this.m_nodeState = NodeStates.RUNNING;
                break;

            case NodeStates.SUCCESS:
                this.m_nodeState = NodeStates.FAILURE;
                break;

            case NodeStates.FAILURE:
                this.m_nodeState = NodeStates.SUCCESS;
                break;
            default:
                break;
        }

        return this.m_nodeState;
    }
}
