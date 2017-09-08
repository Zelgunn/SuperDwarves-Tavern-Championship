using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TokenCounter : MonoBehaviour
{
    public enum ExpandDirection
    {
        ExpandLeft = -1,
        ExpandRight = 1
    };
    [SerializeField] private RawImage m_baseToken;
    [SerializeField] private Texture m_tokenOff;
    [SerializeField] private Texture m_tokenOn;
    [SerializeField] private ExpandDirection m_expandDirection;

    private int m_maxTokenCount = 0;
    private List<RawImage> m_tokens = new List<RawImage>();

    public void SetTokenOnCount (int count)
    {
        for (int i = 0; i < m_maxTokenCount; i++)
        {
            if(i < count)
            {
                m_tokens[i].texture = m_tokenOn;
            }
            else
            {
                m_tokens[i].texture = m_tokenOff;
            }
        }
	}

    public void SetMaxTokenCount(int maxCount)
    {
        RemoveTokens();

        m_maxTokenCount = maxCount;

        for(int i=1; i<m_maxTokenCount; i++)
        {
            RawImage token = GameObject.Instantiate(m_baseToken);

            Vector3 pos = token.transform.position;
            pos.y = m_baseToken.transform.position.y;
            pos.x += (i % 5) * 30 * (int) m_expandDirection;
            pos.y -= (i / 5) * 30;
            token.transform.position = pos;
            token.transform.SetParent(transform);
            token.transform.localScale = m_baseToken.transform.localScale;

            m_tokens.Add(token);
        }
    }

    public void RemoveTokens()
    {
        foreach (RawImage token in m_tokens)
        {
            if (token == m_baseToken)
                continue;

            GameObject.Destroy(token.gameObject);
        }

        m_baseToken.texture = m_tokenOff;
        m_tokens = new List<RawImage>();
        m_tokens.Add(m_baseToken);
    }
}