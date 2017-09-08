using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Networking;

public class CustomNetworkDiscovery : NetworkDiscovery
{
    public bool CustomInitialize()
    {

        NetworkManager.singleton.networkAddress = Network.player.ipAddress;
        if (!Initialize())
            return false;

        broadcastData = "NetworkManager:" + NetworkManager.singleton.networkAddress + ":" + NetworkManager.singleton.networkPort;

        return true;
    }

    public override void OnReceivedBroadcast(string fromAddress, string data)
    {
        if (NetworkManager.singleton == null || NetworkManager.singleton.client != null)
            return;

        Debug.Log(data);

        // Data :
        // 0) "NetworkManager"
        // 1) Adresse réseau du serveur
        // 2) Port réseau utilisé par le serveur
        // 3) A voir (Pseudo de l'hôte, type de partie, config...)
        string[] splittedDatas = data.Split(':');

        if (splittedDatas.Length != 3)
            return;

        if (splittedDatas[0] != "NetworkManager")
            return;

        int port;
        bool success = Int32.TryParse(splittedDatas[2], out port);

        if(!success)
            return;

        NetworkManager.singleton.networkAddress = splittedDatas[1];
        NetworkManager.singleton.networkPort = port;
        NetworkManager.singleton.StartClient();
    }
}