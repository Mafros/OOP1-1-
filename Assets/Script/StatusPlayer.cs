using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusPlayer : MonoBehaviour
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
    [Range(0, 100)] public int chanseHit = 0;

    public bool isMoved;
    public bool isShooted;
    public bool isThrowG;
    public bool isUseAbility;


    void Start()
    {
        if(healthBar != null)
        healthBar.SetMaxHealth(maxhp);


    }
    void Update()
    {
        if (hasChosen == true)
        {

            if (healthBar != null)
            {
                healthBar.SetHealth(hp);
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    TakeDamage(20);
                }
            }
        }
        else
        {

        }
        if (hp <= 0)
        {
            status = 0;
            hasChosen = false;
            gameObject.transform.position = gameObject.transform.parent.GetComponent<TakeInfoToList>().purgatory.transform.position;
            iAmStayOnNode = -1;
            GameObject.FindGameObjectWithTag("Map").GetComponent<PathfindingField>().manageQuads.ClearMap();
            GameObject.FindGameObjectWithTag("Map").GetComponent<PathfindingField>().FieldUpdate();
            GameObject.FindGameObjectWithTag("Map").GetComponent<PathfindingField>().canvasPB.transform.SetParent(null);
            gameObject.SetActive(false);
        }
    }
    void TakeDamage(int damage)
    {
        if (healthBar != null)
        {
            hp -= damage;
            healthBar.SetHealth(hp);
        }
    }

}
