using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PlayerBeer : NetworkBehaviour
{
    [SyncVar]
    private float m_beerAmount = 0;
    [SyncVar]
    private float m_maximumBeerAmount = 100;
    [SyncVar]
    private float m_totalServedAmount = 0;
    [SyncVar]
    private bool m_hasToGetMoney = false;

    [SerializeField] private GameObject m_barrel;

    private CharacterManager m_characterManager;


    public void Start()
    {
        m_characterManager = GetComponent<CharacterManager>();
    }

    public void ResetBeer()
    {
        m_beerAmount = 0;
        m_maximumBeerAmount = 100;
        m_totalServedAmount = 0;
        SetHasToGetMoney(false);
    }

    public void RefillBeer()
    {
        m_beerAmount = m_maximumBeerAmount;
        m_barrel.SetActive(true);
    }

    public float BeerAmount()
    {
        return m_beerAmount;
    }

    public bool BeerAmountAvailable(float beerAmount)
    {
        return beerAmount <= m_beerAmount;
    }

    public void UseBeer(float beerAmount)
    {
        m_beerAmount -= beerAmount;
    }

    public void ServeBeer(float beerAmount)
    {
        m_beerAmount -= beerAmount;
        int previousTotal = (int)m_totalServedAmount/100;
        m_totalServedAmount += beerAmount;

        if ((int)m_totalServedAmount / 100 > previousTotal)
        {
            SetHasToGetMoney(true);
            m_barrel.SetActive(false);
        }
    }

    public float MaximumBeerAmount()
    {
        return m_maximumBeerAmount;
    }

    public bool IsEmpty()
    {
        return (m_beerAmount == 0);
    }

    public void SetHasToGetMoney(bool hasToGetMoney)
    {
        m_hasToGetMoney = hasToGetMoney;
        if (m_characterManager == GameManager.LocalCharacter())
        {
            ProfileInfo.SelectedProfileInfo().IncreaseTotalBarrel(1);
            GameManager.ShowToken(hasToGetMoney);
        }
    }

    [Server]
    public void SetHasToGetMoneyServer(bool hasToGetMoney)
    {
        RpcSetHasToGetMoney(hasToGetMoney);
    }

    [ClientRpc]
    public void RpcSetHasToGetMoney(bool hasToGetMoney)
    {
        SetHasToGetMoney(hasToGetMoney);
    }

    public bool HasToGetMoney()
    {
        return m_hasToGetMoney;
    }

    public float TotalServedAmount()
    {
        return m_totalServedAmount;
    }
}
