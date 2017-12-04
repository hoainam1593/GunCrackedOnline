using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndGameMenu : MonoBehaviour
{
    public int m_respawningTime;
    public Text m_timeText;

    private float m_timeCountDown;

    public EndGameMenu()
    {
        GameStats._EndGameMenu = this;
    }
    
    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.activeSelf)
        {
            m_timeCountDown -= Time.deltaTime;

            if (m_timeCountDown <= 0)
            {
                GameStats._PlayerHealth.OnRespawn();
                m_timeCountDown = 0;
            }

            m_timeText.text = (int)m_timeCountDown + "s";
        }
    }

    public void Enable()
    {
        m_timeCountDown = m_respawningTime;
        gameObject.SetActive(true);
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }
}
