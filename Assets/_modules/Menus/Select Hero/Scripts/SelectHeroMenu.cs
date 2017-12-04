using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectHeroMenu : MonoBehaviour
{
    public ShowroomController m_showroom;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnSelectButtonClicked()
    {
        SceneManager.LoadScene(1);
    }

    public void OnLeftButtonClicked()
    {
        m_showroom.TurnLeft();
    }

    public void OnRightButtonClicked()
    {
        m_showroom.TurnRight();
    }
}
