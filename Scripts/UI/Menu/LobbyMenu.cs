using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class LobbyMenu : MonoBehaviour
{
    [SerializeField] private MainNetworkManager m_networkManager;

    [Header("Emplacements")]
    [SerializeField] private GameObject m_redSlotsContainer;
    [SerializeField] private GameObject m_blueSlotsContainer;
    [SerializeField] private GameObject m_slotPrefab;
    [SerializeField] private Sprite[] m_characterPortraits;

    [Header("Panneau inférieur")]
    [SerializeField] private Image m_startButton;
    [SerializeField] private Button m_swapTeamButton;

    [Header("Compte à rebours")]
    [SerializeField] private Counter m_counter;

    [Header("Images")]
    [SerializeField] private Sprite m_readyOff;
    [SerializeField] private Sprite m_readyOn;

    List<PlayerManager> m_displayedPlayers;
    private PlayerManager m_localPlayerManager;
    private PlayerLobbyManager m_localPlayerLobbyManager;
    private List<PlayerLobbySlot> m_redSlots;
    private List<PlayerLobbySlot> m_blueSlots;

    private float m_startingTimer = -1;
    private bool m_gameIsStarting = false;

    private bool m_isServer = false;

    public void Start()
    {
        m_displayedPlayers = new List<PlayerManager>();
        m_redSlots = new List<PlayerLobbySlot>();
        m_blueSlots = new List<PlayerLobbySlot>();
    }

    public void Update()
    {
        ScanForNewPlayerManager();
        CheckAllPlayersAreReady();
        UpdateTimer();
    }

    /// <summary>
    /// Effectue un scan de tous les PlayerManager ayant été créés. Si un PlayerManager scanné n'est pas trouvé
    /// dans le dictionnaire du menu, on considère qu'il est nouveau et qu'il faut l'ajouter au dictionnaire.
    /// En l'ajoutant au dictionnaire, le PlayerManager est paramétré et son emplacement dans le Lobby créé.
    /// </summary>
    public void ScanForNewPlayerManager()
    {
        List<PlayerManager> allPlayerManagers = PlayerManager.AllPlayerManagers();
        // On suppose que si les listes sont de la même taille, on a pas perdu de joueur ou un nouveau joueur.
        // Cela dit, un cas extrême serait qu'un joueur se déconnecte et qu'un autre se connecte à l'exact même moment
        // Afin de rendre cette condition invalide...
        if (m_displayedPlayers.Count == allPlayerManagers.Count)
            return;

        foreach (PlayerManager playerManager in allPlayerManagers)
        {
            if(!m_displayedPlayers.Contains(playerManager))
            {
                GameObject newSlot = Instantiate(m_slotPrefab);
                PlayerLobbySlot lobbySlot = newSlot.GetComponent<PlayerLobbySlot>();
                m_displayedPlayers.Add(playerManager);
                int teamCount;

                if (playerManager.Team() == 0)
                {
                    teamCount = m_redSlots.Count;
                    newSlot.transform.SetParent(m_redSlotsContainer.transform, false);
                    m_redSlots.Add(lobbySlot);
                }
                else
                {
                    teamCount = m_blueSlots.Count;
                    newSlot.transform.SetParent(m_blueSlotsContainer.transform, false);
                    m_blueSlots.Add(lobbySlot);
                }

                RectTransform newSlotRectT = newSlot.GetComponent<RectTransform>();
                Vector3 offset = new Vector3(0, (newSlotRectT.rect.height + 5) * teamCount, 0);
                newSlotRectT.localPosition = newSlotRectT.localPosition - offset;
                
                playerManager.LobbyManager().Initialize(lobbySlot, this);

                if (playerManager.isLocalPlayer)
                {
                    m_localPlayerManager = playerManager;
                    m_localPlayerLobbyManager = m_localPlayerManager.LobbyManager();
                }
            }
        }

        if (allPlayerManagers.Count > 0)
        {
            m_startButton.GetComponent<Button>().interactable = true;
            if (m_localPlayerLobbyManager != null)
            {
                m_swapTeamButton.interactable = !m_localPlayerLobbyManager.Ready();
            }
        }
    }

    /// <summary>
    /// Met à jour le timer avant de commencer la partie.
    /// Si le timer arrive au bout, la partie est directement chargée.
    /// </summary>
    private void UpdateTimer()
    {
        if (m_gameIsStarting)
        {
            m_startingTimer -= Time.deltaTime;
            m_counter.UpdateImage(m_startingTimer);
            if (m_startingTimer <= 0)
            {
                GameManager.Singleton().StartGame();
                return;
            }
            UpdateStartButton();
        }
        else
        {
            m_counter.TurnOff();
        }
    }

    public void InitLobby(bool isServer = false)
    {
        m_isServer = isServer;

        if (m_isServer)
            m_networkManager.StartDiscoveryAsServer();
        else
            m_networkManager.StartDiscoveryAsClient();
    }

    public void ExitLobby()
    {
        m_networkManager.StopDiscovery();

        foreach (PlayerLobbySlot slot in m_redSlots)
            Destroy(slot.gameObject);
        m_redSlots.Clear();

        foreach (PlayerLobbySlot slot in m_blueSlots)
            Destroy(slot.gameObject);
        m_blueSlots.Clear();
        m_displayedPlayers.Clear();
    }

    private void UpdateStartButton()
    {
        if((m_localPlayerLobbyManager != null) && (m_localPlayerLobbyManager.Ready()))
        {
            if(!m_gameIsStarting)
            {
                m_startButton.sprite = m_readyOn;
            }
            else
            {
                m_startButton.sprite = m_readyOn;
            }
        }
        else
        {
            m_startButton.sprite = m_readyOff;
        }
    }
    
    public void OnReadyButtonPressed()
    {
        m_localPlayerLobbyManager.InvertReady();

        m_swapTeamButton.interactable = !m_localPlayerLobbyManager.Ready();
    }

    public void UpdateSlot(PlayerLobbySlot slot, PlayerLobbyManager lobbyManager)
    {
        slot.SetReadyMarkerTexture(lobbyManager.Ready());
    }

    public void CheckAllPlayersAreReady()
    {
        List<PlayerManager> allPlayerManagers = PlayerManager.AllPlayerManagers();
        bool allPlayersAreReady = (allPlayerManagers.Count > 0);
        foreach (PlayerManager playerManager in allPlayerManagers)
        {
            allPlayersAreReady &= playerManager.LobbyManager().Ready();
        }

        if (allPlayersAreReady)
        {
            if(!m_gameIsStarting)
            {
                m_startingTimer = 5;
                m_gameIsStarting = true;

                GameManager.PlayAllReady(true);
            }
        }
        else
        {
            m_startingTimer = -1;

            if(m_gameIsStarting)
            {
                GameManager.PlayAllReady(false);
            }

            m_gameIsStarting = false;
            UpdateStartButton();
        }
    }

    public void OnChangeTeamButton()
    {
        m_localPlayerLobbyManager.SwapTeam();
    }

    public void SetSlotTeam(PlayerLobbySlot slot, int team)
    {
        RectTransform baseRedRect = null;
        RectTransform baseBlueRect = null;
        if(m_redSlots.Count > 0)
            baseRedRect = m_redSlots[0].GetComponent<RectTransform>();
        if(m_blueSlots.Count > 0)
            baseBlueRect = m_blueSlots[0].GetComponent<RectTransform>();

        int currentIndex = -1;
        if(team == 0)
        {
            if (m_redSlots.Contains(slot))
                return;

            currentIndex = m_blueSlots.IndexOf(slot);
            m_redSlots.Add(slot);
            m_blueSlots.Remove(slot);
            slot.transform.SetParent(m_redSlotsContainer.transform, false);
            if (baseRedRect == null)
            {
                baseRedRect = slot.GetComponent<RectTransform>();
                baseRedRect.localPosition += new Vector3(0, (baseRedRect.rect.height + 5) * currentIndex, 0);
            }
        }
        else
        {
            if (m_blueSlots.Contains(slot))
                return;

            currentIndex = m_redSlots.IndexOf(slot);
            m_blueSlots.Add(slot);
            m_redSlots.Remove(slot);
            slot.transform.SetParent(m_blueSlotsContainer.transform, false);

            if (baseBlueRect == null)
            {
                baseBlueRect = slot.GetComponent<RectTransform>();
                baseBlueRect.localPosition += new Vector3(0, (baseBlueRect.rect.height + 5) * currentIndex, 0);
            }
        }

        
        Vector3 offset = new Vector3(0,0,0);
        for(int i=0; i<m_redSlots.Count; i++)
        {

            m_redSlots[i].GetComponent<RectTransform>().localPosition = baseRedRect.localPosition - offset;
            offset.y += baseRedRect.rect.height + 5;
        }

        offset = new Vector3(0, 0, 0);
        for (int i = 0; i < m_blueSlots.Count; i++)
        {
            m_blueSlots[i].GetComponent<RectTransform>().localPosition = baseBlueRect.localPosition - offset;
            offset.y += baseBlueRect.rect.height + 5;
        }
    }
}