using UnityEngine;
using System.Collections;

public class GameUI : MonoBehaviour
{
    static private GameUI s_singleton;
    [SerializeField] private GameObject m_soloUpperPanel;
    [SerializeField] private GameObject m_multiUpperPanel;

    public void Start()
    {
        s_singleton = this;
    }

    public void SetMultiplayer(bool multi)
    {
        m_soloUpperPanel.SetActive(!multi);
        m_multiUpperPanel.SetActive(multi);
    }

    public void SetSolo(bool solo)
    {
        SetMultiplayer(!solo);
    }

    static public GameUI Singleton()
    {
        return s_singleton;
    }
}