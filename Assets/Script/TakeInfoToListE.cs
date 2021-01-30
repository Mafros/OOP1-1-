using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeInfoToListE : MonoBehaviour
{
    public List<StatusEnemy> Agents;
    GameObject Map;
    public GameObject purgatory;

    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Agents.Add(transform.GetChild(i).gameObject.GetComponent<StatusEnemy>());
            //print(Agents[i].nameAgent);

        }
        Map = GameObject.FindGameObjectWithTag("Map").gameObject;
    }

    void Update()
    {
        //if(Agents.Count != gameObject.transform.childCount)
        CheckAgents();

    }

    public GameObject FindPlayerOnNode(int cordNode)
    {
        GameObject agent = null;

        for (int j = 0; j < gameObject.transform.childCount; j++)
        {
            if (cordNode == gameObject.transform.GetChild(j).gameObject.GetComponent<StatusEnemy>().iAmStayOnNode)
            {
                agent = gameObject.transform.GetChild(j).gameObject;
                return agent;
            }
        }

        return agent;
    }

    public GameObject FindNodeOnPlayer(int nodeStayingPlayer)
    {
        GameObject node = null;

        for (int j = 0; j < Map.transform.childCount; j++)
        {
            if (nodeStayingPlayer == Map.transform.GetChild(j).gameObject.GetComponent<PathfindingNode>().idNode)
            {
                node = Map.transform.GetChild(j).gameObject;
                return node;
            }
        }

        return node;
    }

    public StatusEnemy GetChosenAgent()
    {
        if (Agents.Count != 0)
            for (int i = 0; i < Agents.Count; i++)
                if (Agents[i].hasChosen == true)
                {
                    StatusEnemy agent = Agents[i];
                    return agent;
                }
        return null;
    }
    public bool isHaveChosenAgentBool()
    {
        if (Agents.Count != 0)
            for (int i = 0; i < Agents.Count; i++)
                if (Agents[i].hasChosen == true)
                {
                    return true;
                }
        return false;
    }

    public void GetStatusFromBut(int statusFromBut)
    {
        if (isHaveChosenAgentBool())
            GetChosenAgent().status = statusFromBut;
        if (statusFromBut != GetChosenAgent().status)
        {
            Map.GetComponent<PlaceCordToQuads>().ClearMap();

        }


    }

    public void SetStatusAgent(int setStatus)
    {
        GetStatusFromBut(setStatus);
        GetChosenAgent().status = setStatus;
    }

    public void ClearActionsAll()
    {
        for (int i = 0; i < Agents.Count; i++)
        {
            Agents[i].isMoved = false;
            Agents[i].isShooted = false;
            Agents[i].isThrowG = false;
            Agents[i].isUseAbility = false;
        }
    }

    public void UpdateList()
    {
        Agents.Clear();
        for (int i = 0; i < transform.childCount; i++)
            if(transform.GetChild(i).gameObject.activeSelf)
                Agents.Add(transform.GetChild(i).gameObject.GetComponent<StatusEnemy>());

    }

    void CheckAgents()
    {
        for (int i = 0; i < Agents.Count; i++)
            if (!Agents[i].gameObject.activeSelf)
            {
                UpdateList();
                return;
            }
    }

    public bool IsEnemyStayingNode(int idNode)
    {
        for (int i = 0; i < Agents.Count; i++)
            if (Agents[i].iAmStayOnNode == idNode)
                return true;
        return false;
    }

}
