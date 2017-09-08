using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainMenu : MonoBehaviour
{
    [Header("Menus")]
    [SerializeField] private GameObject m_startMenu;
    [SerializeField] private GameObject m_playMenu;
    [SerializeField] private GameObject m_hostMenu;
    [SerializeField] private GameObject m_joinMenu;
    [SerializeField] private GameObject m_soloMenu;
    [SerializeField] private GameObject m_lobbyMenu;
    [SerializeField] private GameObject m_profileMenu;
    [SerializeField] private GameObject m_newProfileMenu;
    [SerializeField] private GameObject m_modifyProfileMenu;
    [SerializeField] private GameObject m_statsMenu;
    [Header("Portes")]
    [SerializeField] private GateManager m_rightGateManager;
    [SerializeField] private GateManager m_leftGateManager;
    [SerializeField] private GameObject m_gates;
    private bool m_gatesOnTheMove = false;

    static private MainMenu s_mainMenuSingleton;
    private List<GameObject> m_allMenus;
    private GameObject m_currentMenu;
    private float previousScreenHeight = 1080;

    public void Start()
    {
        if(s_mainMenuSingleton != null)
        {
            Debug.Log("Un seul StartupMenuScript peut être utilisé à la fois");
            Destroy(this);
        }

        s_mainMenuSingleton = this;
        m_allMenus = new List<GameObject>();

        m_allMenus.Add(m_startMenu);
        m_allMenus.Add(m_playMenu);
        m_allMenus.Add(m_hostMenu);
        m_allMenus.Add(m_joinMenu);
        m_allMenus.Add(m_lobbyMenu);
        m_allMenus.Add(m_profileMenu);
        m_allMenus.Add(m_newProfileMenu);
        m_allMenus.Add(m_modifyProfileMenu);
        m_allMenus.Add(m_statsMenu);

        ProfileInfo.LoadProfileInfos();
    }

    public void Update()
    {
        if(Screen.height != previousScreenHeight)
        {
            Vector3 scale = m_gates.transform.localScale;
            scale *= Screen.height / previousScreenHeight;
            previousScreenHeight = Screen.height;
            m_gates.transform.localScale = scale;
        }

        if(Input.GetButtonDown("Cancel"))
        {
            if(m_currentMenu == m_startMenu)
            {
                OnExitButton();
            }
            else if(m_currentMenu == m_playMenu)
            {
                OnBack("Play");
            }
            else if(m_currentMenu == m_lobbyMenu)
            {
                OnBack("Lobby");
            }
            else if(m_currentMenu == m_profileMenu)
            {
                OnBack("Profile");
            }
            else if(m_currentMenu == m_newProfileMenu)
            {
                OnBack("NewProfile");
            }
            else if (m_currentMenu == m_modifyProfileMenu)
            {
                OnBack("ModifyProfile");
            }
        }
    }

    public void OnExitButton()
    {
        Application.Quit();
        Debug.Log("Impossible de quitter le jeu dans l'IDE de Unity !");
    }

    public bool SetCurrentMenu(GameObject currentMenu)
    {
        if (m_gatesOnTheMove)
            return false;

        StartCoroutine(SetCurrentMenuCoroutine(currentMenu));
        return true;
    }

    public IEnumerator SetCurrentMenuCoroutine(GameObject currentMenu)
    {
        // Début - Fermeture
        m_gatesOnTheMove = true;
        StartCoroutine(m_rightGateManager.CloseGate());
        yield return StartCoroutine(m_leftGateManager.CloseGate());

        // Les portes sont fermées - Mise à jour du menu
        m_currentMenu = currentMenu;
        foreach (GameObject menu in m_allMenus)
            menu.SetActive(menu == m_currentMenu);

        // Les portes sont fermées - Ré-Ouverture des portes
        StartCoroutine(m_rightGateManager.OpenGate());
        yield return StartCoroutine(m_leftGateManager.OpenGate());
        m_gatesOnTheMove = false;
    }

    public void OnPlayButton()
    {
        SetCurrentMenu(m_playMenu);
    }

    public void OnBack(string currentMenu)
    {
        switch(currentMenu)
        {
            case "Play":
                SetCurrentMenu(m_startMenu);
                break;

            case "Lobby":
                if (SetCurrentMenu(m_playMenu))
                {
                    MainNetworkManager.Singleton().StopHost();
                    MainNetworkManager.Singleton().StopClient();
                    m_lobbyMenu.GetComponent<LobbyMenu>().ExitLobby();
                    PlayerManager.ReinitializeAllPlayerManagers();
                }
                break;

            case "Profile":
                SetCurrentMenu(m_playMenu);
                break;

            case "NewProfile":
            case "ModifyProfile":
            case "Stats":
                SetCurrentMenu(m_profileMenu);
                break;
        }
    }

    public void OnSoloGame()
    {
        if (!ProfileInfo.ProfileInfosLoaded())
        {
            SetCurrentMenu(m_newProfileMenu);
            return;
        }

        if (SetCurrentMenu(m_soloMenu))
        {
            MainNetworkManager.Singleton().StopHost();
            MainNetworkManager.Singleton().StopClient();

            MainNetworkManager.Singleton().StartHost();
            GameManager.Singleton().StartSoloGame();
        }
    }

    public void OnHostGame()
    {
        if(!ProfileInfo.ProfileInfosLoaded())
        {
            SetCurrentMenu(m_newProfileMenu);
            return;
        }

        if(SetCurrentMenu(m_lobbyMenu))
        {
            m_lobbyMenu.GetComponent<LobbyMenu>().InitLobby(true);
        }
    }

    public void OnJoinGame()
    {
        if (!ProfileInfo.ProfileInfosLoaded())
        {
            SetCurrentMenu(m_newProfileMenu);
            return;
        }

        if (SetCurrentMenu(m_lobbyMenu))
        {
            m_lobbyMenu.GetComponent<LobbyMenu>().InitLobby(false);
        }
    }

    public void OnProfileButton()
    {
        SetCurrentMenu(m_profileMenu);
    }

    public void OnNewProfileButton()
    {
        SetCurrentMenu(m_newProfileMenu);
    }

    public void OnNewProfileOkButton()
    {
        ProfileInfo.AddProfileInfo(m_newProfileMenu.GetComponent<ProfileManager>().GetProfileInfo());
        ProfileInfo.SaveProfileInfos();
        OnBack("NewProfile");
    }

    public void OnModifyProfileButton()
    {
        m_modifyProfileMenu.GetComponent<ProfileManager>().SetProfile(ProfileInfo.SelectedProfileInfo());
        SetCurrentMenu(m_modifyProfileMenu);
    }

    public void OnModifyProfileOkButton()
    {
        ProfileInfo.ReplaceProfileInfo(m_modifyProfileMenu.GetComponent<ProfileManager>().GetProfileInfo());
        ProfileInfo.SaveProfileInfos();
        OnBack("ModifyProfile");
    }

    public void OnStatsButton()
    {
        if(SetCurrentMenu(m_statsMenu))
        {
            m_statsMenu.GetComponent<StatsMenu>().UpdateValues();
        }
    }

    public void SetActive(bool value)
    {
        m_gatesOnTheMove = !value;
        gameObject.SetActive(value);
    }

    static public MainMenu MainMenuSingleton()
    {
        return s_mainMenuSingleton;
    }
}