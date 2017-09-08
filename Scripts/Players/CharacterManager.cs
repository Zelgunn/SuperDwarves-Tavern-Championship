using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class CharacterManager : NetworkBehaviour
{
    protected PlayerIngameManager m_ingameManager;
    protected PlayerNetworkMove m_networkMove;
    protected PlayerNetworkSkills m_networkSkills;
    protected PlayerNetworkData m_networkData;
    protected Rigidbody m_rigidbody;

    protected PlayerBeer m_playerBeer;
    //private PlayerBeerSlider m_slider = null;

    protected List<Buff> m_buffs;
    protected bool m_stunned = false;

    // Animator
    protected Animator m_animator;
    protected int m_stunHash = Animator.StringToHash("Stunned");
    
    [SerializeField] private Renderer[] m_renderers;

    public void Start()
    {
        m_networkMove = GetComponent<PlayerNetworkMove>();
        m_networkSkills = GetComponent<PlayerNetworkSkills>();
        m_networkData = GetComponent<PlayerNetworkData>();
        m_playerBeer = GetComponent<PlayerBeer>();
        m_rigidbody = GetComponent<Rigidbody>();
        m_animator = GetComponent<Animator>();

        m_buffs = new List<Buff>();
    }

    public void Update()
    {
        UpdateBuffs();

        
    }

    public void LateUpdate()
    {
        if (hasAuthority)
        {
            Camera.main.transform.position = new Vector3(m_rigidbody.transform.position.x,
                                                      m_rigidbody.transform.position.y + 1.5f,
                                                      Camera.main.transform.position.z);
        }
    }

    [ClientRpc]
    public void RpcSetOwner(int id)
    {
        List<PlayerManager> allPlayerManagers = PlayerManager.AllPlayerManagers();
        foreach (PlayerManager playerManager in allPlayerManagers)
        {
            if(playerManager.IngameManager().Id() == id)
            {
                m_ingameManager = playerManager.IngameManager();
                m_ingameManager.SetCharacter(this);
            }
        }
    }

    public void AddBuff(Buff buff)
    {
        m_buffs.Add(buff);
    }

    protected void UpdateBuffs()
    {
        float deltaTime = Time.deltaTime;

        List<Buff> buffsToRemove = new List<Buff>();
        bool stunned = false;
        float speedIncrease = 1.0f;

        foreach(Buff buff in m_buffs)
        {
            if(buff.UpdateDuration(deltaTime) <= 0)
            {
                buffsToRemove.Add(buff);
            }
            else
            {
                switch(buff.Type())
                {
                    case Buff.BuffTypes.Stun:
                        stunned = true;
                        break;
                    case Buff.BuffTypes.SpeedUp:
                        speedIncrease *= 1.25f;
                        break;
                }
            }
        }

        UpdateStunned(stunned);
        UpdateSpeedUp(speedIncrease);

        foreach (Buff buff in buffsToRemove)
            m_buffs.Remove(buff);
    }

    protected void UpdateStunned(bool stunned)
    {
        if (stunned == m_stunned)
            return;

        SetStunned(stunned);
        if(isServer)
        {
            RpcSetStunned(stunned);
        }
        else if(hasAuthority)
        {
            CmdSetStunned(stunned);
        }
    }

    private void SetStunned(bool stunned)
    {
        m_stunned = stunned;

        m_networkMove.enabled = !m_stunned;
        m_networkSkills.enabled = !m_stunned;
        m_playerBeer.enabled = !m_stunned;

        m_animator.SetBool(m_stunHash, m_stunned);
    }

    [Command]
    private void CmdSetStunned(bool stunned)
    {
        SetStunned(stunned);
        RpcSetStunned(stunned);
    }

    [ClientRpc]
    private void RpcSetStunned(bool stunned)
    {
        SetStunned(stunned);
    }

    protected void UpdateSpeedUp(float speedIncrease)
    {
        m_networkMove.SetSpeed(speedIncrease);
    }

    virtual public void Respawn()
    {
        m_networkMove.MoveToSpawn();
        m_playerBeer.ResetBeer();
    }

    virtual public void EnableControl(bool enable, bool enableRigidbody = true)
    {
        m_networkMove.EnableRigidbody(enableRigidbody);

        m_networkMove.enabled = enable;
        m_networkSkills.enabled = enable;
        m_networkData.enabled = enable;
    }

    virtual public void DisableControl(bool disable, bool enableRigidbody = false)
    {
        EnableControl(!disable, enableRigidbody);
    }

    virtual public PlayerBeer GetPlayerBeer()
    {
        return m_playerBeer;
    }

    virtual public void EnableRenderers(bool enable = true)
    {
        foreach (Renderer renderer in m_renderers)
            renderer.enabled = enable;
    }

    public int Team()
    {
        return m_networkData.PlayerTeam();
    }
}
