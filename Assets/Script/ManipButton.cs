using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManipButton : MonoBehaviour
{
    public List<StatusButton> stat;
    TakeInfoToList infoAgents;
    PathfindingField pathfScript;

    // Start is called before the first frame update
    void Start()
    {
        infoAgents = GameObject.FindGameObjectWithTag("Controls").GetComponent<TakeInfoToList>();
        pathfScript = GameObject.FindGameObjectWithTag("Map").GetComponent<PathfindingField>();
        for (int i = 0; i < gameObject.transform.childCount; i++)
            if(gameObject.transform.GetChild(i).GetComponent<StatusButton>())
                stat.Add(gameObject.transform.GetChild(i).GetComponent<StatusButton>());
    }

    // Update is called once per frame
    void Update()
    {
        CheckStatus();
        if (infoAgents.isHaveChosenAgentBool())
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                infoAgents.GetChosenAgent().status = 0;
                pathfScript.manageQuads.ClearMap();
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                infoAgents.GetChosenAgent().status = 1;
                //pathfScript.UpdateNodeState_inst();
                pathfScript.manageQuads.ClearMap();
                pathfScript.GeneratePathMayToWay();
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                infoAgents.GetChosenAgent().status = 3;
                pathfScript.manageQuads.ClearMap();
                pathfScript.GeneratePlaceShootRate();
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {
                infoAgents.GetChosenAgent().status = 2;
                pathfScript.manageQuads.ClearMap();
                pathfScript.GeneratePlaceMayToBlow();
            }
            else if (Input.GetKeyDown(KeyCode.T))
            {
                infoAgents.GetChosenAgent().status = 4;
                pathfScript.manageQuads.ClearMap();

            }
        }
    }

    void CheckStatus()
    {
        if (infoAgents.isHaveChosenAgentBool())
        {
            if (stat[0].statusToSendAgent == infoAgents.GetChosenAgent().status)
                stat[0].gameObject.GetComponent<Button>().interactable = false;
            else
                stat[0].gameObject.GetComponent<Button>().interactable = true;
            if (stat[1].statusToSendAgent == infoAgents.GetChosenAgent().status || infoAgents.GetChosenAgent().isMoved)
                stat[1].gameObject.GetComponent<Button>().interactable = false;
            else
                stat[1].gameObject.GetComponent<Button>().interactable = true;
            if (stat[2].statusToSendAgent == infoAgents.GetChosenAgent().status || infoAgents.GetChosenAgent().isShooted)
                stat[2].gameObject.GetComponent<Button>().interactable = false;
            else
                stat[2].gameObject.GetComponent<Button>().interactable = true;
            if (stat[3].statusToSendAgent == infoAgents.GetChosenAgent().status || infoAgents.GetChosenAgent().isThrowG)
                stat[3].gameObject.GetComponent<Button>().interactable = false;
            else
                stat[3].gameObject.GetComponent<Button>().interactable = true;
            if (stat[4].statusToSendAgent == infoAgents.GetChosenAgent().status || infoAgents.GetChosenAgent().isUseAbility)
                stat[4].gameObject.GetComponent<Button>().interactable = false;
            else
                stat[4].gameObject.GetComponent<Button>().interactable = true;
        }
        else
        {
            for (int i = 0; i < stat.Count; i++)
                stat[i].GetComponent<Button>().interactable = false;
        }
    }
}
