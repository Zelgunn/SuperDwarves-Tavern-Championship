using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerIngameManager : NetworkBehaviour
{
    private PlayerManager m_playerManager;

    private CharacterManager m_character;

    [SyncVar] private int m_id;

    public void Start()
    {
        m_playerManager = GetComponent<PlayerManager>();
        if (isServer)
        {
            m_id = PlayerManager.AllPlayerManagers().Count;
            RpcSetId(m_id);
        }
	}
	
    public void Update()
    {
	}

    public void InitializeCharacter()
    {
        if(isLocalPlayer)
        {
            CmdSpawnCharacter(m_playerManager.Team(), m_id);
        }
    }

    [ClientRpc]
    public void RpcSetId(int id)
    {
        m_id = id;
    }

    public int Id()
    {
        return m_id;
    }

    [Command]
    private void CmdSpawnCharacter(int team, int id)
    {
        GameObject characterPrefab = GameManager.CharacterPrefab();
        Transform spawn;
        if (team == 0)
            spawn = GameManager.RedTeamSpawn();
        else
            spawn = GameManager.BlueTeamSpawn();

        GameObject go = Instantiate(characterPrefab, spawn.position, spawn.rotation) as GameObject;
        go.GetComponent<PlayerNetworkMove>().SetTeam(team);

        NetworkServer.SpawnWithClientAuthority(go, connectionToClient);
        go.GetComponent<PlayerNetworkData>().RpcSetPlayerTeam(team);
        go.GetComponent<CharacterManager>().RpcSetOwner(id);
    }

    public CharacterManager Character()
    {
        return m_character;
    }

    public void SetCharacter(CharacterManager character)
    {
        m_character = character;
    }

    public void RespawnPlayer()
    {
        m_character.Respawn();
    }

    public void EnablePlayerControl(bool enable, bool enableRigidbody = true)
    {
        m_character.EnableControl(enable, enableRigidbody);
    }

    public void DisablePlayerControl(bool disable, bool enableRigidbody = false)
    {
        EnablePlayerControl(!disable, enableRigidbody);
    }
}
