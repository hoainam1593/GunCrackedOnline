using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TPSHeroesManager : NetworkBehaviour
{
    [SyncVar(hook = "OnChangedActiveHeroId")]
    private int m_activeHeroId = 0;
    
    public override void OnStartLocalPlayer()
    {
        CmdSetActiveHero(ShowroomController.s_selectedHero);
    }

    public override void OnStartClient()
    {
        SetActiveHeroForUsers(SetActiveHero(m_activeHeroId));
    }

    [Command]
    void CmdSetActiveHero(int id)
    {
        m_activeHeroId = id;
    }

    void OnChangedActiveHeroId(int id)
    {
        SetActiveHeroForUsers(SetActiveHero(id));
    }
    
    void SetActiveHeroForUsers(TPSHeroProperties hero)
    {
        GetComponent<TPSMovement>().ActiveHero = hero;
        GetComponent<TPSShooting>().ActiveHero = hero;
        GetComponent<TPSHealth>().ActiveHero = hero;
    }
    
    TPSHeroProperties SetActiveHero(int id)
    {
        TPSHeroProperties hero = null;

        for (int i = 0; i < transform.childCount; i++)
        {
            var obj = transform.GetChild(i).gameObject;
            if (i == id)
            {
                obj.SetActive(true);
                hero = obj.GetComponent<TPSHeroProperties>();
            }
            else
            {
                obj.SetActive(false);
            }
        }

        return hero;
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}