using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class PlayerManager : NetworkBehaviour
{
    static private List<PlayerManager> s_allPlayerManagers = new List<PlayerManager>();

    private PlayerIngameManager m_ingameManager;
    private PlayerLobbyManager m_lobbyManager;

    public void Start()
    {
        m_ingameManager = GetComponent<PlayerIngameManager>();
        m_lobbyManager = GetComponent<PlayerLobbyManager>();
        InitTeam();
        
        s_allPlayerManagers.Add(this);
    }

    private void InitTeam()
    {
        int redCount = 0, blueCount = 0;
        List<PlayerManager> allPlayerManagers = PlayerManager.AllPlayerManagers();
        foreach (PlayerManager playerManager in allPlayerManagers)
        {
            if (playerManager.Team() == 0)
                redCount++;
            else
                blueCount++;
        }

        if (redCount <= blueCount)
            m_lobbyManager.SetTeam(0);
        else
            m_lobbyManager.SetTeam(1);
    }

    public int Team()
    {
        return m_lobbyManager.Team();
    }

    public PlayerLobbyManager LobbyManager()
    {
        return m_lobbyManager;
    }

    public PlayerIngameManager IngameManager()
    {
        return m_ingameManager;
    }

    public void InitializeIngame()
    {
        m_ingameManager.InitializeCharacter();
        m_lobbyManager.enabled = false;
    }

    static public List<PlayerManager> AllPlayerManagers()
    {
        return s_allPlayerManagers;
    }

    static public void ReinitializeAllPlayerManagers()
    {
        s_allPlayerManagers = new List<PlayerManager>();
    }
}