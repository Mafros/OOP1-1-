using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeInfoToList : MonoBehaviour
{
    public List<StatusPlayer> Agents;
    GameObject Map;
    TakeInfoToListE enemyControls;
    public GameObject purgatory;

    void Start()
    {
        UpdateList();
        Map = GameObject.FindGameObjectWithTag("Map").gameObject;
        enemyControls = GameObject.FindGameObjectWithTag("EControls").GetComponent<TakeInfoToListE>();
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
            if (cordNode == gameObject.transform.GetChild(j).gameObject.GetComponent<StatusPlayer>().iAmStayOnNode)
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

    public StatusPlayer GetChosenAgent()
    {
        if (Agents.Count != 0)
            for (int i = 0; i < Agents.Count; i++)
                if (Agents[i].hasChosen == true)
                    return Agents[i];
        return null;
    }
    public bool isHaveChosenAgentBool()
    {
        if (Agents.Count != 0)
            for (int i = 0; i < Agents.Count; i++)
                if (Agents[i].hasChosen == true)
                    return true;
        return false;
    }

    public GameObject FindEnemyOnNode(int idNode)
    {
        for(int i = 0;i < enemyControls.Agents.Count; i++)
        {
            if (enemyControls.Agents[i].iAmStayOnNode == idNode)
                return enemyControls.Agents[i].gameObject;
        }
        return null;
    }


    public void GetStatusFromBut(int statusFromBut)
    {
        if(isHaveChosenAgentBool())
            GetChosenAgent().status = statusFromBut;
        if(statusFromBut != GetChosenAgent().status)
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
        for(int i = 0;i < Agents.Count; i++)
        {
            Agents[i].isMoved = false;
            Agents[i].isShooted = false;
            Agents[i].isThrowG = false;
            Agents[i].isUseAbility = false;
        }
    }

    public List<StatusPlayer> UpdateList()
    {
        Agents.Clear();
        for (int i = 0; i < transform.childCount; i++)
            if (transform.GetChild(i).gameObject.activeSelf)
                Agents.Add(transform.GetChild(i).gameObject.GetComponent<StatusPlayer>());
        return Agents;
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

    public bool IsPlayerStayingNode(int idNode)
    {
        for (int i = 0; i < Agents.Count; i++)
            if (Agents[i].iAmStayOnNode == idNode)
                return true;
        return false;
    }
}
