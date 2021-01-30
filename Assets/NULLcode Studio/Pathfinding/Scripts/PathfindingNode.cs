using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;

public class PathfindingNode : MonoBehaviour
{

    TakeInfoToList infoAgents;
    TakeInfoToListE infoEAgents;
    PlaceCordToQuads scrPCTQ;
    GameObject integObj;
    string nodeName;
    public int idNode;
    public bool iAmTagged;
    public bool blowQuad;
    public bool shootQuad;
    public bool enemyStayingHere;
    public bool isActivatedNode = true;
    public bool isFoVActiveNode = false;
    public bool nodeOnFog = true;
    public bool isIntegrationsNode = false;
    //public bool showFogObj = true;

    public int x { get; set; }
    public int y { get; set; }
    public int cost { get; set; }
    public Transform target { get; set; }
    public bool isLock { get; set; }
    public bool isPlayer { get; set; }
    public MeshRenderer mesh; // указываем меш клетки

    Collider colSphere;

    void Start()
    {
        scrPCTQ = gameObject.transform.parent.GetComponent<PlaceCordToQuads>();
        infoAgents = GameObject.FindGameObjectWithTag("Controls").gameObject.GetComponent<TakeInfoToList>();
        infoEAgents = GameObject.FindGameObjectWithTag("EControls").gameObject.GetComponent<TakeInfoToListE>();
        nodeName = name;
        int.TryParse(string.Join("", nodeName.Where(c => char.IsDigit(c))), out idNode);
        colSphere = GameObject.FindGameObjectWithTag("SphereCMTQ").GetComponent<SphereCollider>();
        integObj = GameObject.FindGameObjectWithTag("IntegrationsObj");
    }

    void Update()
    {
        if (!isActivatedNode)
        {
            mesh.material.color = Color.clear;
            transform.GetChild(1).gameObject.SetActive(false);
        }
        //if (showFogObj)
        //    gameObject.transform.GetChild(1).gameObject.SetActive(true);
        //else
        //    gameObject.transform.GetChild(1).gameObject.SetActive(false);
    }



    private void OnMouseEnter()
    {
        if (isActivatedNode && !nodeOnFog)
            if (infoAgents.isHaveChosenAgentBool())
                if (infoAgents.GetChosenAgent().status == 2)
                {
                    if (iAmTagged == true)
                        foreach (Collider item in Physics.OverlapSphere(gameObject.transform.position, 1f))
                            if (item.gameObject.GetComponent<PathfindingNode>())
                                item.gameObject.GetComponent<PathfindingNode>().blowQuad = true;
                }
                else if (infoAgents.GetChosenAgent().status == 3)
                {
                    if (iAmTagged == true)
                        shootQuad = true;
                }
    }

    private void OnMouseExit()
    {
        if (isActivatedNode && !nodeOnFog)
            if (infoAgents.isHaveChosenAgentBool())
                if (infoAgents.GetChosenAgent().status == 2)
                {
                    if (iAmTagged == true)
                        foreach (Collider item in Physics.OverlapSphere(gameObject.transform.position, 1f))
                            if (item.gameObject.GetComponent<PathfindingNode>())
                                item.gameObject.GetComponent<PathfindingNode>().blowQuad = false;
                }
                else if (infoAgents.GetChosenAgent().status == 3)
                {
                    if (iAmTagged == true)
                        shootQuad = false;
                }
    }
    private void OnTriggerStay(Collider other)
    {
        for (int i = 0; i < scrPCTQ.boxColTg.Count; i++)
            if (other == scrPCTQ.boxColTg[i])
                isActivatedNode = false;

        if (infoAgents.Agents.Count != 0)
            for (int i = 0; i < infoAgents.Agents.Count; i++)
                if (infoAgents.Agents[i].gameObject.GetComponent<StatusPlayer>())
                    if (other == infoAgents.Agents[i].gameObject.GetComponent<Collider>())
                        if (infoAgents.Agents[i].iAmStayOnNode == -1)
                            infoAgents.Agents[i].iAmStayOnNode = idNode;

        if (infoEAgents.Agents.Count != 0)
            for (int i = 0; i < infoEAgents.Agents.Count; i++)
                if (infoEAgents.Agents[i].gameObject.GetComponent<StatusEnemy>())
                    if (other == infoEAgents.Agents[i].gameObject.GetComponent<Collider>())
                        if (infoEAgents.Agents[i].iAmStayOnNode == -1)
                            infoEAgents.Agents[i].iAmStayOnNode = idNode;

        if (integObj != null)
        for (int i = 0; i < integObj.transform.childCount; i++)
            if (integObj.transform.GetChild(i).CompareTag("InteractBut"))
            {
                GameObject subObj = integObj.transform.GetChild(i).gameObject;
                for (int j = 0; j < integObj.transform.GetChild(i).transform.childCount; j++)
                    if (other == subObj.transform.GetChild(j).GetComponent<Collider>())
                        if (!isIntegrationsNode)
                        {
                            isIntegrationsNode = true;
                            other.transform.parent.gameObject.GetComponent<IntegrationsObj>().idNodeOnStaying.Add(idNode);
                        }
            }

        if (isActivatedNode && !nodeOnFog)
            if (infoAgents.isHaveChosenAgentBool())
            {
                for (int i = 0; i < infoAgents.Agents.Count; i++)
                    if (infoAgents.Agents[i])
                        if (other == infoAgents.Agents[i].gameObject.GetComponent<Collider>())
                            infoAgents.Agents[i].iAmStayOnNode = idNode;
            }
            else if (infoEAgents.isHaveChosenAgentBool())
            {
                for (int i = 0; i < infoEAgents.Agents.Count; i++)
                    if (other == infoEAgents.Agents[i].gameObject.GetComponent<Collider>())
                        infoEAgents.Agents[i].iAmStayOnNode = idNode;
            }
        

    }

    private void OnTriggerEnter(Collider other)
    {
        if (isActivatedNode && !nodeOnFog)
            if (infoAgents.isHaveChosenAgentBool())
                if (infoAgents.GetChosenAgent().status == 1)
                {
                    if (other == infoAgents.GetChosenAgent().transform.GetChild(0).GetComponent<Collider>())
                    {
                        if (infoAgents.FindPlayerOnNode(idNode))
                            iAmTagged = false;
                        else
                            iAmTagged = true;

                        if (infoAgents.FindEnemyOnNode(idNode))
                        {
                            enemyStayingHere = true;
                        }
                        else
                            enemyStayingHere = false;

                    }
                }
                else if (infoAgents.GetChosenAgent().status == 2)
                {
                    if (other == infoAgents.GetChosenAgent().transform.GetChild(0).GetComponent<Collider>())
                    {
                        if (infoAgents.FindPlayerOnNode(idNode))
                            iAmTagged = false;
                        else
                            iAmTagged = true;
                        if (infoAgents.FindEnemyOnNode(idNode))
                            enemyStayingHere = true;
                        else
                            enemyStayingHere = false;
                    }

                }
                else if (infoAgents.GetChosenAgent().status == 3)
                {
                    if (other == infoAgents.GetChosenAgent().transform.GetChild(0).GetComponent<Collider>())
                    {
                        if (infoAgents.FindPlayerOnNode(idNode))
                            iAmTagged = false;
                        else
                            iAmTagged = true;
                        if (infoAgents.FindEnemyOnNode(idNode))
                            enemyStayingHere = true;
                        else
                            enemyStayingHere = false;
                    }
                }

    }

    private void OnTriggerExit(Collider other)
    {
        if (isActivatedNode && !nodeOnFog)
            if (infoAgents.isHaveChosenAgentBool())
            {
                if (infoAgents.GetChosenAgent().status == 1)
                {
                    if (other == infoAgents.GetChosenAgent().transform.GetChild(0).GetComponent<Collider>())
                    {
                        iAmTagged = false;
                    }
                }
                else if (infoAgents.GetChosenAgent().status == 2)
                {

                    if (other == infoAgents.GetChosenAgent().transform.GetChild(0).GetComponent<Collider>())
                    {
                        iAmTagged = false;
                    }

                }

            }
    }
}
