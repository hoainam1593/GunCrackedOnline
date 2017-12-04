using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPSHeroProperties : MonoBehaviour {

    [Header("Transform")]
    public Transform m_rootBone;
    public Transform m_upperBodyBone;
    public Transform m_muzzle;

    [Header("Animation")]
    public Animator m_animator;

    [Header("Effect")]
    public GameObject m_muzzleFlash;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
