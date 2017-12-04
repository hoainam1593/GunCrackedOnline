using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PickableHealthBox : NetworkBehaviour
{

    public float m_healingValue;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (!isServer)
        {
            return;
        }

        if (other.CompareTag("Player"))
        {
            var player = other.gameObject.GetComponent<TPSHealth>();
            player.TakeMedicine(m_healingValue);

            NetworkServer.Destroy(gameObject);
        }
    }
}