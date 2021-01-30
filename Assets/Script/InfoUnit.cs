using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoUnit : MonoBehaviour
{

    public void SelectedUnitName(string nameAgent)
    {
        transform.GetChild(0).gameObject.GetComponent<Text>().text = nameAgent;
    }
}
