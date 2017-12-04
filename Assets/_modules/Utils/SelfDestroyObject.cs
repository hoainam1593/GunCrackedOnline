using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestroyObject : MonoBehaviour {

	public float m_lifeTime;

	// Use this for initialization
	void Start () {
		Destroy (gameObject, m_lifeTime);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
