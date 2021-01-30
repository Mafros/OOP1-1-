using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarOfView : MonoBehaviour
{
    [SerializeField] private bool isOnColliderFogOfWar;
    Color prevColor;
    Color changeColor;
    // Start is called before the first frame update
    void Start()
    {
        prevColor = gameObject.GetComponent<MeshRenderer>().material.color;
        changeColor = prevColor;
        changeColor.a = 0.4f;
    }

    // Update is called once per frame
    void Update()
    {
        if (isOnColliderFogOfWar)
            if (gameObject.GetComponent<PathfindingNode>())
                gameObject.GetComponent<PathfindingNode>().mesh.material.color = changeColor;
            else
                gameObject.GetComponent<MeshRenderer>().material.color = changeColor;
        else
        {
            if (gameObject.GetComponent<PathfindingNode>())
                gameObject.GetComponent<PathfindingNode>().mesh.material.color = changeColor;
            else
                gameObject.GetComponent<MeshRenderer>().material.color = prevColor;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == GameObject.FindGameObjectWithTag("TriggerRangeOfView").GetComponent<Collider>())
            isOnColliderFogOfWar = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other == GameObject.FindGameObjectWithTag("TriggerRangeOfView").GetComponent<Collider>())
            isOnColliderFogOfWar = false;
    }



}
