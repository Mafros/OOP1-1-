using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleCaptionMesh : MonoBehaviour
{

    Material prevMat;

    public bool isShotThrough;

    public bool OHPercentToProtect;

    // Start is called before the first frame update
    void Start()
    {
        prevMat = gameObject.GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnMouseEnter()
    {
        if (gameObject.GetComponent<MeshRenderer>())
        {
            //gameObject.GetComponent<Renderer>().material = new Material(Shader.Find("FX/Flare"));
            gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    private void OnMouseExit()
    {
        if (gameObject.GetComponent<MeshRenderer>())
        {
            //gameObject.GetComponent<Renderer>().material = prevMat;
            gameObject.GetComponent<MeshRenderer>().enabled = true;
        }
    }

}
