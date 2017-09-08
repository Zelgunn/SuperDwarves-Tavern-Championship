using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //// Paramètres de jeu
    private int m_redScore = 0;
    private int m_blueScore = 0;
    private bool m_allPlayersConfigured = false;

    [Header("Score")]
    [SerializeField] private int m_winningScore = 5;
    [SerializeField] private GameObject m_token;

    [Header("Equipes")]
    [SerializeField] private Transform m_redTeamSpawn;
    [SerializeField] private Transform m_blueTeamSpawn;

    [Header("UI")]
    [SerializeField] private IngameMenu m_ingameMenu;
    [SerializeField] private GameUI m_gameUI;
    // Top
    [SerializeField] private ScorePanel m_blueScorePanel;
    [SerializeField] private ScorePanel m_redScorePanel;
    // BeerSlider
    [SerializeField] private PlayerBeerInfo m_playerBeerInfo;
    [SerializeField] private CentralPanel m_centralPanel;

    [Header("Musiques")]
    [SerializeField] private AudioSource m_musicSource;
    [SerializeField] private AudioClip m_introMusic;
    [SerializeField] private AudioClip m_ingameMusic;

    [Header("Voix")]
    [SerializeField] private AudioSource m_voiceSource;
    [SerializeField] private AudioClip m_welcome;
    [SerializeField] private AudioClip m_allReady;
    [SerializeField] private AudioClip m_letsGo;
    [SerializeField] private AudioClip m_redTeamWins;
    [SerializeField] private AudioClip m_blueTeamWins;
    [SerializeField] private AudioClip m_youWin;
    [SerializeField] private AudioClip m_youLose;
    [SerializeField] private AudioClip m_redTeamAboutToWin;
    [SerializeField] private AudioClip m_blueTeamAboutToWin;

    [Header("Cameras")]
    [SerializeField] private Camera m_menuCamera;
    [SerializeField] private Camera m_gameCamera;

    //// Joueurs
    // Personnages
    [Header("Personnages")]
    [SerializeField] private GameObject m_characterPrefab;

    [Header("Autres prefabs réseaux")]
    [SerializeField] private GameObject[] m_otherPrefabs;

    //// Managers
    static private GameManager s_GameManagerSingleton;
    private SoloManager m_soloManager;
    private bool m_isSolo = false;
    private bool m_isMulti = false;
    static private CharacterManager s_localPlayer;

    public void Start()
	{
        if (s_GameManagerSingleton != null)
        {
            Debug.Log("Un seul GameManager peut être utilisé à la fois");
            Destroy(gameObject);
        }

        s_GameManagerSingleton = this;

        ClientScene.RegisterPrefab(m_characterPrefab);

        Physics.gravity = new Vector3(0, -20, 0);

        m_soloManager = GetComponent<SoloManager>();

        m_voiceSource.clip = m_welcome;
        m_voiceSource.Play();
	}

    public void StartGame()
    {
        List<PlayerManager> allPlayerManagers = PlayerManager.AllPlayerManagers();
        foreach (PlayerManager playerManager in allPlayerManagers)
        {
            playerManager.InitializeIngame();
        }

        MainMenu.MainMenuSingleton().gameObject.SetActive(false);
        m_menuCamera.gameObject.SetActive(false);
        m_gameCamera.gameObject.SetActive(true);
        m_ingameMenu.gameObject.SetActive(true);
        m_isMulti = true;

        PlayIngameMusic();
        MainNetworkManager.Singleton().StopDiscovery();
        RoundManager.Singleton().ResetRoundManager();
        StartCoroutine(GameLoop());
    }

    public void StartSoloGame()
    {
        MainMenu.MainMenuSingleton().gameObject.SetActive(false);
        m_menuCamera.gameObject.SetActive(false);
        m_gameCamera.gameObject.SetActive(true);
        m_ingameMenu.gameObject.SetActive(true);
        m_isSolo = true;

        PlayIngameMusic();
        StartCoroutine(StartSoloGameEnd());
    }

    private IEnumerator StartSoloGameEnd()
    {
        // On attend la fin de la création de l'ID du joueur
        while (PlayerManager.AllPlayerManagers().Count == 0)
            yield return null;

        PlayerManager playerManager = PlayerManager.AllPlayerManagers()[0];
        playerManager.InitializeIngame();

        // On attend la fin de la création du personnage
        while (playerManager.IngameManager().Character() == null)
            yield return null;

        InitSoloGameUI();
        s_localPlayer = playerManager.IngameManager().Character();
        m_playerBeerInfo.SetPlayer(s_localPlayer);
        StartCoroutine(SoloGameLoop());
    }

    public void ExitGame()
    {
        ProfileInfo.SaveProfileInfos();
        PlayerManager.ReinitializeAllPlayerManagers();

        MainMenu.MainMenuSingleton().SetActive(true);
        MainMenu.MainMenuSingleton().OnPlayButton();
        m_gameCamera.gameObject.SetActive(false);
        m_menuCamera.gameObject.SetActive(true);
        m_ingameMenu.gameObject.SetActive(false);
        m_gameUI.gameObject.SetActive(false);

        if(m_isSolo)
        {
            m_soloManager.RemoveTokens();
        }
        else
        {
            RoundManager.Singleton().RemoveTokens();
        }

        m_isSolo = false;
        m_isMulti = false;

        PlayIntroMusic();
        StopCoroutine(GameLoop());
        MainNetworkManager.Singleton().StopHost();
        MainNetworkManager.Singleton().StopClient();
    }

    public void IncreaseBlueScore()
    {
        m_blueScore++;
        m_blueScorePanel.SetNumber(m_blueScore);
    }

    public void IncreaseRedScore()
    {
        m_redScore++;
        m_redScorePanel.SetNumber(m_redScore);
    }

    public void InitGameGUI()
    {
        m_gameUI.gameObject.SetActive(true);
        m_gameUI.SetSolo(false);
        UpdateScores();
    }

    public void InitSoloGameUI()
    {
        m_gameUI.gameObject.SetActive(true);
        m_gameUI.SetSolo(true);
    }

    static public GameManager Singleton()
    {
        return s_GameManagerSingleton;
    }

    static public GameObject CharacterPrefab()
    {
        return s_GameManagerSingleton.m_characterPrefab;
    }

    static public Transform RedTeamSpawn()
    {
        return s_GameManagerSingleton.m_redTeamSpawn;
    }

    static public Transform BlueTeamSpawn()
    {
        return s_GameManagerSingleton.m_blueTeamSpawn;
    }

    static public void PlayIntroMusic()
    {
        s_GameManagerSingleton.m_musicSource.clip = s_GameManagerSingleton.m_introMusic;
        PlayMusic();
    }

    static public void PlayIngameMusic()
    {
        s_GameManagerSingleton.m_musicSource.clip = s_GameManagerSingleton.m_ingameMusic;
        PlayMusic();
    }

    static public void PlayMusic(bool play = true)
    {
        if(play)
            s_GameManagerSingleton.m_musicSource.Play();
        else
            s_GameManagerSingleton.m_musicSource.Stop();
    }

    private IEnumerator CheckAllPlayersReady()
    {
        List<PlayerManager> allPlayerManagers = PlayerManager.AllPlayerManagers();
        while (!m_allPlayersConfigured)
        {
            yield return null;
            m_allPlayersConfigured = true;
            foreach (PlayerManager playerManager in allPlayerManagers)
            {
                m_allPlayersConfigured &= (playerManager.IngameManager().Character() != null);
                if (!m_allPlayersConfigured)
                    break;
                if(playerManager.isLocalPlayer)
                {
                    s_localPlayer = playerManager.IngameManager().Character();
                    m_playerBeerInfo.SetPlayer(s_localPlayer);
                }
            }
        }
        InitGameGUI();
    }

    private IEnumerator GameLoop()
    {
        yield return StartCoroutine(CheckAllPlayersReady());
        yield return StartCoroutine(RoundManager.Singleton().RoundLoop());

        bool playerWins = s_localPlayer.Team() == RoundManager.WinningTeam();
        m_centralPanel.SetVictorious(playerWins);

        if ((m_redScore < m_winningScore) && (m_blueScore < m_winningScore))
        {
            m_centralPanel.ShowMenu(true);
            PlayTeamWins(RoundManager.WinningTeam() == 0);
            yield return new WaitForSeconds(5);
            m_centralPanel.ShowMenu(false);
            StartCoroutine(GameLoop());
        }
        else
        {
            m_centralPanel.ShowMenu(true, true);
            PlayPlayerVictory(playerWins);
        }
    }

    private IEnumerator SoloGameLoop()
    {
        yield return StartCoroutine(m_soloManager.SoloLoop());

        StartCoroutine(SoloGameLoop());
    }

    public void UpdateScores()
    {
        m_redScorePanel.SetNumber(m_redScore);
        m_blueScorePanel.SetNumber(m_blueScore);
    }

    static public void ShowToken(bool showToken)
    {
        s_GameManagerSingleton.m_token.SetActive(showToken);
    }

    static public bool IsSolo()
    {
        return s_GameManagerSingleton.m_isSolo;
    }

    static public bool IsMulti()
    {
        return s_GameManagerSingleton.m_isMulti;
    }

    static public CharacterManager LocalCharacter()
    {
        return s_localPlayer;
    }

    static public void PlayAllReady(bool play)
    {
        if(play)
        {
            s_GameManagerSingleton.m_voiceSource.clip = s_GameManagerSingleton.m_allReady;
            s_GameManagerSingleton.m_voiceSource.Play();
        }
        else
        {
            s_GameManagerSingleton.m_voiceSource.Stop();
        }
    }

    static public void PlayLetsGo()
    {
        s_GameManagerSingleton.m_voiceSource.clip = s_GameManagerSingleton.m_letsGo;
        s_GameManagerSingleton.m_voiceSource.Play();
    }

    static public void PlayTeamWins(bool redTeamWins)
    {
        if(redTeamWins)
        {
            s_GameManagerSingleton.m_voiceSource.clip = s_GameManagerSingleton.m_redTeamWins;
        }
        else
        {
            s_GameManagerSingleton.m_voiceSource.clip = s_GameManagerSingleton.m_blueTeamWins;
        }
        s_GameManagerSingleton.m_voiceSource.Play();
    }

    static public void PlayPlayerVictory(bool playerWins)
    {
        if (playerWins)
        {
            s_GameManagerSingleton.m_voiceSource.clip = s_GameManagerSingleton.m_youWin;
        }
        else
        {
            s_GameManagerSingleton.m_voiceSource.clip = s_GameManagerSingleton.m_youLose;
        }
        s_GameManagerSingleton.m_voiceSource.Play();
    }

    static public void PlayRedTeamAboutToWin()
    {
        s_GameManagerSingleton.m_voiceSource.clip = s_GameManagerSingleton.m_redTeamAboutToWin;
        s_GameManagerSingleton.m_voiceSource.Play();
    }

    static public void PlayBlueTeamAboutToWin()
    {
        s_GameManagerSingleton.m_voiceSource.clip = s_GameManagerSingleton.m_blueTeamAboutToWin;
        s_GameManagerSingleton.m_voiceSource.Play();
    }
}