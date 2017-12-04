using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowroomController : MonoBehaviour
{
    public static int s_selectedHero = 0;

    public int m_heroesCount;
    public float m_turningSpeed;

    private float m_turningAngle;
    private float m_targetRot;
    private Vector3 m_currentRot;

    // Use this for initialization
    void Start()
    {
        if (m_heroesCount <= 1)
        {
            m_turningAngle = 0;
        }
        else
        {
            m_turningAngle = 360.0f / m_heroesCount;
        }

        m_targetRot = transform.eulerAngles.y;
        m_currentRot = transform.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        var t = m_turningSpeed * Time.deltaTime;
        m_currentRot.y = Mathf.Lerp(m_currentRot.y, m_targetRot, t);
        transform.eulerAngles = m_currentRot;
    }

    public void TurnLeft()
    {
        s_selectedHero--;
        if (s_selectedHero < 0)
        {
            s_selectedHero = m_heroesCount - 1;
        }

        m_targetRot -= m_turningAngle;
    }

    public void TurnRight()
    {
        s_selectedHero++;
        if (s_selectedHero >= m_heroesCount)
        {
            s_selectedHero = 0;
        }

        m_targetRot += m_turningAngle;
    }
}
