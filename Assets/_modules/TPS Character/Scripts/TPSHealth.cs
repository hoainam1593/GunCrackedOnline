using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TPSHealth : NetworkBehaviour, ICharacterHealth
{
    public float m_maxHealth;
    public GameObject m_healthBarPrefab;
    public Vector2 m_healthBarOffset;

    [SyncVar]
    private float m_health;
    private HealthBarController m_healthBarController;

    public TPSHeroProperties ActiveHero { get; set; }

    #region Start/Update/Destroy

    public override void OnStartLocalPlayer()
    {
        GameStats._PlayerHealth = this;
    }
    
    void Start()
    {
        m_health = m_maxHealth;
        InstantiateHealthBar();
    }

    void Update()
    {
        UpdateHealthBar();
    }

    void OnDestroy()
    {
        if (m_healthBarController != null)
        {
            Destroy(m_healthBarController.gameObject);
        }
    }

    #endregion

    #region Health Bar

    void InstantiateHealthBar()
    {
        var obj = Instantiate(m_healthBarPrefab);
        m_healthBarController = obj.GetComponent<HealthBarController>();
    }

    void UpdateHealthBar()
    {
        Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);

        m_healthBarController.SetPosition(pos.x + m_healthBarOffset.x, pos.y + m_healthBarOffset.y);
        m_healthBarController.SetNormalizedHealth(m_health / m_maxHealth);
    }

    #endregion

    #region Public functions

    public void TakeDamage(float dam)
    {
        if (!isServer)
        {
            return;
        }

        m_health -= dam;
        if (m_health <= 0)
        {
            m_health = 0;
            OnDie();
        }
    }

    public void TakeMedicine(float med)
    {
        if (!isServer)
        {
            return;
        }

        m_health += med;
        m_health = Mathf.Clamp(m_health, 0, m_maxHealth);
    }

    #endregion

    #region Die
    
    void OnDie()
    {
        RpcOnDie();
    }

    [ClientRpc]
    void RpcOnDie()
    {
        if (ActiveHero != null)
        {
            ActiveHero.m_animator.SetTrigger("Die");
        }

        if (isLocalPlayer)
        {
            MyInput.Lock();
            GameStats._EndGameMenu.Enable();
        }
    }

    #endregion

    #region Respawn

    public void OnRespawn()
    {
        CmdOnRespawn();

        MyInput.Unlock();
        GameStats._EndGameMenu.Disable();
    }

    [Command]
    void CmdOnRespawn()
    {
        var loc = NetworkManager.singleton.GetStartPosition();
        var newPlayer = Instantiate(NetworkManager.singleton.playerPrefab, loc.position, loc.rotation);

        NetworkServer.Destroy(gameObject);
        NetworkServer.ReplacePlayerForConnection(connectionToClient, newPlayer, playerControllerId);
    }

    #endregion
    
}
