using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public static void QuitGame () {
		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#else
		Application.Quit();
		#endif
	}

	public static void Log(Vector3 v) {
        var str = String.Format("({0}; {1}; {2})", v.x, v.y, v.z);
		Debug.Log (str);
	}

    public static void Log(string prefix, Vector3 v)
    {
        var str = String.Format("{0}({1}; {2}; {3})", prefix, v.x, v.y, v.z);
        Debug.Log(str);
    }

    public static Vector3 ScreenPointToWorldPointOnPlane(Vector3 screenPoint, Plane plane, Camera camera)
    {
        float dist;
        var ray = camera.ScreenPointToRay(screenPoint);

        plane.Raycast(ray, out dist);

        return ray.GetPoint(dist);
    }
}
