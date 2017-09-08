using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkMenu : MonoBehaviour
{
    [SerializeField]
    private MainNetworkManager m_networkManager;
    private Canvas m_connectionMenu;
    private Canvas m_onJoiningGameMenu;

    public void Start()
    {
        // Menus
        Canvas[] allCanvas = GetComponentsInChildren<Canvas>();

        foreach (Canvas canvas in allCanvas)
        {
            // [OnJ]oiningGame Menu
            if (canvas.name.StartsWith("OnJ"))
            {
                m_onJoiningGameMenu = canvas;
                m_onJoiningGameMenu.gameObject.SetActive(false);
            }

            // [Con]nection Menu
            if (canvas.name.StartsWith("Con"))
            {
                m_connectionMenu = canvas;
            }
        }
    }

    public void OnServerButtonPressed()
    {
        if (m_networkManager.StartDiscoveryAsServer())
            ShowConnectionMenu(false);
    }

    public void OnClientButtonPressed()
    {
        if (m_networkManager.StartDiscoveryAsClient())
            ShowOnJoiningGameMenu();
    }

    public void OnCancelJoiningPressed()
    {
        m_networkManager.StopDiscovery();

        ShowConnectionMenu(true);
    }

    public void ShowConnectionMenu(bool show = true)
    {
        m_connectionMenu.gameObject.SetActive(show);

        if (show)
        {
            m_onJoiningGameMenu.gameObject.SetActive(false);
        }
    }

    public void ShowOnJoiningGameMenu(bool show = true)
    {
        m_onJoiningGameMenu.gameObject.SetActive(show);

        if (show)
        {
            m_connectionMenu.gameObject.SetActive(false);
        }
    }

}