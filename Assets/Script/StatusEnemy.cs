using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEnemy : MonoBehaviour
{

    public string nameAgent;
    public int status;
    public int maxhp;
    public int hp;
    public HP healthBar;
    public int arm;
    public int iAmStayOnNode;
    public int idGun;
    public bool hasChosen;
    public bool hurted;
    public bool moved;
    public bool isMidfield;
    public bool isFProt;

    public bool isMoved;
    public bool isShooted;
    public bool isThrowG;
    public bool isUseAbility;

    public bool isViewOnTrigger;

    TakeInfoToList infoAgents;

    void Start()
    {
        infoAgents = GameObject.FindGameObjectWithTag("Controls").GetComponent<TakeInfoToList>();
        healthBar.SetMaxHealth(maxhp);
    }

    void Update()
    {
        if (isViewOnTrigger)
        {
            gameObject.GetComponent<MeshRenderer>().enabled = true;
            gameObject.transform.GetChild(gameObject.transform.childCount - 1).gameObject.SetActive(true);
        }
        else
        {
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            gameObject.transform.GetChild(gameObject.transform.childCount - 1).gameObject.SetActive(false);

        }
        healthBar.SetHealth(hp);
        if(hp <= 0)
        {

            status = 0;
            gameObject.transform.position = gameObject.transform.parent.GetComponent<TakeInfoToListE>().purgatory.transform.position;
            gameObject.transform.parent.GetComponent<TakeInfoToListE>().FindNodeOnPlayer(iAmStayOnNode).GetComponent<PathfindingNode>().enemyStayingHere = false;
            iAmStayOnNode = -1;
            GameObject.FindGameObjectWithTag("Map").GetComponent<PathfindingField>().manageQuads.ClearMap();
            GameObject.FindGameObjectWithTag("Map").GetComponent<PathfindingField>().FieldUpdate();

            gameObject.SetActive(false);
        }

    }

    //private void OnTriggerStay(Collider other)
    //{
    //    if (!isViewOnTrigger)
    //        if (infoAgents.Agents.Count != 0)
    //            for (int i = 0; i < infoAgents.Agents.Count; i++)
    //                if (infoAgents.Agents.Count != 0)
    //                    if (other == infoAgents.Agents[i].transform.GetChild(3).gameObject.GetComponent<Collider>())
    //                        isViewOnTrigger = true;
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    if (isViewOnTrigger)
    //        if (infoAgents.Agents.Count != 0)
    //            for (int i = 0; i < infoAgents.Agents.Count; i++)
    //                if (other == infoAgents.Agents[i].transform.GetChild(3).gameObject.GetComponent<Collider>())
    //                    isViewOnTrigger = false;
    //}

    //public List<int> GetStatsAgent()
    //{
    //    List<int> stats = new List<int>();
    //    stats.Add(status);
    //    stats.Add(maxhp);
    //    stats.Add(maxhp);
    //    stats.Add(hp);
    //    stats.Add(arm);
    //    stats.Add(iAmStayOnNode);
    //    stats.Add(idGun);

    //    return stats;

    //}

}
