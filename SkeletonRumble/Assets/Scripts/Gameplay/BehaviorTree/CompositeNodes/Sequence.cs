using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sequence : Node
{
    public List<Node> m_nodes = new List<Node>();

    //public Sequence(List<Node> nodes)
    //{
    //    this.m_nodes = nodes;
    //}

    public override NodeStates Evaluate()
    {  
            bool anyChildRunning = false;

            foreach (Node node in this.m_nodes)
            {
                switch (node.Evaluate())
                {
                    case NodeStates.RUNNING:
                        anyChildRunning = true;
                        break;
                        

                    case NodeStates.SUCCESS:
                        break;

                    case NodeStates.FAILURE:
                        this.m_nodeState = NodeStates.FAILURE;
                        return this.m_nodeState;

                    default:
                        break;
                }
            }

            this.m_nodeState = anyChildRunning ? NodeStates.RUNNING : NodeStates.SUCCESS;

            return this.m_nodeState;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(190, 185, 221);

        Vector2 pos1 = this.nodeBottomPosition.position;
        Vector2 pos2;
        Vector2 pos3;
        Vector2 pos4;

        foreach (Node nodes in this.m_nodes)
        {
            pos4 = nodes.nodeTopPosition.position;
            pos2 = pos1;
            pos2.y = pos4.y + (pos1.y - pos4.y) / 2f;

            pos3 = pos4;
            pos3.y = pos2.y;

            Gizmos.DrawLine(pos1, pos2);
            Gizmos.DrawLine(pos2, pos3);
            Gizmos.DrawLine(pos3, pos4);
        }

        for (int i = 0; i < this.m_nodes.Count; i++)
        {
            this.m_nodes[i].index.text = i.ToString();
        }
    }
}
