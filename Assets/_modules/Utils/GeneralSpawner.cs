using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GeneralSpawner : NetworkBehaviour
{

    public Transform m_spawnLocationsParent;
    public GameObject m_spawnedObject;
    public float m_spawningRate;
    public int m_maxObjectNumber;

    [Header("Gizmos")]
    public bool m_enableGizmos;
    public Color m_lineColor;

    public override void OnStartServer()
    {
        StartCoroutine("SpawnCoroutine_Server");
    }

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator SpawnCoroutine_Server()
    {
        while (true)
        {
            SpawnAnObject_Server();

            yield return new WaitForSeconds(m_spawningRate);
        }
    }

    void SpawnAnObject_Server()
    {
        if (transform.childCount < m_maxObjectNumber)
        {
            // Instantiate object on server.
            var obj = Instantiate(m_spawnedObject, transform, false);

            // Translate object.
            var loc = GetSpawningLocation();

            var pos = obj.transform.position;
            pos.x = loc.x;
            pos.z = loc.z;
            obj.transform.position = pos;

            // Rotate object.
            var yRot = Random.Range(0.0f, 360.0f);
            obj.transform.Rotate(0, yRot, 0, Space.World);

            // Spawn object on all clients.
            NetworkServer.Spawn(obj);
        }
    }
    
    #region Utilities

    Vector3 GetSpawningLocation()
    {
        var index = Random.Range(0, m_spawnLocationsParent.childCount);
        var segment = m_spawnLocationsParent.GetChild(index);
        if (segment.childCount > 1)
        {
            var p1 = segment.GetChild(0).position;
            var p2 = segment.GetChild(1).position;

            var t = Random.Range(0.0f, 1.0f);

            return Vector3.Lerp(p1, p2, t);
        }

        return new Vector3(0, 0, 0);
    }

    void OnDrawGizmos()
    {
        if (!m_enableGizmos)
        {
            return;
        }

        Gizmos.color = m_lineColor;

        foreach (Transform segment in m_spawnLocationsParent.transform)
        {
            if (segment.childCount > 1)
            {
                var p1 = segment.GetChild(0).position;
                var p2 = segment.GetChild(1).position;

                Gizmos.DrawLine(p1, p2);
            }
        }
    }

    #endregion

}