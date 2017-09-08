using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class PlayerBeerInfo : MonoBehaviour
{
    public enum PlayerBeerState
    {
        BarrelEmpty,
        BarrelFull,
        BarrelToken
    };
    private PlayerBeer m_localPlayerBeer;
    private RawImage m_icon;

    private PlayerBeerState m_previousState = PlayerBeerState.BarrelEmpty;
    [SerializeField]
    private Texture m_barrelToken;
    [SerializeField]
    private Texture m_barrelFull;
    [SerializeField]
    private Texture m_BarrelEmpty;

    public void Start()
    {
        m_icon = GetComponent<RawImage>();
	}

	public void Update ()
    {
        if (m_localPlayerBeer == null)
            return;

        PlayerBeerState currentState;

        if(m_localPlayerBeer.HasToGetMoney())
        {
            currentState = PlayerBeerState.BarrelToken;
        }
        else if(m_localPlayerBeer.BeerAmount() > 0)
        {
            currentState = PlayerBeerState.BarrelFull;
        }
        else
            currentState = PlayerBeerState.BarrelEmpty;

        if(currentState != m_previousState)
        {
            m_previousState = currentState;
            switch(currentState)
            {
                case PlayerBeerState.BarrelFull:
                    m_icon.texture = m_barrelFull;
                    break;
                case PlayerBeerState.BarrelToken:
                    m_icon.texture = m_barrelToken;
                    break;
                case PlayerBeerState.BarrelEmpty:
                    m_icon.texture = m_BarrelEmpty;
                    break;
            }
        }
	}

    public void SetPlayer(CharacterManager character)
    {
        m_localPlayerBeer = character.GetComponent<PlayerBeer>();
    }
}
