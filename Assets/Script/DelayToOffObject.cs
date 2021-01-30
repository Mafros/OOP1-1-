using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayToOffObject : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("FindTargetsWithDelay", .2f);
    }

    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            OnOff();
        }
    }

    void OnOff()
    {
        if (gameObject.GetComponent<Collider>().enabled == true) gameObject.GetComponent<Collider>().enabled = false;
        else gameObject.GetComponent<Collider>().enabled = true;
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
