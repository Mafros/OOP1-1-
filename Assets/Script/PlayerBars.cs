using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBars : MonoBehaviour
{
    GameObject controls;
    public List<StatusPlayer> statusplayer;

    void Update()
    {
        controls = GameObject.FindGameObjectWithTag("Controls").gameObject;
        statusplayer = controls.GetComponent<TakeInfoToList>().UpdateList();
        SetValueHpBar();
    }

    void SetValueHpBar()
    {
        if (statusplayer.Count == 0)
            for (int i = 0; i < statusplayer.Count; i++)
            {
                GameObject item = gameObject.transform.GetChild(i).gameObject.transform.GetChild(0).gameObject;
                item.GetComponent<HP>().SetMaxHealth(statusplayer[i].maxhp);
                item.GetComponent<HP>().SetHealth(statusplayer[i].hp);
            }
    }
}
