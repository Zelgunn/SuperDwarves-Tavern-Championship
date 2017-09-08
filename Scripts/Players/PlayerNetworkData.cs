using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerNetworkData : NetworkBehaviour
{
    //static private int s_playerCount = 0;
    //private int m_playerNumber = 0;
    private int m_playerTeam = -1;

    // Modèles
    [Header("Materials")]
    [SerializeField]
    private Material m_blueTeamMaterial;
    [SerializeField]
    private Material m_redTeamMaterial;

    [Header("Renderers")]
    [SerializeField]
    private Renderer m_dwarfRenderer;

    public void Start ()
    {
        //m_playerNumber = s_playerCount++;
	}

    public void Update()
    {
        
	}

    public int PlayerTeam()
    {
        return m_playerTeam;
    }

    public void SetPlayerTeam(int playerTeam)
    {
        if(isServer)
        {
            RpcSetPlayerTeam(playerTeam);
        }
    }

    [ClientRpc]
    public void RpcSetPlayerTeam(int playerTeam)
    {
        m_playerTeam = playerTeam;
        Material selectedMaterial = m_redTeamMaterial;
        if (playerTeam == 1)
            selectedMaterial = m_blueTeamMaterial;

        m_dwarfRenderer.material = selectedMaterial;
    }
}
