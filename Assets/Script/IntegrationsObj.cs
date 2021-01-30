using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class IntegrationsObj : MonoBehaviour
{

    public GameObject pDoor;
    public List<int> idNodeOnStaying = new List<int>();
    public bool stateDoor;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            

        }
    }



    public void OpensDoor()
    {
        if (!stateDoor) {
            pDoor.transform.GetChild(0).GetComponent<Animation>().Play("doorL");
            pDoor.transform.GetChild(1).GetComponent<Animation>().Play("doorR");

            StartCoroutine(Timer50ms());
            stateDoor = true;
        }
        else
        {
            pDoor.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = true;
            pDoor.transform.GetChild(1).GetComponent<MeshRenderer>().enabled = true;

            pDoor.transform.GetChild(0).GetComponent<Animation>().Play("CdoorL");
            pDoor.transform.GetChild(1).GetComponent<Animation>().Play("CdoorR");
            stateDoor = false;

        }

    }



    IEnumerator Timer50ms()
    {
        yield return new WaitForSeconds(0.5f);

        if (!pDoor.transform.GetChild(0).GetComponent<Animation>().IsPlaying("doorL"))
            pDoor.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
        if (!pDoor.transform.GetChild(1).GetComponent<Animation>().IsPlaying("doorR"))
            pDoor.transform.GetChild(1).GetComponent<MeshRenderer>().enabled = false;
    }



}
