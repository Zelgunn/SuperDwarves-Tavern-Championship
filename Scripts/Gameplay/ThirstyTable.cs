using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ThirstyTable : NetworkBehaviour
{
    //// Données
    // Par défaut
    [Header ("Réglages bière")]
    private float m_baseBeerAmount = 0;
    private float m_baseTotalBeerAmount = 0;
    [SerializeField] float m_baseBeerConsumption = 0.5f;
    [SerializeField] private float m_baseMaximumBeerAmount = 50;

    // Courantes
    [SyncVar] private float m_beerAmount = 0;
    [SyncVar]  private float m_totalBeerAmount = 0;
    [SyncVar] private float m_beerConsumption;
    private float m_consumptionModifier = 1.0f;
    [SyncVar] private float m_maximumBeerAmount;

    [SerializeField] private Drinker[] m_drinkers;

    private Buff m_buff = null;

    public void Awake()
    {
        m_baseBeerConsumption *= m_drinkers.Length;
        m_baseMaximumBeerAmount += m_drinkers.Length * m_baseMaximumBeerAmount / 2;

        ResetBeer();
    }

    public void Update()
    {
        float bufferBeerAmount = m_beerAmount;
        if (isServer)
        {
            m_beerAmount -= m_beerConsumption * m_consumptionModifier * Time.deltaTime;
            RandomizeBuff();
        }

        if (m_beerAmount <= 0)
        {
            m_beerAmount = 0;
            if (bufferBeerAmount > 0)
            {
                UpdateDrinkers();
                if(isServer)
                    RemoveBuff();
            }
        }
        else
        {
            UpdateBuff();
        }
        
    }

    private void RandomizeBuff()
    {
        if ((m_buff != null) || (m_beerAmount <= 0))
            return;

        float random = Random.Range(0f, 1f);

        if(random < (0.05f * Time.deltaTime))
        {
            SetBuff(Buff.BuffTypes.SpeedUp, Random.Range(4.0f, 6.0f));
        }
        else if (random < (0.1f * Time.deltaTime))
        {
            SetBuff(Buff.BuffTypes.Stun, Random.Range(4.0f, 6.0f));
        }
    }

    [Server]
    private void RemoveBuff()
    {
        m_buff = null;

        if (isServer)
            RpcRemoveBuff();
    }

    [ClientRpc]
    private void RpcRemoveBuff()
    {
        m_buff = null;
    }
    
    [Server]
    private void SetBuff(Buff.BuffTypes type, float duration)
    {
        m_buff = new Buff(type, duration);
        if (isServer)
            RpcSetBuff(type, duration);
    }

    [ClientRpc]
    private void RpcSetBuff(Buff.BuffTypes type, float duration)
    {
        m_buff = new Buff(type, duration); 
    }

    private void UpdateBuff()
    {
        if (m_buff == null)
            return;

        float consumptionModifier = 1.0f;

        if (m_buff.UpdateDuration(Time.deltaTime) <= 0)
        {
            if(isServer)
                RemoveBuff();
        }
        else
        {
            switch (m_buff.Type())
            {
                case Buff.BuffTypes.Stun:
                    consumptionModifier = 0;
                    break;
                case Buff.BuffTypes.SpeedUp:
                    consumptionModifier = 5;
                    break;
            }
        }

        if(m_consumptionModifier != consumptionModifier)
        {
            m_consumptionModifier = consumptionModifier;
            UpdateDrinkers();
        }
    }

    private void UpdateDrinkers()
    {
        foreach (Drinker drinker in m_drinkers)
        {
            if (m_beerAmount > 0)
                drinker.SetSpeedModifier(Mathf.Sqrt(m_consumptionModifier));

            drinker.SetDrinking(m_beerAmount > 0);
        }
    }

    public void ResetBeer()
    {
        m_beerAmount = m_baseBeerAmount;
        m_totalBeerAmount = m_baseTotalBeerAmount;
        m_beerConsumption = m_baseBeerConsumption;
        m_maximumBeerAmount = m_baseMaximumBeerAmount;
        
        if(isServer)
            RemoveBuff();

        UpdateDrinkers();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!isServer)
            return;

        GameObject gObject = other.gameObject;
        if(gObject.tag == "Player")
        {
            ServeBeerToTable(gObject.GetComponent<PlayerBeer>());
            UpdateDrinkers();
        }
    }

    [Server]
    private void ServeBeerToTable(PlayerBeer playerBeer)
    {
        float playerBeerAmount = playerBeer.BeerAmount();
        float ServeableBeerAmount = Mathf.Min(playerBeerAmount, m_maximumBeerAmount - m_beerAmount);

        m_beerAmount += ServeableBeerAmount;
        m_totalBeerAmount += ServeableBeerAmount;

        playerBeer.ServeBeer(ServeableBeerAmount);
    }
}
