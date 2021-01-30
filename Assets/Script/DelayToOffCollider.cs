using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayToOffCollider : MonoBehaviour
{

    void OnEnable()
    {
        StartCoroutine(StartDelay(0.3f));
    }

    IEnumerator StartDelay(float value)
    {

        yield return new WaitForSeconds(value);
        gameObject.SetActive(false);
    }

}
