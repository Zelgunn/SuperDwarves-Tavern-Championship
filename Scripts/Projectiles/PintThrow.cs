using UnityEngine;
using System.Collections;

public class PintThrow : MonoBehaviour
{
    private int m_team = -1;

    public void Start()
    {
        GameObject.Destroy(gameObject, 15);
    }

    public void SetTeam(int team)
    {
        m_team = team;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;

        GameObject gObject = other.gameObject;

        if (gObject.tag == "Player")
        {
            StunPlayer(gObject);
        }
        else
        {
            //GameObject.Destroy(gameObject);
        }
    }

    public void OnCollisionEnter(Collision other)
    {
        if (other.collider.isTrigger)
            return;

        GameObject gObject = other.gameObject;

        if (gObject.tag == "Player")
        {
            StunPlayer(gObject);
        }
    }

    public void StunPlayer(GameObject player)
    {
        PlayerNetworkData playerNetData = player.GetComponent<PlayerNetworkData>();
        if (playerNetData.PlayerTeam() == m_team)
            return;

        CharacterManager character = player.GetComponent<CharacterManager>();
        character.AddBuff(new Buff(Buff.BuffTypes.Stun, 1));

        GameObject.Destroy(gameObject);
    }
}