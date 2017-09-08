using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Chest : NetworkBehaviour
{
    [SerializeField] private ChestMobile m_mobilePart;
    private int m_playerCountInFrontOfChest = 0;

    public void OnTriggerEnter(Collider other)
    {
        if (!isServer)
            return;

        GameObject gObject = other.gameObject;
        if (gObject.tag == "Player")
        {
            m_playerCountInFrontOfChest++;
            OpenChest(gObject);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (!isServer)
            return;

        GameObject gObject = other.gameObject;
        if (gObject.tag == "Player")
        {
            m_playerCountInFrontOfChest--;
            CloseChest(gObject);
        }
    }

    [Server]
    private void OpenChest(GameObject player)
    {
        if(m_playerCountInFrontOfChest == 1)
        {
            StartCoroutine(m_mobilePart.OpenChestCoroutine());
            RpcOpenChest(player);
            player.GetComponent<CharacterManager>().AddBuff(new Buff(Buff.BuffTypes.SpeedUp, 5.0f));
        }
    }

    [ClientRpc]
    private void RpcOpenChest(GameObject player)
    {
        StartCoroutine(m_mobilePart.OpenChestCoroutine());
    }

    [Server]
    private void CloseChest(GameObject player)
    {
        if (m_playerCountInFrontOfChest == 0)
        {
            StartCoroutine(m_mobilePart.OpenChestCoroutine());
            RpcCloseChest(player);
        }
    }

    [ClientRpc]
    private void RpcCloseChest(GameObject player)
    {
        StartCoroutine(m_mobilePart.CloseChestCoroutine());
    }
}
