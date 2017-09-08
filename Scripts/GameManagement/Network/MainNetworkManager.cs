using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MainNetworkManager : NetworkManager
{
    static private MainNetworkManager s_singleton;
    private CustomNetworkDiscovery m_discovery;

    public void Start()
    {
        // Network Discovery
        m_discovery = GetComponent<CustomNetworkDiscovery>();
        m_discovery.CustomInitialize();

        s_singleton = this;
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
    }

    public bool StartDiscoveryAsClient()
    {
        return m_discovery.StartAsClient();
    }

    public bool StartDiscoveryAsServer()
    {
        if (!m_discovery.StartAsServer())
        {
            return false;
        }

        if (NetworkClient.active || NetworkServer.active)
            return false;

        StartHost();

        return true;
    }

    public void StopDiscovery()
    {
        m_discovery.StopBroadcast();
    }

    static public MainNetworkManager Singleton()
    {
        return s_singleton;
    }
}