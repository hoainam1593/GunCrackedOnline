using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarController : MonoBehaviour {

    public RectTransform m_foregroundImage;
    public RectTransform m_backgroundImage;
    
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        
    }

    // Set health value between [0;1].
    public void SetNormalizedHealth(float health)
    {
        var pos = m_foregroundImage.localPosition;
        var sz = m_foregroundImage.sizeDelta;
        var oriW = m_backgroundImage.sizeDelta.x;
        var oriX = m_backgroundImage.localPosition.x;

        health = Mathf.Clamp(health, 0.0f, 1.0f);

        sz.x = health * oriW;
        pos.x = oriX - (oriW - sz.x) / 2.0f;

        m_foregroundImage.localPosition = pos;
        m_foregroundImage.sizeDelta = sz;
    }

    public void SetPosition(float x, float y)
    {
        m_foregroundImage.position = new Vector3(x, y, 0.0f);
        m_backgroundImage.position = new Vector3(x, y, 0.0f);
    }
}
