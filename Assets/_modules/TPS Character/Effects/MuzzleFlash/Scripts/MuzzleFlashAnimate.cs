using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleFlashAnimate : MonoBehaviour {

    public float m_minScale;
    public float m_maxScale;

    public float m_minLightIntensity;
    public float m_maxLightIntensity;

    private Light m_light;

	// Use this for initialization
	void Start () {
        m_light = GetComponent<Light>();
	}
	
	// Update is called once per frame
	void Update () {
        // Animate transform.
        var rot = transform.localEulerAngles;
        rot.z = Random.Range(0, 90.0f);

        transform.localScale = Vector3.one * Random.Range(m_minScale, m_maxScale);
        transform.localEulerAngles = rot;

        // Animate light.
        m_light.intensity = Random.Range(m_minLightIntensity, m_maxLightIntensity);
    }
}
