using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleBullet : MonoBehaviour
{

    public float m_speed;

    public float Distance { get; set; }

    void Awake()
    {
        Distance = float.MaxValue;
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * m_speed * Time.deltaTime;

        Distance -= m_speed * Time.deltaTime;
        if (Distance <= 0)
        {
            Destroy(gameObject);
        }
    }
}
