using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyInput
{
    private static bool m_isLock = false;
    private static Rect m_screenRect = new Rect();
    private static Vector3 m_prevMousePos;

    public static void Lock()
    {
        m_isLock = true;
    }

    public static void Unlock()
    {
        m_isLock = false;
    }

    public static float GetAxis(string axisName)
    {
        if (m_isLock)
        {
            return 0.0f;
        }
        else
        {
            return Input.GetAxis(axisName);
        }
    }

    public static bool GetButton(string buttonName)
    {
        if (m_isLock)
        {
            return false;
        }
        else
        {
            return Input.GetButton(buttonName);
        }
    }

    public static bool GetButtonDown(string buttonName)
    {
        if (m_isLock)
        {
            return false;
        }
        else
        {
            return Input.GetButtonDown(buttonName);
        }
    }

    public static bool GetButtonUp(string buttonName)
    {
        if (m_isLock)
        {
            return false;
        }
        else
        {
            return Input.GetButtonUp(buttonName);
        }
    }

    public static Vector3 GetMousePosition()
    {
        m_screenRect.x = m_screenRect.y = 0;
        m_screenRect.width = Screen.width;
        m_screenRect.height = Screen.height;

        var pos = Input.mousePosition;

        if (!m_isLock && m_screenRect.Contains(pos))
        {
            m_prevMousePos = pos;
        }

        if (m_isLock || !m_screenRect.Contains(pos))
        {
            return m_prevMousePos;
        }
        else
        {
            return pos;
        }
    }
}
