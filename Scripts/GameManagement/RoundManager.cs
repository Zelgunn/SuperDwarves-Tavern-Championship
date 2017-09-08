using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RoundManager : NetworkBehaviour
{
    static private RoundManager s_singleton;
    // Score (Round)
    private int s_redTokens = 0;
    private int s_blueTokens = 0;
    private int m_redTokensToWin = 0;
    private int m_blueTokensToWin = 0;
    static private int s_winningTeam = -1;
    private int m_redTeamPlayersCount = 0;
    private int m_blueTeamPlayersCount = 0;

    //// Joueurs
    // Local 
    private GameObject m_localPlayer;

    //// Environnement
    // Tables
    private ThirstyTable[] m_thirtyTables;

    [SerializeField] private Counter m_counter;
    [SerializeField] private TokenCounter m_redTokenCounter;
    [SerializeField] private TokenCounter m_blueTokenCounter;

    private bool m_letsGoPlayed = false;

    public void Start()
    {
        s_singleton = this;
    }

    public void ResetRoundManager()
    {
        m_letsGoPlayed = false;
    }

    public IEnumerator RoundLoop()
    {
        yield return StartCoroutine(RoundStarting());

        yield return StartCoroutine(RoundPlaying());

        if (WinningTeam() == 0)
        {
            GameManager.Singleton().IncreaseRedScore();
        }
        else
        {
            GameManager.Singleton().IncreaseBlueScore();
        }
    }

    private IEnumerator RoundStarting()
    {
        if(m_thirtyTables == null)
            m_thirtyTables = GameObject.FindObjectsOfType<ThirstyTable>();
        DisablePlayersControl();
        ResetRoundValues();
        RespawnPlayers();
        ResetTokens();

        m_counter.gameObject.SetActive(true);
        for (int i = 4; i >= 0; i--)
        {
            m_counter.UpdateImage(i);
            yield return new WaitForSeconds(1);
        }

        if(!m_letsGoPlayed)
        {
            m_letsGoPlayed = true;
            GameManager.PlayLetsGo();
        }

        m_counter.gameObject.SetActive(false);
    }

    private IEnumerator RoundPlaying()
    {
        EnablePlayersControl();
        while ((s_redTokens < m_redTokensToWin) && (s_blueTokens < m_blueTokensToWin))
        {
            yield return null;
        }

        if (s_redTokens >= m_redTokensToWin)
        {
            s_winningTeam = 0;
        }
        else
        {
            s_winningTeam = 1;
        }

        DisablePlayersControl();
    }

    public void ResetRoundValues()
    {
        s_redTokens = 0;
        s_blueTokens = 0;
        s_winningTeam = -1;
        foreach(ThirstyTable table in m_thirtyTables)
        {
            table.ResetBeer();
        }
    }

    static public void RespawnPlayers()
    {
        List<PlayerManager> allPlayerManagers = PlayerManager.AllPlayerManagers();
        foreach (PlayerManager playerManager in allPlayerManagers)
            playerManager.IngameManager().RespawnPlayer();
    }

    static public void EnablePlayersControl(bool enableRigidbody = true)
    {
        List<PlayerManager> allPlayerManagers = PlayerManager.AllPlayerManagers();
        foreach (PlayerManager playerManager in allPlayerManagers)
            playerManager.IngameManager().EnablePlayerControl(true);
    }

    static public void DisablePlayersControl()
    {
        List<PlayerManager> allPlayerManagers = PlayerManager.AllPlayerManagers();
        foreach (PlayerManager playerManager in allPlayerManagers)
            playerManager.IngameManager().DisablePlayerControl(true, false);
    }

    public void ResetTokens()
    {
        m_redTeamPlayersCount = 0;
        m_blueTeamPlayersCount = 0;

        List<PlayerManager> allPlayerManagers = PlayerManager.AllPlayerManagers();
        foreach (PlayerManager playerManager in allPlayerManagers)
        {
            if (playerManager.Team() == 0)
                m_redTeamPlayersCount++;
            else
                m_blueTeamPlayersCount++;
        }

        m_redTokensToWin = 2 + 3 * m_redTeamPlayersCount;
        m_blueTokensToWin = 2 + 3 * m_blueTeamPlayersCount;

        m_redTokenCounter.gameObject.SetActive(m_redTeamPlayersCount != 0);
        m_blueTokenCounter.gameObject.SetActive(m_blueTeamPlayersCount != 0);

        m_redTokenCounter.SetMaxTokenCount(m_redTokensToWin);
        m_blueTokenCounter.SetMaxTokenCount(m_blueTokensToWin);
    }

    static public int WinningTeam()
    {
        return s_winningTeam;
    }
    
    static public void IncreaseTokens(CharacterManager characterManager)
    {
        if(s_singleton.isServer)
        {
            s_singleton.IncreaseTeamTokensServer(characterManager.Team());
        }
    }

    [Server]
    private void IncreaseTeamTokensServer(int team)
    {
        if (team == 0)
        {
            s_redTokens++;
            RpcSetRedTokens(s_redTokens);
            SetRedTokens(s_redTokens);
        }
        else
        { 
            s_blueTokens++;
            RpcSetBlueTokens(s_blueTokens);
            SetBlueTokens(s_blueTokens);
        }
    }

    [ClientRpc]
    private void RpcSetRedTokens(int count)
    {
        SetRedTokens(count);
    }

    [ClientRpc]
    private void RpcSetBlueTokens(int count)
    {
        SetBlueTokens(count);
    }

    private void SetRedTokens(int count)
    {
        s_redTokens = count;

        if (s_redTokens == (m_redTeamPlayersCount * 2 + 2))
        {
            GameManager.PlayRedTeamAboutToWin();
        }

        m_redTokenCounter.SetTokenOnCount(s_redTokens);
    }

    private void SetBlueTokens(int count)
    {
        s_blueTokens = count;

        if (s_blueTokens == (m_blueTeamPlayersCount * 2 + 2))
        {
            GameManager.PlayBlueTeamAboutToWin();
        }

        m_blueTokenCounter.SetTokenOnCount(s_blueTokens);
    }

    public void RemoveTokens()
    {
        m_blueTokenCounter.RemoveTokens();
        m_redTokenCounter.RemoveTokens();
    }

    static public RoundManager Singleton()
    {
        return s_singleton;
    }
}