using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class PlayerLobbySlot : NetworkBehaviour
{
    [Header("Widgets")]
    [SerializeField] private Text m_playerNameLabel;
    [SerializeField] private RawImage m_readyMarker;

    [Header("Images")]
    [SerializeField] private Texture m_readyTexture;
    [SerializeField] private Texture m_notReadyTexture;

    public void Start()
    {

    }

    public void SetReadyMarkerTexture(bool ready)
    {
        if (ready)
        {
            m_readyMarker.texture = m_readyTexture;
        }
        else
        {
            m_readyMarker.texture = m_notReadyTexture;
        }
    }

    public void SetSelectedCharacter(int selectedCharacter)
    {
        // TO DO
    }

    public void SetPlayerName(string playerName)
    {
        m_playerNameLabel.text = playerName;
    }

    public void Update()
    {

    }
}