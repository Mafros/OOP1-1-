using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceCordToQuads : MonoBehaviour
{

    public int x;
    public int y;
    public int sizemap;
    public string[] numbers;
    public List<GameObject> quadTagged;
    public GameObject outOfBoundsQuad;
    [HideInInspector] public List<BoxCollider> boxColTg = new List<BoxCollider>();

    // Start is called before the first frame update
    void Start()
    {
        x = gameObject.GetComponent<PathfindingField>().width;
        y = gameObject.GetComponent<PathfindingField>().height;
        sizemap = x * y;

        GameObject obs = GameObject.FindGameObjectWithTag("Obstacles").gameObject;
        for (int i = 0; i < obs.transform.childCount; i++)
            if (obs.transform.GetChild(i).CompareTag("CapObstacle"))
                boxColTg.Add(obs.transform.GetChild(i).GetComponent<BoxCollider>());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MapQuadTagging()
    {

        GameObject quadMap;
        for (int i = 0; i < sizemap; i++)
        {
            quadMap = gameObject.transform.GetChild(i).gameObject;
            if (quadMap.GetComponent<PathfindingNode>())
            {
                if (quadMap.GetComponent<PathfindingNode>().iAmTagged == true)
                    quadTagged.Add(quadMap);
                else
                    ClearMap();
            }
        }

    }

    public void ClearMap()
    {
        GameObject quadMap;
        for (int i = 0; i < sizemap; i++)
        {
            if (gameObject.transform.GetChild(i).gameObject != null)
            {
                quadMap = gameObject.transform.GetChild(i).gameObject;

                if (quadMap.GetComponent<PathfindingNode>())
                {
                    quadMap.GetComponent<PathfindingNode>().iAmTagged = false;
                    quadMap.GetComponent<PathfindingNode>().blowQuad = false;
                }
                else
                {
                    //gameObject.GetComponent<PathfindingField>().UpdateNodeState_inst();
                    quadTagged.Clear();
                    break;
                }
            }
        }
    }
}
