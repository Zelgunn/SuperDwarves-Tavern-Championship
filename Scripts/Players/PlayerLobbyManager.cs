using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class PlayerLobbyManager : NetworkBehaviour
{
    private PlayerLobbySlot m_slot;
    private LobbyMenu m_menu;

    [SyncVar] private int m_team = 0;
    [SyncVar] private string m_playerName;
    [SyncVar] private bool m_ready = false;

    public void Start()
    {
        //m_playerManager = GetComponent<PlayerManager>();
        if(isLocalPlayer)
        {
            SetName(ProfileInfo.SelectedProfileInfo().ProfileName());
        }
    }
	
	public void Update ()
    {
	
	}

    public void Initialize(PlayerLobbySlot slot, LobbyMenu menu)
    {
        m_slot = slot;
        m_menu = menu;
        m_menu.SetSlotTeam(m_slot, m_team);
        m_slot.SetPlayerName(m_playerName);
        m_slot.SetReadyMarkerTexture(m_ready);
    }

    // Team
    public int Team()
    {
        return m_team;
    }

    public void SetTeam(int team)
    {
        if (m_team == team)
            return;

        m_team = team;
        if((m_slot != null) && (m_menu != null))
        {
            m_menu.SetSlotTeam(m_slot, m_team);
        }

        if (isServer)
        {
            RpcSetTeam(team);
        }
        else if (isLocalPlayer)
        {
            CmdSetTeam(team);
        }
    }

    public void SwapTeam()
    {
        SetTeam((m_team + 1)%2);
    }

    [Command]
    private void CmdSetTeam(int team)
    {
        SetTeam(team);
    }

    [ClientRpc]
    private void RpcSetTeam(int team)
    {
        if (isLocalPlayer)
            return;
        SetTeam(team);
    }

    // Nom
    public string PlayerName()
    {
        return m_playerName;
    }

    public void SetName(string playerName)
    {
        if (m_playerName == playerName)
            return;

        m_playerName = playerName;
        if(m_slot != null)
            m_slot.SetPlayerName(m_playerName);

        if (isServer)
        {
            RpcSetName(playerName);
        }
        else if (isLocalPlayer)
        {
            CmdSetName(playerName);
        }
    }

    [Command]
    private void CmdSetName(string playerName)
    {
        SetName(playerName);
    }

    [ClientRpc]
    private void RpcSetName(string playerName)
    {
        if (isLocalPlayer)
            return;
        SetName(playerName);
    }

    // Prêt
    public bool Ready()
    {
        return m_ready;
    }

    public void SetReady(bool ready)
    {
        if (m_ready == ready)
            return;

        m_ready = ready;
        m_slot.SetReadyMarkerTexture(m_ready);

        if(isServer)
        {
            RpcSetReady(ready);
        }
        else if(isLocalPlayer)
        {
            CmdSetReady(ready);
        }
    }

    public void InvertReady()
    {
        SetReady(!m_ready);
    }

    [Command]
    private void CmdSetReady(bool ready)
    {
        SetReady(ready);
    }

    [ClientRpc]
    private void RpcSetReady(bool ready)
    {
        if (isLocalPlayer)
            return;
        SetReady(ready);
    }
}
