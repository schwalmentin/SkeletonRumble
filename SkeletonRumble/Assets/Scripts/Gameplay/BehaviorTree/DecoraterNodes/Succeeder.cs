using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Succeeder : Node
{
    public Node m_node;

    public override NodeStates Evaluate()
    {
        switch (this.m_node.Evaluate())
        {
            case NodeStates.RUNNING:
                this.m_nodeState = NodeStates.RUNNING;
                break;

            case NodeStates.SUCCESS:
                this.m_nodeState = NodeStates.SUCCESS;
                break;

            case NodeStates.FAILURE:
                this.m_nodeState = NodeStates.SUCCESS;
                break;
            default:
                break;
        }

        return this.m_nodeState;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(190, 185, 221);

        Vector2 pos1 = this.nodeBottomPosition.position;
        Vector2 pos2;
        Vector2 pos3;
        Vector2 pos4;

        pos4 = this.m_node.nodeTopPosition.position;
        pos2 = pos1;
        pos2.y = pos4.y + (pos1.y - pos4.y) / 2f;

        pos3 = pos4;
        pos3.y = pos2.y;

        Gizmos.DrawLine(pos1, pos2);
        Gizmos.DrawLine(pos2, pos3);
        Gizmos.DrawLine(pos3, pos4);
    }
}
