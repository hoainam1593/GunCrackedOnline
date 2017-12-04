using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TPSShooting : NetworkBehaviour
{
    public GameObject m_bullet;
    public float m_fireRate;
    public float m_fireRange;
    public float m_fireDamage;
    public float m_coneAngle;

    private DecalsManager m_decalsManager;

    public TPSHeroProperties ActiveHero { get; set; }

    // Use this for initialization
    void Start()
    {
        m_decalsManager = GetComponent<DecalsManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        if (MyInput.GetButtonDown("Fire1"))
        {
            CmdBeginShoot();
        }
        if (MyInput.GetButtonUp("Fire1"))
        {
            CmdEndShoot();
        }
    }
    
    #region On Server

    [Command]
    void CmdBeginShoot()
    {
        RpcBeginShoot();
        StartCoroutine("FireCoroutine_Server");
    }

    [Command]
    void CmdEndShoot()
    {
        RpcEndShoot();
        StopCoroutine("FireCoroutine_Server");
    }

    IEnumerator FireCoroutine_Server()
    {
        while (true)
        {
            FireOneBullet_Server();
            yield return new WaitForSeconds(m_fireRate);
        }
    }

    void FireOneBullet_Server()
    {
        if (ActiveHero == null)
        {
            return;
        }

        float distance = float.MaxValue;

        // Raycasting.
        RaycastHit hitInfo;
        if (Physics.Raycast(ActiveHero.m_muzzle.position, ActiveHero.m_muzzle.forward, out hitInfo, m_fireRange))
        {
            // Hurt enemy if hit enemy.
            var enemy = hitInfo.collider.GetComponent<ICharacterHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(m_fireDamage);
            }

            // Create decal.
            var decal = m_decalsManager.SpawnDecal(hitInfo);
            if (decal != null)
            {
                NetworkServer.Spawn(decal);
            }

            // Calculate the distance between gun and obstacle.
            distance = hitInfo.distance;
        }

        // Spawn bullet.
        var coneRandomRotation = Quaternion.Euler(
            Random.Range(-m_coneAngle, m_coneAngle),
            Random.Range(-m_coneAngle, m_coneAngle),
            0);
        var bullet = Instantiate(m_bullet, ActiveHero.m_muzzle.position, ActiveHero.m_muzzle.rotation * coneRandomRotation);
        bullet.GetComponent<SimpleBullet>().Distance = distance;
        
        NetworkServer.Spawn(bullet);
    }

    #endregion
    
    #region On Client

    [ClientRpc]
    void RpcBeginShoot()
    {
        if (ActiveHero == null)
        {
            return;
        }

        ActiveHero.m_muzzleFlash.SetActive(true);
        ActiveHero.m_animator.SetBool("IsFiring", true);
        StartCoroutine("FireCoroutine_Client");
    }

    [ClientRpc]
    void RpcEndShoot()
    {
        if (ActiveHero == null)
        {
            return;
        }

        ActiveHero.m_muzzleFlash.SetActive(false);
        ActiveHero.m_animator.SetBool("IsFiring", false);
        StopCoroutine("FireCoroutine_Client");
    }

    IEnumerator FireCoroutine_Client()
    {
        while (true)
        {
            FireOneBullet_Client();
            yield return new WaitForSeconds(m_fireRate);
        }
    }

    void FireOneBullet_Client()
    {
        if (ActiveHero == null)
        {
            return;
        }

        ActiveHero.m_animator.Play(0, 1, 0.0f);
    }

    #endregion

}
